namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary.Employees.DutyOfCare;

    /// <summary>
    /// A class which contains informtion about the document expiry
    /// </summary>
    public class DocumentExpiry : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Employees.DutyOfCare.DocumentExpiryResult, DocumentExpiry>
    {
        /// <summary>
        /// Gets or sets the Id of the car for which the document is invalid
        /// </summary>
        public int carId { get; set; }

        /// <summary>
        /// Gets of sets the flag indicating whether or not the document has expired
        /// </summary>
        public bool HasExpired { get; set; }

        /// <summary>
        /// Gets or sets information regarding which document name that expired, and when
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether licence is valid.
        /// </summary>
        public bool IsValidLicence { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether or not the document has failed review.
        /// </summary>
        public bool IsReviewFailed { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether or not the document is awaiting review.
        /// </summary>
        public bool IsAwaitingReview { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether or not the document needs to be updated.
        /// </summary>
        public bool UpdateDocument { get; set; }

        /// <summary>
        /// Gets or sets the duty of care expiry messages.
        /// </summary>
        public string DutyOfCareExpiryMessages { get; set; }


        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public DocumentExpiry From(DocumentExpiryResult dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.carId = dbType.carId;
            this.HasExpired = dbType.HasExpired;
            this.Reason = dbType.Reason;
            this.IsValidLicence = dbType.IsValidLicence;
            this.IsReviewFailed = dbType.IsReviewFailed;
            this.IsAwaitingReview = dbType.IsAwaitingReview;
            this.UpdateDocument = dbType.UpdateDocument;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public DocumentExpiryResult To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}