namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using Interfaces;
    using Helpers;
    using System;
    using System.Data;
    using Enumerators;

    /// <summary>
    /// The driving license for this employee.
    /// </summary>
    public class PhotoCardDrivingLicence : IDutyOfCareDocument
    {
        /// <summary>
        /// Gets or sets the expiry date for this driving license.
        /// </summary>
        public DateTime? ValidDate { get; set; }

        /// <summary>
        /// Gets or Sets the Car Registration Number
        /// </summary>
        public string CarRegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets whether the block is needed on the document
        /// </summary>
        public bool  IsBlocked { get; private set; }

        /// <summary>
        /// Gets or sets the driving licence review status
        /// </summary>
        public string ReviewStatus { get; set; }

        /// <summary>
        /// Sets the text for the document review status if it is awaiting review
        /// </summary>
        public const string AwaitingReview = "is awaiting review";

        /// <summary>
        /// Sets the text for the document review status if the review failed.
        /// </summary>
        public const string FailedReview = "has failed review";

        /// <summary>
        /// Gets a value indicating whether the licence is valid (passed review, not expired, passed DVLA lookup).
        /// </summary>
        public bool IsValidLicence { get; }

        /// <summary>
        /// Gets a value indicating whether the licence is valid. This is for manual licences only, (ie, it has not come from a DVLA lookup).
        /// </summary>
        public bool isManualLicenceValid { get; }

        ///<see cref="PhotoCardDrivingLicence"/>
        /// <summary>
        /// initialises the PhotoCardDrivingLicence document
        /// </summary>
        /// <param name="registration">
        /// Car registration number
        /// </param>
        /// <param name="validDate">
        /// The date the licence became valid
        /// </param>
        /// <param name="isBlocked">
        /// is there a check need on the document validity
        /// </param>
        /// <param name="reviewStatus">
        /// Gives status of review document.
        /// </param>
        /// <param name="isValidLicence">
        /// Value indicating whether is valid licence.
        /// </param>
        /// <param name="isValidManualLicence">
        /// Value indicating whether manual licence is valid.
        /// </param>
        public PhotoCardDrivingLicence(string registration, DateTime validDate, bool isBlocked, string reviewStatus, bool isValidLicence, bool isValidManualLicence)
        {
            this.CarRegistrationNumber = registration;
            this.ValidDate = validDate;
            this.IsBlocked = isBlocked;
            this.ReviewStatus = reviewStatus;
            this.IsValidLicence = isValidLicence;
            this.isManualLicenceValid = isValidManualLicence;
        }

        /// <see cref="HasExpired"/>
        /// <summary>
        /// Checks whether this document has expired or not.
        /// Returns the result of the check, and sets HasExpired to true or false depending on if it passed or not.
        /// Checks whether there is a licence, whether it has expired (and when) and the review status of the licence.
        /// </summary>
        /// <param name="expenseItemDate">
        /// Date of expense item
        /// </param>
        /// <returns name="expiryResult">
        /// This method returns a DocumentExpiryResult(), the result of the expiry check. 
        /// This result will indicate whether the licence is valid or not and, if not, why.
        /// </returns>
        public DocumentExpiryResult HasExpired(DateTime expenseItemDate)
        {
            var expiryResult = new DocumentExpiryResult();
            expiryResult.IsValidLicence = this.IsValidLicence;
            expiryResult.IsValidManualLicence = this.isManualLicenceValid;
            if (!this.IsValidLicence)
            {
                expiryResult.HasExpired = true;
                return expiryResult;
            }

            if (IsBlocked)
            {
                var zeroDate = new DateTime(1900, 1, 1);
                if (this.ValidDate == null || this.ValidDate.Equals(zeroDate) || this.ValidDate.Equals(default(DateTime)))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = "You do not have a driving licence attached.";
                    return expiryResult;

                }
                if (this.ValidDate < expenseItemDate.Date)
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = string.Format("Your driving licence has expired. " +
                           "This expired on {0:d}.", this.ValidDate);
                    return expiryResult;
                }

                if (this.ValidDate >= expenseItemDate && !string.IsNullOrWhiteSpace(this.ReviewStatus))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = $"Your driving licence {this.ReviewStatus}.";
                    expiryResult.UpdateDocument = FailedReview == this.ReviewStatus;

                    if (!expiryResult.UpdateDocument)
                    {
                        expiryResult.IsReviewFailed = true;
                    }

                    expiryResult.IsAwaitingReview = AwaitingReview == this.ReviewStatus;
                    return expiryResult;
                }
            }

            expiryResult.HasExpired = false;
            return expiryResult;
        }
    }
}


