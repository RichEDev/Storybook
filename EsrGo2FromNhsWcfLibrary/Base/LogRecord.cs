namespace EsrGo2FromNhsWcfLibrary.Base
{
    using System;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Runtime.Serialization;

    /// <summary>
    /// The log record.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class LogRecord
    {
        #region Enums

        /// <summary>
        /// The log item types.
        /// </summary>
        public enum LogItemTypes
        {
            /// <summary>
            /// The default state
            /// </summary>
            [EnumMember]
            None = 0,

            /// <summary>
            /// Outbound file downloaded successfully
            /// </summary>
            [EnumMember]
            OutboundFileDownloaded = 1,

            /// <summary>
            /// Outbound file has downloaded but there is no data
            /// </summary>
            [EnumMember]
            OutboundFileDownloadedWithoutData = 2,

            /// <summary>
            /// Outbound file import started
            /// </summary>
            [EnumMember]
            OutboundFileImportStarted = 3,

            /// <summary>
            /// Outbound file import was successful
            /// </summary>
            [EnumMember]
            OutboundFileImportedSuccessfully = 4,

            /// <summary>
            /// Outbound file import was unsuccessful
            /// </summary>
            [EnumMember]
            OutboundFileImportFailed = 5,

            /// <summary>
            /// Inbound file processed to the ESRFileTransfer database successfully
            /// </summary>
            [EnumMember]
            InboundFileProcessedSuccessfully = 6,

            /// <summary>
            /// Inbound file processed to the ESRFileTransfer database unsuccessfully
            /// </summary>
            [EnumMember]
            InboundFileProcessingFailed = 7,

            /// <summary>
            /// Inbound file successfully uploaded to the NHS hub
            /// </summary>
            [EnumMember]
            InboundFileUploadedSuccessfully = 8,

            /// <summary>
            /// Inbound file failed to upload to the NHS hub
            /// </summary>
            [EnumMember]
            InboundFileUploadFailed = 9,

            /// <summary>
            /// The outbound file format does not meet the validation requirements
            /// </summary>
            [EnumMember]
            OutboundFileValidationFailed = 10,

            /// <summary>
            /// Log notification of the SEL File Service on development server stopping
            /// </summary>
            [EnumMember]
            SelFileServiceStopped = 11,

            /// <summary>
            /// Log notification of the SEL File Service on development server having an error
            /// </summary>
            [EnumMember]
            SelFileServiceErrored = 12,

            /// <summary>
            /// Log notification of the ESR Service on the N3 machine stopping
            /// </summary>
            [EnumMember]
            EsrServiceStopped = 13,

            /// <summary>
            /// Log notification of the ESR Service on the N3 machine having an error
            /// </summary>
            [EnumMember]
            EsrServiceErrored = 14,

            /// <summary>
            /// This is a notification to inform that the outbound file could not be downloaded from the N3 FTP site
            /// </summary>
            [EnumMember]
            OutboundFileDownloadFailed = 15,

            /// <summary>
            /// This is a mid-processing message status for the ESR outbound file
            /// </summary>
            [EnumMember]
            OutboundFileProgressMessage = 16
        }

        /// <summary>
        /// The log reason type.
        /// </summary>
        public enum LogReasonType
        {
            /// <summary>
            /// The none.
            /// </summary>
            [EnumMember]
            None = 0,

            /// <summary>
            /// The max length exceeded.
            /// </summary>
            [EnumMember]
            MaxLengthExceeded = 1,

            /// <summary>
            /// The mandatory field.
            /// </summary>
            [EnumMember]
            MandatoryField = 2,

            /// <summary>
            /// The unique field.
            /// </summary>
            [EnumMember]
            UniqueField = 3,

            /// <summary>
            /// The wrong data type.
            /// </summary>
            [EnumMember]
            WrongDataType = 4,

            /// <summary>
            /// The success.
            /// </summary>
            [EnumMember]
            Success = 5,

            /// <summary>
            /// The warning.
            /// </summary>
            [EnumMember]
            Warning = 6,

            /// <summary>
            /// The error.
            /// </summary>
            [EnumMember]
            Error = 7,

            /// <summary>
            /// The SQL error.
            /// </summary>
            [EnumMember]
            SqlError = 8,

            /// <summary>
            /// The success add.
            /// </summary>
            [EnumMember]
            SuccessAdd = 9,

            /// <summary>
            /// The success update.
            /// </summary>
            [EnumMember]
            SuccessUpdate = 10,

            /// <summary>
            /// The success delete.
            /// </summary>
            [EnumMember]
            SuccessDelete = 11
        }

        /// <summary>
        /// The transfer types.
        /// </summary>
        public enum TransferTypes
        {
            /// <summary>
            /// The none.
            /// </summary>
            [EnumMember]
            None = 0,

            /// <summary>
            /// The ESR inbound.
            /// </summary>
            [EnumMember]
            EsrInbound = 1,

            /// <summary>
            /// The ESR outbound.
            /// </summary>
            [EnumMember]
            EsrOutbound
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the meta base.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string MetaBase { get; set; }

        /// <summary>
        /// Gets or sets the NHS VPD.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string NhsVpd { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the log item type.
        /// </summary>
        [DataMember(IsRequired = true)]
        public LogItemTypes LogItemType { get; set; }

        /// <summary>
        /// Gets or sets the transfer type.
        /// </summary>
        [DataMember(IsRequired = true)]
        public TransferTypes TransferType { get; set; }

        /// <summary>
        /// Gets or sets the Log Item ID.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int LogId { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets a reason for the log item.
        /// </summary>
        [DataMember(IsRequired = false)]
        public LogReasonType LogReason { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Source { get; set; }

        #endregion

        /// <summary>
        /// Class properties.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>PropertyInfo[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public PropertyInfo[] ClassProperties()
        {
            if (MemoryCache.Default.Contains(string.Format("Properties_{0}", "LogRecord")))
            {
                return MemoryCache.Default.Get(string.Format("Properties_{0}", "LogRecord")) as PropertyInfo[];
            }

            var properties = this.GetType().GetProperties();

            MemoryCache.Default.Add(
                string.Format("Properties_{0}", "LogRecord"), properties, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(60) });
            return properties;
        }
    }
}
