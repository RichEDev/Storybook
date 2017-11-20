namespace EsrGo2FromNhsWcfLibrary.ESR
{
    /// <summary>
    /// The ESR trailer record.
    /// </summary>
    public class EsrTrailerRecord
    {
        /// <summary>
        /// The _record type.
        /// </summary>
        private readonly string recordType;

        /// <summary>
        /// The Number of records.
        /// </summary>
        private readonly int numRecords;

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrTrailerRecord"/> class.
        /// </summary>
        /// <param name="numberOfRecords">
        /// The number of records.
        /// </param>
        public EsrTrailerRecord(int numberOfRecords)
        {
            this.recordType = EsrRecordTypes.EsrTrailerRecordType;
            this.numRecords = numberOfRecords;
        }

        /// <summary>
        /// Enumeration of columns for the delimited data file row
        /// </summary>
        private enum TrailerRecordColRef
        {
            /// <summary>
            /// The record type.
            /// </summary>
            RecordType = 0,

            /// <summary>
            /// The number of records.
            /// </summary>
            NumRecords = 1
        }

        #region properties

        /// <summary>
        /// Gets or Sets the number of
        /// </summary>
        public int NumberOfRecords
        {
            get
            {
                return this.numRecords;
            }
        }

        /// <summary>
        /// Gets the record type
        /// </summary>
        public string RecordType
        {
            get
            {
                return this.recordType;
            }
        }

        #endregion

        /// <summary>
        /// Parses the delimited
        /// </summary>
        /// <param name="recordLine">
        /// Record to process
        /// </param>
        /// <returns>
        /// ESR trailer record
        /// </returns>
        public static EsrTrailerRecord ParseEsrTrailerRecord(string recordLine)
        {
            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);

            if (rec[(int)TrailerRecordColRef.RecordType].ToUpper() != EsrRecordTypes.EsrTrailerRecordType)
            {
                return null;
            }

            if (rec.Length != (int)TrailerRecordColRef.NumRecords + 1)
            {
                return null;
            }

            int numRecords;
            int.TryParse(rec[(int)TrailerRecordColRef.NumRecords], out numRecords);
            return new EsrTrailerRecord(numRecords);
        }
    }
}
