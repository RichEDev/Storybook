namespace ApiLibrary.DataObjects.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The import log.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class ImportLog : DataClassBase
    {
        /// <summary>
        /// The log id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true)]
        public int logID;

        /// <summary>
        /// The log type.
        /// </summary>
        [DataMember]
        public byte logType;

        /// <summary>
        /// The log name.
        /// </summary>
        [DataMember]
        public string logName;

        /// <summary>
        /// The successful lines.
        /// </summary>
        [DataMember]
        public int successfulLines;

        /// <summary>
        /// The failed lines.
        /// </summary>
        [DataMember]
        public int failedLines;

        /// <summary>
        /// The warning lines.
        /// </summary>
        [DataMember]
        public int warningLines;

        /// <summary>
        /// The expected lines.
        /// </summary>
        [DataMember]
        public int expectedLines;

        /// <summary>
        /// The processed lines.
        /// </summary>
        [DataMember]
        public int processedLines;
    }
}
