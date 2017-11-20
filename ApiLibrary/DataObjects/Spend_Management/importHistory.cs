namespace ApiLibrary.DataObjects.Spend_Management
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The import history.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class ImportHistory : DataClassBase
    {
        /// <summary>
        /// The history id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int historyId;

        /// <summary>
        /// The import id.
        /// </summary>
        [DataMember]
        public int importId;

        /// <summary>
        /// The log id.
        /// </summary>
        [DataMember]
        public int logId;

        /// <summary>
        /// The import date.
        /// </summary>
        [DataMember]
        public DateTime importedDate;

        /// <summary>
        /// The status.
        /// </summary>
        [DataMember]
        public int importStatus;

        /// <summary>
        /// The app type.
        /// </summary>
        [DataMember]
        public int applicationType;

        /// <summary>
        /// The data id.
        /// </summary>
        [DataMember]
        public int dataId;
    }
}
