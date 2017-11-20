namespace SpendManagementLibrary.Expedite
{
    /// <summary>
    /// Represents a type of Envelope, from the EnvelopeTypes table in metabase.
    /// </summary>
    public class EnvelopeType
    {
        /// <summary>
        /// The unique primary key of this type.
        /// </summary>
        public int EnvelopeTypeId { get; set; }

        /// <summary>
        /// The label for human consumption.
        /// </summary>
        public string Label { get; set; }
    }
}
