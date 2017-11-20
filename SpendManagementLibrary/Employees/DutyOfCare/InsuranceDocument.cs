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
    public class InsuranceDocument : Interfaces.IDutyOfCareDocument
    {
        /// <summary>
        /// Policy Number
        /// </summary>
        public string PolicyNumber { get; set; }

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
        /// initialises the insurance document
        /// </summary>
        /// <param name="registration">car registration number</param>
        /// <param name="validDate">valid date of the document</param>
        /// <param name="isBlocked">is there a check need on the document validity</param>
        /// <param name="isReviewed">Check to see if the document is reviewed or not</param>
        public InsuranceDocument(string registration, DateTime validDate, bool isBlocked, bool isReviewed)
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
        /// <param name="expenseItemDate">Date of expense item</param>
        /// <returns></returns>
        public DocumentExpiryResult HasExpired(DateTime expenseItemDate)
        {
            var expiryInformation = new DocumentExpiryResult();
            if (IsBlocked)
            {
                var zeroDate = new DateTime(1900, 1, 1);
                if (this.ValidDate == null || this.ValidDate.Equals(zeroDate) || this.ValidDate.Equals(DateTime.MinValue))
                {
                    expiryInformation.HasExpired = true;
                    expiryInformation.Reason = string.Format("Your vehicle with registration {0} does not have an insurance record attached.", CarRegistrationNumber);
                    
                    return expiryInformation;
                }

                if (this.ValidDate <= expenseItemDate.AddDays(-1))
                {
                    expiryInformation.HasExpired = true;
                    expiryInformation.Reason =
                        $"Your vehicle with registration {this.CarRegistrationNumber} has an expired insurance certificate. " +
                        $"This expired on {this.ValidDate:d}.";
                    return expiryInformation;
                }               

                if (this.ValidDate >= expenseItemDate && IsReviewed == false)
                {
                    expiryInformation.HasExpired = true;
                    expiryInformation.Reason = string.Format("The insurance document for vehicle with registration {0} is awaiting approval.", CarRegistrationNumber);
                    return expiryInformation;
                }

            }
            expiryInformation.HasExpired = false;
            return expiryInformation;
        }

       
    }
}
