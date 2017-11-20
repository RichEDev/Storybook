namespace ApiLibrary.DataObjects.ESR
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The ESR trust.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrTrust : DataClassBase
    {
        /// <summary>
        /// The trust id.
        /// </summary>
        [DataClass(IsKeyField = true)]
        [DataMember(IsRequired = true)]
        public int trustID = 0;

        /// <summary>
        /// The trust VPD.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string trustVPD = string.Empty;

        /// <summary>
        /// The period type.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string periodType = string.Empty;

        /// <summary>
        /// The period run.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string periodRun = string.Empty;

        /// <summary>
        /// The run sequence number.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int runSequenceNumber;

        /// <summary>
        /// The FTP address.
        /// </summary>
        [DataMember]
        public string ftpAddress = string.Empty;

        /// <summary>
        /// The FTP username.
        /// </summary>
        [DataMember]
        public string ftpUsername = string.Empty;

        /// <summary>
        /// The FTP password.
        /// </summary>
        [DataMember]
        public string ftpPassword = string.Empty;

        /// <summary>
        /// The archived.
        /// </summary>
        [DataMember]
        public bool archived = false;

        /// <summary>
        /// The created on.
        /// </summary>
        [DataMember]
        public DateTime? createdOn;

        /// <summary>
        /// The modified on.
        /// </summary>
        [DataMember]
        public DateTime? modifiedOn;

        /// <summary>
        /// The trust name.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string trustName = string.Empty;

        /// <summary>
        /// The delimiter character.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string delimiterCharacter = string.Empty;

        /// <summary>
        /// The Esr Version number.
        /// </summary>
        [DataMember(IsRequired = true)]
        public byte ESRVersionNumber;

        /// <summary>
        /// The current Outbound File sequence number
        /// </summary>
        [DataMember]
        public int? currentOutboundSequence = null;

        /// <summary>
        /// The customer account id that the trust belongs to
        /// </summary>
        [DataMember]
        public int AccountId { get; set; }
    }
}
