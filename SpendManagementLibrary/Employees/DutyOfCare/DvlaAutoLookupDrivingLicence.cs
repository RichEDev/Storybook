namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using Interfaces;
    using System;

    /// <summary>
    /// Driving licence which confims to duty of care. 
    /// </summary>
    public class DvlaAutoLookupDrivingLicence : IDutyOfCareDocument
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
        /// Gets or sets value indication if the users without valid driving licence will be blocked from claiming mileage 
        /// </summary>
        public bool IsBlocked { get; private set; }

        /// <summary>
        /// Gets or sets a Value which indicates if the document is reviewed or not
        /// </summary>
        public bool IsReviewed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is valid licence.
        /// </summary>
        public bool IsValidLicence { get; }

        /// <summary>
        /// Gets a value indicating whether manual licence is valid.
        /// </summary>
        public bool IsManualLicenceValid { get; }
        /// <summary>
        /// Initialises the Driving Licence document
        /// </summary>
        /// <param name="registration">car registration number</param>
        /// <param name="validDate">valid date of the document</param>
        /// <param name="isBlocked">is there a check need on the document validity</param>
        /// <param name="isReviewed">Check to see if the document is reviewed or not</param>
        /// <param name="isValidLicence">Value indicating whether is valid licence.</param>
        /// <param name="isManualLicenceValid">Value indicating whether manual licence is valid.</param>
        public DvlaAutoLookupDrivingLicence(string registration, DateTime validDate, bool isBlocked, bool isReviewed,bool isValidLicence, bool isManualLicenceValid)
        {
            this.CarRegistrationNumber = registration;
            this.ValidDate = validDate;
            this.IsBlocked = isBlocked;
            this.IsReviewed = isReviewed;
            this.IsValidLicence = isValidLicence;
            this.IsManualLicenceValid = isManualLicenceValid;
        }

        /// <summary>
        /// This method checks this document has expired or not.
        /// </summary>
        /// <param name="expenseItemDate">Date of expense item</param>
        /// <returns><see cref="DocumentExpiryResult"/>Result which says if there is valid document or document is exipred</returns>
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

            if (this.IsBlocked)
            {
                var zeroDate = new DateTime(1900, 1, 1);

                if (this.ValidDate == null || this.ValidDate.Equals(zeroDate) || this.ValidDate.Equals(DateTime.MinValue))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = "You do not have a driving licence attached.";
                    return expiryResult;
                }

                if (this.ValidDate < expenseItemDate.Date)
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = "Your driving licence has expired. " + $"This expired on {this.ValidDate:d}.";
                    return expiryResult;
                }
            }

            expiryResult.HasExpired = false;
            return expiryResult;
        }
    }
}
