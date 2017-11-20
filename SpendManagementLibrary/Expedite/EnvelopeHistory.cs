namespace SpendManagementLibrary.Expedite
{
    using System;
    using Enumerators.Expedite;

    /// <summary>
    /// Represents a change of state of an <see cref="Envelope"/>.
    /// </summary>
    public class EnvelopeHistory
    {
        /// <summary>
        /// The unique Id of the history item.
        /// </summary>
        public int EnvelopeHistoryId { get; set; }
        
        /// <summary>
        /// The Id of the envelope that this history item applies to.
        /// </summary>
        public int EnvelopeId { get; set; }

        /// <summary>
        /// The status of the envelope at this point in time.
        /// </summary>
        public EnvelopeStatus EnvelopeStatus { get; set; }

        /// <summary>
        /// An optional 'notes' or related information for this history item.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The EmployeeId of the user who changed the state.
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// The exact time this history item was created.
        /// </summary>
        public DateTime ModifiedOn { get; set; }
    }
}
