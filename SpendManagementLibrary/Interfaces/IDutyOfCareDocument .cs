using SpendManagementLibrary.Employees.DutyOfCare;
using SpendManagementLibrary.Enumerators;
using System;

namespace SpendManagementLibrary.Interfaces
{
    /// <summary>
    /// every document that belongs to duty of care should inherit this.
    /// </summary>
    public interface IDutyOfCareDocument
    {
        /// <summary>
        /// Car registration number if the document is associated with car
        /// </summary>
        string CarRegistrationNumber { get; set; }
        /// <summary>
        /// validDate of the document
        /// </summary>
        DateTime? ValidDate { get; set; }
        /// <summary>
        /// if the check needs to be done on the document's validity
        /// </summary>
        bool IsBlocked { get; }

        /// <summary>
        /// checks whether the duty of care document has expired
        /// </summary>
        /// <param name="expenseItemDate">Date fo expense item</param>
        /// <returns>returns expired/not flag</returns>
        DocumentExpiryResult HasExpired(DateTime expenseItemDate);

    }
}
