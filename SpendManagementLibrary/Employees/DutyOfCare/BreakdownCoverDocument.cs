using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SpendManagementLibrary.Enumerators;

namespace SpendManagementLibrary.Employees.DutyOfCare
{
    /// <summary>
    /// Insurance document which confims to duty of care
    /// </summary>
    public class BreakdownCoverDocument : IDutyOfCareDocument
    {

        /// <summary>
        /// Expiry Date
        /// </summary>
        public DateTime? ValidDate { get; set; }
        /// <summary>
        /// Car Registration Number
        /// </summary>
        public string CarRegistrationNumber { get; set; }
        /// <summary>
        /// is the block needed on the document's validity
        /// </summary>
        public bool IsBlocked { get; private set; }

        /// <summary>
        /// Check to see if the document is reviewed or not
        /// </summary>
        public bool IsReviewed { get; private set; }

        /// <summary>
        /// Expiry date of the document
        /// </summary>
        public DateTime? ExpiryDate { get; private set; }

        /// <summary>
        /// initialises the insurance document
        /// </summary>
        /// <param name="registration">car registration number</param>
        /// <param name="validDate">valid date of the document</param>
        /// <param name="isBlocked">is there a check need on the document validity</param>
        /// <param name="isReviewed">Check to see if the document is reviewed or not</param>
        public BreakdownCoverDocument(string registration, DateTime validDate, bool isBlocked, bool isReviewed)
        {
            this.CarRegistrationNumber = registration;
            this.ValidDate = validDate;
            this.IsBlocked = isBlocked;
            this.IsReviewed = isReviewed;
        }

        /// <summary>
        /// checks this document has expired or not
        /// </summary>
        /// <param name="expenseItemDate">Date of expense item</param>
        /// <returns></returns>
        public DocumentExpiryResult HasExpired(DateTime expenseItemDate)
        {
            var expiryInformation = new DocumentExpiryResult();
            if (this.IsBlocked)
            {
                var zeroDate = new DateTime(1900, 1, 1);
                if (this.ValidDate == null || this.ValidDate.Equals(zeroDate) || this.ValidDate.Equals(DateTime.MinValue))
                {
                    expiryInformation.HasExpired = true;
                    expiryInformation.Reason = string.Format("Your vehicle with registration {0} does not have a breakdown cover record attached.", this.CarRegistrationNumber);
                    
                    return expiryInformation;
                }

                if (this.ValidDate <= expenseItemDate.AddDays(-1))
                {
                    expiryInformation.HasExpired = true;
                    expiryInformation.Reason =
                        $"Your vehicle with registration {this.CarRegistrationNumber} has an expired breakdown cover certificate. " +
                        $"This expired on {this.ValidDate:d}.";
                    return expiryInformation;
                }

                if (this.ValidDate >= expenseItemDate && this.IsReviewed == false)
                {
                    expiryInformation.HasExpired = true;
                    expiryInformation.Reason = string.Format("The breakdown cover record for vehicle with registration {0} is awaiting approval.", this.CarRegistrationNumber);
                    return expiryInformation;
                }

            }
            expiryInformation.HasExpired = false;
            return expiryInformation;
        }

       
    }
}
