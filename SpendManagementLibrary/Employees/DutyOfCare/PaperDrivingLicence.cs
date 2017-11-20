namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using Interfaces;
    using Helpers;
    using System;
    using System.Data;
    using Enumerators;

    /// <summary>
    /// PaperDrivingLicence document which confims to duty of care
    /// </summary>
    public class PaperDrivingLicence : IDutyOfCareDocument
    {
        /// <summary>
        /// Gets or sets the expiry date for this driving license.
        /// </summary>
        public DateTime? ValidDate { get; set; }

        /// <summary>
        /// Gets or sets the Car Registration Number
        /// </summary>
        public string CarRegistrationNumber { get; set; }
        /// <summary>
        /// Gets or sets whether the document is blocked (and thus needs checking)
        /// </summary>
        public bool IsBlocked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is valid licence.
        /// </summary>
        public bool IsValidLicence { get; }

        /// <summary>
        /// Gets or sets the driving licence review status
        /// </summary>
        public string ReviewStatus { get; set; }
        
        /// <summary>
        /// Sets the text is the document is awaiting review
        /// </summary>
        public const string AwaitingReview = "is awaiting review";

        /// <summary>
        /// Sets the text is the document has failed review
        /// </summary>
        public const string FailedReview = "has failed review";

        /// <summary>
        /// Gets a value indicating whether manual licence is valid.
        /// </summary>
        public bool isManualLicenceValid { get; }


        /// <summary>
        /// initialises the PaperDrivingLicence document
        /// </summary>
        /// <param name="registration">
        /// car registration number
        /// </param>
        /// <param name="validDate">
        /// valid date of the document
        /// </param>
        /// <param name="isBlocked">
        /// is there a check needed on the document validity
        /// </param>
        /// <param name="reviewStatus">
        /// Gives status of document review
        /// </param>
        /// <param name="isValidLicence">
        /// Value indicating whether is valid licence.
        /// </param>
        /// <param name="isValidManualLicence">
        /// Value indicating whether manual licence is valid.
        /// </param>
        public PaperDrivingLicence(string registration, DateTime validDate, bool isBlocked, string reviewStatus, bool isValidLicence, bool isValidManualLicence)
        {
            CarRegistrationNumber = registration;
            this.ValidDate = validDate;
            IsBlocked = isBlocked;
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
            var zeroDate = new DateTime(1900, 1, 1);
            expiryResult.IsValidManualLicence = this.isManualLicenceValid;
            expiryResult.IsValidLicence = this.IsValidLicence;
            if (!this.IsValidLicence)
            {
                expiryResult.HasExpired = true;
                return expiryResult;
            }

            if (IsBlocked)
            {
                if ((expenseItemDate < this.ValidDate) || (this.ValidDate == null || this.ValidDate.Equals(zeroDate) || this.ValidDate.Equals(DateTime.MinValue)))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = "You do not have a driving licence attached.";
                    return expiryResult;
                }
                
                if (this.ValidDate <= expenseItemDate && !string.IsNullOrWhiteSpace(this.ReviewStatus))
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


