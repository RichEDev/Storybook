namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Globalization;

    public class EsrHeaderRecord
    {
        /// <summary>
        /// Indicates the record type
        /// </summary>
        private readonly string recordType;

        /// <summary>
        /// The filename of the ESR File
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// The version of the NHS Hub interface software that produced the file
        /// </summary>
        private readonly string interfaceVersion;

        /// <summary>
        /// The VPD number the file pertains to
        /// </summary>
        private readonly int vpdNumber;

        /// <summary>
        /// Extract type code (F=Full, C=Changes) (derived from the filename)
        /// </summary>
        private readonly char extractTypeCode;

        /// <summary>
        /// File Extract Date
        /// </summary>
        private readonly DateTime fileExtractDate;

        /// <summary>
        /// Date of previous file extract
        /// </summary>
        private readonly DateTime previousFileExtractDate;

        /// <summary>
        /// Unique Id (usually a sequentially incremented number) (derived from the filename)
        /// </summary>
        private readonly int uniqueFileSequenceId;

        /// <summary>
        /// File creation timestamp
        /// </summary>
        private readonly DateTime fileCreationDate;

        #region properties

        /// <summary>
        /// Gets the ESR record type
        /// </summary>
        public string RecordType
        {
            get
            {
                return this.recordType;
            }
        }

        /// <summary>
        /// Gets the filename of the ESR file
        /// </summary>
        public string Filename
        {
            get
            {
                return this.fileName;
            }
        }

        /// <summary>
        /// Gets the version of the NHS Hub interface software that produced the file
        /// </summary>
        public string InterfaceVersion
        {
            get
            {
                return this.interfaceVersion;
            }
        }

        /// <summary>
        /// Gets the VPD Number that the file pertains to
        /// </summary>
        public int VpdNumber
        {
            get
            {
                return this.vpdNumber;
            }
        }

        /// <summary>
        /// Gets the ESR file type (F=Full, C=Changes)
        /// </summary>
        public char FileTypeCode
        {
            get
            {
                return this.extractTypeCode;
            }
        }

        /// <summary>
        /// Gets the Date the ESR file was generated
        /// </summary>
        public DateTime RunDate
        {
            get
            {
                return this.fileExtractDate;
            }
        }

        /// <summary>
        /// Gets the Date the ESR file was generated
        /// </summary>
        public DateTime PreviousRunDate
        {
            get
            {
                return this.previousFileExtractDate;
            }
        }

        /// <summary>
        /// Gets the unique sequence Id for the ESR file
        /// </summary>
        public int UniqueFileSequenceId
        {
            get
            {
                return this.uniqueFileSequenceId;
            }
        }

        /// <summary>
        /// File creation timestamp
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                return this.fileCreationDate;
            }
        }

        #endregion properties

        /// <summary>
        /// Constructor for the ESR Header record type
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="creationDate"></param>
        /// <param name="trustVpd"></param>
        /// <param name="previousRunDate"></param>
        /// <param name="runDate"></param>
        /// <param name="interfaceversion"></param>
        public EsrHeaderRecord(string filename, DateTime creationDate, string trustVpd, DateTime previousRunDate, DateTime runDate, string interfaceversion)
        {
            this.recordType = EsrRecordTypes.EsrHeaderRecordType;
            this.fileName = filename;
            this.fileCreationDate = creationDate;

            if (!int.TryParse(trustVpd, out this.vpdNumber))
            {
                this.vpdNumber = -1;
            }

            this.previousFileExtractDate = previousRunDate;
            this.fileExtractDate = runDate;
            this.interfaceVersion = interfaceversion;

            string tmpVpd;
            ParseOutboundFilename(filename, out this.extractTypeCode, out this.uniqueFileSequenceId, out tmpVpd);
        }

        /// <summary>
        /// Extracts the sequence number and the extract type (Full or Change) from the filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="typeCode"></param>
        /// <param name="fileSequenceId"></param>
        /// <param name="trustVpd"></param>
        public static void ParseOutboundFilename(string fileName, out char typeCode, out int fileSequenceId, out string trustVpd)
        {
            // GO_VPD_EXP_GOF_20110826_00001234.DAT
            // Where:
            // GO		Denotes that the file is a Generic Outbound file
            // VPD		Trust Virtual Private Database number
            // EXP		Sub-file type (e.g. EXP - Expenses)
            // GO 		GO Outbound format/version (or if you like sub-file type)
            // F/C 		Extract Type Code (F – Full, C – Changes)
            // 20110826 File Extract Date (e.g. YYYYMMDD)
            // 00001234	Unique ID (usually a sequentially incremented number)
            // DAT 		Constant ‘DAT’ extension denoting data file
            fileName = fileName.Replace(".DAT", string.Empty);
            string[] filenameParts = fileName.Split('_');

            trustVpd = filenameParts[1];
            typeCode = Convert.ToChar(filenameParts[3].Substring(2, 1));
            
            int.TryParse(filenameParts[5], out fileSequenceId);
        }

        /// <summary>
        /// Enumeration of columns for the delimited data file row
        /// </summary>
        private enum HeaderRecordColRef
        {
            /// <summary>
            /// The record type.
            /// </summary>
            RecordType = 0,

            /// <summary>
            /// The ESR Filename.
            /// </summary>
            Filename,

            /// <summary>
            /// File creation timestamp
            /// </summary>
            CreationDate,

            /// <summary>
            /// The unique identifier for NHS employer organisation (VPD number)
            /// </summary>
            TrustIdentifier,

            /// <summary>
            /// Previous run date
            /// </summary>
            PreviousRunDate,

            /// <summary>
            /// Run date of this file
            /// </summary>
            RunDate,

            /// <summary>
            /// The interface version number
            /// </summary>
            InterfaceVersionNumber
        }

        /// <summary>
        /// Parses the delimited
        /// </summary>
        /// <param name="recordLine">
        /// Record to process
        /// </param>
        /// <returns>
        /// ESR trailer record
        /// </returns>
        public static EsrHeaderRecord ParseEsrHeaderRecord(string recordLine)
        {
            string[] rec = recordLine.Split(global::EsrGo2FromNhs.ESR.EsrFile.RecordDelimiter);

            if (rec[(int)HeaderRecordColRef.RecordType].ToUpper() != EsrRecordTypes.EsrHeaderRecordType)
            {
                return null;
            }

            if (rec.Length != (int)HeaderRecordColRef.InterfaceVersionNumber + 1)
            {
                return null;
            }

            string filename = rec[(int)HeaderRecordColRef.Filename];
            DateTime tmpDate;
            DateTime creationDate = DateTime.MinValue;
            if (DateTime.TryParseExact(rec[(int)HeaderRecordColRef.CreationDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
            {
                creationDate = tmpDate;
            }
            string trustvpd = rec[(int)HeaderRecordColRef.TrustIdentifier];
            DateTime prevrundate = DateTime.MinValue;
            if (DateTime.TryParseExact(rec[(int)HeaderRecordColRef.PreviousRunDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
            {
                prevrundate = tmpDate;
            }

            DateTime rundate = DateTime.MinValue;
            if (DateTime.TryParseExact(rec[(int)HeaderRecordColRef.RunDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
            {
                rundate = tmpDate;
            }

            string interfaceversion = rec[(int)HeaderRecordColRef.InterfaceVersionNumber];
            return new EsrHeaderRecord(filename, creationDate, trustvpd, prevrundate, rundate, interfaceversion);
        }
    }
}
