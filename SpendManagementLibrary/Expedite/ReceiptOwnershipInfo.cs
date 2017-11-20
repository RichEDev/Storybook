namespace SpendManagementLibrary.Expedite
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the relationship between a receipt and owners.
    /// A receipt can be set to 0-many claim lines, or to a claim header,
    /// or to a user, or even to SEL, but never a combination of those.
    /// </summary>
    public class ReceiptOwnershipInfo
    {
        /// <summary>
        /// The list of related Ids for claim lines (or 'savedexpenses').
        /// </summary>
        public List<int> ClaimLines { get; set; }

        /// <summary>
        /// The Id of the claim. Having this property set means 
        /// the receipt is attached to the claim header.
        /// </summary>
        public int? ClaimId { get; set; }

        /// <summary>
        /// The Id of the employee. Having this property set means 
        /// the receipt is attached to the employee.
        /// </summary>
        public int? EmployeeId { get; set; }
    }
}
