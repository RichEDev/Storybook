namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using System;

    /// <summary>
    /// The driving licence review details.
    /// </summary>
    public class DrivingLicenceReviewDetails
    {
        /// <summary>
        /// Gets or sets the check code.
        /// </summary>
        public string CheckCode { get; set; }

        /// <summary>
        /// Gets or sets the reviewer notes.
        /// </summary>
        public string ReviewerNotes { get; set; }

        /// <summary>
        /// Gets or sets the check code expiry.
        /// </summary>
        public DateTime CheckCodeExpiry { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the licence summary.
        /// </summary>
        public string LicenceSummary { get; set; }

        /// <summary>
        /// Gets or sets the licence summary comment.
        /// </summary>
        public string LicenceSummaryComment { get; set; }

        /// <summary>
        /// Gets or sets the driving licence number.
        /// </summary>
        public int DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the review date.
        /// </summary>
        public DateTime ReviewDate { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the reviewed by.
        /// </summary>
        public int ReviewedBy { get; set; }
    }
}
