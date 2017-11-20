using System.Linq;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary.Expedite
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Spend_Management;


    /// <summary>
    /// Represents the results of trying to attach multiple envelopes to a claim.
    /// </summary>
    public class ClaimEnvelopeAttachmentResults
    {
        /// <summary>
        /// The Overall result of the attachment process.
        /// This is set by adding results using <see cref="AddStatus"/>.
        /// </summary>
        public bool OverallResult { get; private set; }

        /// <summary>
        /// If <see cref="OverallResult"/> is true, this CRN will be populated.
        /// </summary>
        public string ClaimReferenceNumber { get; set; }

        /// <summary>
        /// The Readonly list of results from attempting to attach envelopes to a claim.
        /// </summary>
        public List<EnvelopeAttachmentResult> Results { get; private set; }

        /// <summary>
        /// Creates a new Set of results.
        /// </summary>
        public ClaimEnvelopeAttachmentResults()
        {
            OverallResult = true;
            Results = new List<EnvelopeAttachmentResult>();
        }

        /// <summary>
        /// Adds a result. Updates the <see cref="OverallResult"/> property.
        /// </summary>
        /// <param name="status">The <see cref="ClaimEnvelopeAttachmentStatus"/> to add.</param>
        /// <param name="envelopeNumber">The EnvelopeNumber, if one applies to this result.</param>
        /// <returns>The OverallResult.</returns>
        public bool AddStatus(ClaimEnvelopeAttachmentStatus status, string envelopeNumber = null)
        {
            // add new result
            var ear = new EnvelopeAttachmentResult(status, envelopeNumber);
            Results.Add(ear);

            // update overall result
            OverallResult = Results.All(r => r.Success);
            return OverallResult;
        }
    }

    /// <summary>
    /// Represents the JavaScript friendly result of trying to attach an <see cref="Envelope"/> to a <see cref="Claim"/>.
    /// </summary>
    public class EnvelopeAttachmentResult
    {
        /// <summary>
        /// Whether the Attachment was a success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The reason why the attachment was not a success, if attachment failed.
        /// </summary>
        public string EnvelopeNumber { get; set; }

        /// <summary>
        /// The reason why the attachment was not a success, if attachment failed.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Creates a new JS friendly AttachmentResult from a <see cref="ClaimEnvelopeAttachmentStatus"/>.
        /// </summary>
        /// <param name="status">The Status to build from.</param>
        /// <param name="envelopeNumber">The EnvelopeNumber that this result applies to (if there is one)</param>
        public EnvelopeAttachmentResult(ClaimEnvelopeAttachmentStatus status, string envelopeNumber = null)
        {
            Success = status == ClaimEnvelopeAttachmentStatus.Success;
            Reason = EnumHelpers<ClaimEnvelopeAttachmentStatus>.GetDisplayValue(status);
            EnvelopeNumber = envelopeNumber;
        }
    }


    /// <summary>
    /// Represents the possible reasons why an <see cref="Envelope"/>Might
    /// </summary>
    public enum ClaimEnvelopeAttachmentStatus
    {
        /// <summary>
        /// The attachment was a success.
        /// </summary>
        [Display(Name = @"This envelope number is valid and would succeed in attachment.")]
        Success = 0,

        /// <summary>
        /// The attachment failed as the wrong AccountId was provided.
        /// </summary>
        [Display(Name = @"No account matching the specified Id was found.")]
        FailedNoAccount = -1,

        /// <summary>
        /// No claim matching the specified Id was found.
        /// </summary>
        [Display(Name = @"No claim matching the specified Id was found.")]
        FailedNoClaimFound = -2,

        /// <summary>
        /// The attachment failed as no Envelopes were supplied.
        /// </summary>
        [Display(Name = @"No envelope numbers were provided. Please enter at least one valid envelope number.")]
        FailedNoEnvelopeNumbersSupplied = -10,

        /// <summary>
        /// The attachment failed as the EnvelopeNumber could not be found.
        /// </summary>
        [Display(Name = @"No envelope matching this envelope number was found in the system. Please double-check your envelope number.")]
        FailedNoEnvelopeFound = -11,

        /// <summary>
        /// The attachment failed as the EnvelopeNumber has not been assigned to the account.
        /// </summary>
        [Display(Name = @"Our records show that this envelope has not been issued to your company. Please double-check your envelope number.")]
        FailedEnvelopeNotAssigned = -12,

        /// <summary>
        /// The attachment failed as the EnvelopeNumber has not been assigned to the account.
        /// </summary>
        [Display(Name = @"The envelope has already been used. Please use another envelope.")]
        FailedEnvelopeAlreadyUsed = -13,
    }
}