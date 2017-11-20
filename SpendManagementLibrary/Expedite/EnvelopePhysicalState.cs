namespace SpendManagementLibrary.Expedite
{
    /// <summary>
    /// Represents a detail about an <see cref="Envelope"/>'s physical state.
    /// </summary>
    public class EnvelopePhysicalState
    {
        /// <summary>
        /// The unique database Id.
        /// </summary>
        public int EnvelopePhysicalStateId { get; set; }

        /// <summary>
        /// The details of of the envelope state.
        /// </summary>
        public string Details { get; set; }
    }
}
