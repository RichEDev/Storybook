namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The claim envelope info.
    /// </summary>
    public class ClaimEnvelopeInfo : ApiRequest
    {       
        /// <summary>
        /// Gets or sets the Claim Envelope ID
        /// </summary>
        public int ClaimEnvelopeId;

        /// <summary>
        /// Gets or sets the Envelope Number
        /// </summary>
        public string EnvelopeNumber;

        /// <summary>
        /// Gets or sets the physical state of the envelope
        /// </summary>
        public string PhysicalState { get; set; }

        /// <summary>
        /// Gets or sets any excess charges from postage or handling.
        /// </summary>
        public string ExcessCharge { get; set; }

        /// <summary>
        /// Gets or sets details about if the envelope was processed / received / scanned after it had been marked lost.
        /// </summary>
        public string ProcessedAfterMarkedLost { get; set; }
    }
}