namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using Interfaces;
    using Helpers;
    using System;
    using System.Data;
    using Enumerators;

    ///<summary>
    /// The driving licence for this employee, of type non gb
    /// </summary>

    public class NonGbDrivingLicence : IDutyOfCareDocument
    {
        /// <summary>
        /// Gets or sets the start date for this driving licence.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date the licence is valid until
        /// </summary>
        public DateTime? ValidDate { get; set; }

        /// <summary>
        /// Gets or sets the Car Registration Number.
        /// </summary>
        public string CarRegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets whether the block is needed on the document.
        /// </summary>
        public bool IsBlocked { get; private set; }

        /// <summary>
        /// Gets or sets the driving licence review status
        /// </summary>
        public string ReviewStatus { get; set; }

        /// <summary>
        /// Set the string for if the document is awaiting review.
        /// </summary>
        public const string AwaitingReview = "is awaiting review";

        /// <summary>
        /// Set the string for if the document has failed review.
        /// </summary>
        public const string FailedReview = "has failed review";

        /// <summary>
        /// Gets a value indicating whether the licence is valid (passed review, not expired, passed DVLA lookup).
        /// </summary>
        public bool IsValidLicence { get; }

        /// <summary>
        /// Gets a value indicating whether the licence is valid. This is for manual licences only, (ie, it has not come from a DVLA lookup).
        /// </summary>
        public bool IsManualLicenceValid { get; }

        /// <see cref="NonGbDrivingLicence"/>
        /// <summary>
        /// Initialises the NonGbDrivingLicence document
        /// </summary>
        /// <param name="registration">
        /// Car registration number
        /// </param>
        /// <param name="startDate">
        /// Start date of the licence
        /// </param>
        /// <param name="validDate">
        /// Date the document is valid until
        /// </param>
        /// <param name="isBlocked">
        /// Is a check needed on the document's validity
        /// </param>
        /// <param name="reviewStatus">
        /// Gives status of review document.
        /// </param>
        /// <param name="isValidLicence">
        /// Value indicating whether the claimants licence is valid. If invalid the user will not be able to make a claim
        /// </param>
        /// <param name="isValidManualLicence">
        /// Value indicating whether the manual licence is valid (if it's review status is passed - ok). If invalid the user will not be able to make a claim
        /// </param>
        public NonGbDrivingLicence(string registration, DateTime startDate, DateTime validDate, bool isBlocked, string reviewStatus, bool isValidLicence, bool isValidManualLicence)
        {
            this.CarRegistrationNumber = registration;
            this.StartDate = startDate;
            this.ValidDate = validDate;
            this.IsBlocked = isBlocked;
            this.ReviewStatus = reviewStatus;
            this.IsValidLicence = isValidLicence;
            this.IsManualLicenceValid = isValidManualLicence;
        }

        /// <see cref="HasExpired"/>
        /// <summary>
        /// Checks whether this document (Non Gb Licence) has expired or not.
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
            expiryResult.IsValidManualLicence = this.IsManualLicenceValid;

            if (!this.IsValidLicence)
            {
                expiryResult.HasExpired = true;
                return expiryResult;
            }

            if (IsBlocked)
            {                                             
                //If the licence does have a valid date check it's not expired
                if (this.HasDate(this.ValidDate) && (this.ValidDate < expenseItemDate.Date))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = $"Your driving licence has expired. This expired on {this.ValidDate:d}.";
                    return expiryResult;
                }

                //If the licence has a start date, check it exists on or before the expense item date
                if (this.HasDate(this.StartDate) && expenseItemDate.Date < this.StartDate)
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = "You do not have a driving licence attached.";
                    return expiryResult;
                }

                //As non gb licence can have Valid and/or Start, or neither date(s), the review status check needs a condition for each possibility
                if ((this.HasDate(this.ValidDate) && this.ValidDate >= expenseItemDate && !string.IsNullOrWhiteSpace(this.ReviewStatus)) 
                    || (this.HasDate(this.StartDate) && this.StartDate <= expenseItemDate && !string.IsNullOrWhiteSpace(this.ReviewStatus)) 
                    || (!this.HasDate(this.ValidDate)) && (!this.HasDate(this.StartDate)) && !string.IsNullOrWhiteSpace(this.ReviewStatus))                        
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

        /// <summary>
        /// This method checks whether the non-gb licence has a date, by taking in the supplied date time (valid or start), and performing the three checks (null, mindate, default). 
        /// This is because non-gb licences don't have to have one or both dates. If none of these checks result in true then it has this date value.
        /// </summary>
        /// <param name="licenceDate">Takes a nullable DateTime (Valid or Start)</param>
        /// <returns>True or false, depending if the licence has the date or not</returns>
        private bool HasDate(DateTime? licenceDate)
        {
            return !licenceDate.Equals(DateTime.MinValue) && !licenceDate.Equals(default(DateTime)) && licenceDate.HasValue;
        }
    }
}