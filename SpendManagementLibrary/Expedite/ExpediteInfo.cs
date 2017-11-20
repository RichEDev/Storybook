namespace SpendManagementLibrary.Expedite
{
    /// <summary>
    /// Encapsulates the information necessary to mange receipts by Expedite.
    /// </summary>
    public class ExpediteInfo
    {
        /// <summary>
        /// If this receipt was modified by an expedite user, 
        /// this contains the name of the staff member.
        /// </summary>
        public string ExpediteUserName { get; set; }

        /// <summary>
        /// The database Id of the Envelope in which this 
        /// receipt arrived, if it was sent to Expedite.
        /// </summary>
        public int EnvelopeId { get; set; }
    }
}
