using System.Collections.Generic;

namespace SpendManagementLibrary.Expedite
{
    using System;
    using Enumerators.Expedite;
    
    /// <summary>
    /// Represents an uploaded file that can be linked to a Claim Line (savedexpense).
    /// A recieipt can be attached to multiple claim lines, or the header of a claim,
    /// or if these cannot be determined, then a user or even just the account.
    /// </summary>
    public class Receipt : OrphanedReceipt
    {
        public Receipt()
        {
            OwnershipInfo = new ReceiptOwnershipInfo
            {
                EmployeeId = null,
                ClaimId = null,
                ClaimLines = new List<int>()
            };
        }

        /// <summary>
        /// The user who created the receipt.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// The last time this receipt was modified.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// The last user to modify this receipt.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Information related to Expedite.
        /// </summary>
        public ReceiptOwnershipInfo OwnershipInfo { get; set; }

        /// <summary>
        /// Contains information about where this receipt is assigned.
        /// </summary>
        public ExpediteInfo ExpediteInfo { get; set; }
    }

    public class OrphanedReceipt
    {
        /// <summary>
        /// The unique Id of this receipt.
        /// </summary>
        public int ReceiptId { get; set; }

        /// <summary>
        /// The file extension of this receipt.
        /// </summary>
        public string Extension { get; set; }
        
        /// <summary>
        /// The method by which this receipt was created.
        /// </summary>
        public ReceiptCreationMethod CreationMethod { get; set; }

        /// <summary>
        /// The url of this receipt, if it has been downloaded from the cloud for serving to a client.
        /// </summary>
        public string TemporaryUrl { get; set; }

        /// <summary>
        /// The url of the icon for this receipt, if it isn't a web-displayable format.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// The time the receipt was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }

}
