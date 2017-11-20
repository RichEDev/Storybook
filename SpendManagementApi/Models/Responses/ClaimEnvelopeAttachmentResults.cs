namespace SpendManagementApi.Models.Responses
{
    using System;
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The claim envelope attachment results.
    /// </summary>
    public class ClaimEnvelopeAttachmentResults : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.ClaimEnvelopeAttachmentResults, ClaimEnvelopeAttachmentResults>
    {
        /// <summary>
        /// Gets or sets the Overall result of the attachment process.
        /// This is set by adding results using <see cref="AddStatus"/>.
        /// </summary>
        public bool OverallResult { get; private set; }

        /// <summary>
        /// Gets or sets the claim reference number.
        /// </summary>
        public string ClaimReferenceNumber { get; set; }

        /// <summary>
        /// Gets a readonly list of results from attempting to attach envelopes to a claim.
        /// </summary>
        public List<EnvelopeAttachmentResult> Results { get; private set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ClaimEnvelopeAttachmentResults From(SpendManagementLibrary.Expedite.ClaimEnvelopeAttachmentResults dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.ClaimReferenceNumber = dbType.ClaimReferenceNumber;
            this.OverallResult = dbType.OverallResult;

            var listOfEnvelopeAttachmentResults = new List<EnvelopeAttachmentResult>();

            foreach (var result in dbType.Results)
            {
                var envelopeAttachmentResult = new EnvelopeAttachmentResult().From(result, actionContext);
                listOfEnvelopeAttachmentResults.Add(envelopeAttachmentResult);
            }

            this.Results = listOfEnvelopeAttachmentResults;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.ClaimEnvelopeAttachmentResults To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}