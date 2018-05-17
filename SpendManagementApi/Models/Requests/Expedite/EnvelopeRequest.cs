namespace SpendManagementApi.Models.Requests.Expedite
{
    /// <summary>
    /// The envelope request.
    /// </summary>
    public class EnvelopeRequest
    {
        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// Gets or sets the envelope number.
        /// </summary>
        public string EnvelopeNumber { get; set; }
    }
}