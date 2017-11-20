namespace SpendManagementLibrary.Employees.DutyOfCare
{
    /// <summary>
    /// class which contains informtion about the document expiry
    /// </summary>
    public class DocumentExpiryResult
    {
        /// <summary>
        /// Id of the car for which the document is invalid
        /// </summary>
        public int carId { get; set; }
        /// <summary>
        /// returns the flag indicating whether or not the document has expired
        /// </summary>
        public bool HasExpired { get; set; }
        /// <summary>
        /// inofrmation regarding which document name that expired, and when
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Returns the flag indicating whether or not the document has failed review
        /// </summary>
        public bool IsReviewFailed { get; set; }

        /// <summary>
        /// Returns the flag indicating whether or not document is awaiting review
        /// </summary>
        public bool IsAwaitingReview { get; set; }

        /// <summary>
        /// Returns the flag indicating whether or not the document need to be updated
        /// </summary>
        public bool UpdateDocument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is valid licence.
        /// </summary>
        public bool IsValidLicence{get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether manual licence is valid.
        /// </summary>
        public bool IsValidManualLicence { get; set; }
}
}
