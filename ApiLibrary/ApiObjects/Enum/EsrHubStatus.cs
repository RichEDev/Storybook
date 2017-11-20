namespace ApiLibrary.ApiObjects.Enum
{
    /// <summary>
    /// The ESR hub status.
    /// </summary>
    public class EsrHubStatus
    {
        /// <summary>
        /// The ESR hub transfer status.
        /// </summary>
        public enum EsrHubTransferStatus
        {
            /// <summary>
            /// The success.
            /// </summary>
            Success = 0,

            /// <summary>
            /// The validation failed no header.
            /// </summary>
            ValidationFailedNoHeader = 1,

            /// <summary>
            /// The validation failed no footer.
            /// </summary>
            ValidationFailedNoFooter = 2,

            /// <summary>
            /// The validation failed record count.
            /// </summary>
            ValidationFailedRecordCount = 3,

            /// <summary>
            /// The validation failed wrong format.
            /// </summary>
            ValidationFailedWrongFormat = 4,

            /// <summary>
            /// The validation failed byte count.
            /// </summary>
            ValidationFailedByteCount = 5,

            /// <summary>
            /// The NHS VPD was not found.
            /// </summary>
            NhsVpdNotFound = 6,

            /// <summary>
            /// The validation wrong sequence number.
            /// </summary>
            ValidationWrongSequenceNumber = 7,

            /// <summary>
            /// Requested file for transfer again does not exist in download folder
            /// </summary>
            FileNotAvailableForDownload = 8,

            Unknown = -1
        }
    }
}
