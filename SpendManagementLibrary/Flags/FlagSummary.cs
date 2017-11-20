namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A flagged item and associated flag id.
    /// </summary>
    [Serializable]
    [DataContract]
    public class FlagSummary
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FlagSummary"/> class.
        /// </summary>
        /// <param name="flagID">
        /// The flag id.
        /// </param>
        /// <param name="flaggedItem">
        /// The instance of the flagged item.
        /// </param>
        public FlagSummary(int flagID, FlaggedItem flaggedItem)
        {
            this.FlagID = flagID;
            this.FlaggedItem = flaggedItem;
        }

        /// <summary>
        /// Gets the flag id.
        /// </summary>
        [DataMember]
        public int FlagID { get; private set; }

        /// <summary>
        /// Gets the flagged item.
        /// </summary>
        [DataMember]
        public FlaggedItem FlaggedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the flagDescription as sComments to pass to the mobile ap
        /// </summary>
        [DataMember]
        public string sComments
        {
            get
            {
                return this.FlaggedItem != null ? this.FlaggedItem.FlagDescription : string.Empty;
            }
        }
    }
}
