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
    public class TaxDocument : IDutyOfCareDocument
    {
        /// <summary>
        /// Expiry Date
        /// </summary>
        public DateTime? ValidDate { get; set; }

        /// <summary>
        /// Review Date
        /// </summary>
        public DateTime? ReviewDate { get; set; }
        /// <summary>
        /// Reviewed By
        /// </summary>
        public int ReviewedBy { get; set; }
        /// <summary>
        /// Car Registration Number
        /// </summary>
        public string CarRegistrationNumber { get; set; }
       
        /// <summary>
        /// is the block needed on the document
        /// </summary>
        public bool IsBlocked { get; private set; }

        /// <summary>
        /// Check to see if the document is reviewed or not
        /// </summary>
        public bool IsReviewed { get; private set; }


        /// <summary>
        /// initialises the PhotoCardDrivingLicence document
        /// </summary>
        /// <param name="registration">car registration number</param>
        /// <param name="validDate">valid date of the document</param>
        /// <param name="isBlocked">is there a check need on the document validity</param>
        /// <param name="isReviewed">Check to see if the document is reviewed or not</param>
        public TaxDocument(string registration, DateTime validDate, bool isBlocked, bool isReviewed)
        {
            CarRegistrationNumber = registration;
            this.ValidDate = validDate;
            IsBlocked = isBlocked;
            IsReviewed = isReviewed;
        }

        /// <summary>
        /// checks this document has expired or not
        /// </summary>
        /// <param name="reasonText">reasons text</param>
        /// <param name="expenseItemDate">Date fo expense item</param>
        /// <returns></returns>
        public DocumentExpiryResult HasExpired(DateTime expenseItemDate)
        {
            var expiryResult = new DocumentExpiryResult();
            if (IsBlocked)
            {
                var zeroDate = new DateTime(1900, 1, 1);
                if (this.ValidDate == null || this.ValidDate.Equals(zeroDate) || this.ValidDate.Equals(DateTime.MinValue))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = string.Format("Your vehicle with registration {0} does not have a tax record attached.", CarRegistrationNumber);
                    return expiryResult;

                }

                if (this.ValidDate <= expenseItemDate.AddDays(-1))
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason =
                        $"Your vehicle with registration {this.CarRegistrationNumber} has an expired tax document. " +
                        $"This expired on {this.ValidDate:d}.";
                    return expiryResult;
                }
             
                //IsReviewed == false for Review = failed/invalidated/not reviewed
                if (this.ValidDate >= expenseItemDate && IsReviewed == false)
                {
                    expiryResult.HasExpired = true;
                    expiryResult.Reason = string.Format("The tax record for vehicle with registration {0} is awaiting approval.", CarRegistrationNumber);
                    return expiryResult;
                }               
            }
            expiryResult.HasExpired = false;
            return expiryResult;
        }
    }
}
