namespace SpendManagementLibrary
{
    using System;

    public class ClaimBasic
    {
        /// <summary>
        /// The employee id number.
        /// </summary>
        public int EmployeeId { get; set; } 

        /// <summary>
        /// The claim id number.
        /// </summary>
        public int ClaimId { get; set; } 

        /// <summary>
        /// The claim number.
        /// </summary>
        public int ClaimNumber { get; set; } 

        /// <summary>
        /// The given claim name.
        /// </summary>
        public string ClaimName { get; set; }

        /// <summary>
        /// The claim description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The employee name
        /// </summary>
        public string EmployeeName { get; set; } 

        /// <summary>
        /// The base currency of the claim.
        /// </summary>
        public int BaseCurrency { get; set; } 

        /// <summary>
        /// The current stage of the claim in the sign off group
        /// </summary>
        public int Stage { get; set; } 

        /// <summary>
        /// The current status of the claim.
        /// </summary>
        public SpendManagementLibrary.ClaimStatus Status { get; set; } 

        /// <summary>
        /// Specified if the claim is currently approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Shows whether the claim has been submitted or not.
        /// </summary>
        public bool Submitted { get; set; }

        /// <summary>
        /// The id number of the claim checker
        /// </summary>
        public int? CheckerId { get; set; }

        /// <summary>
        /// Gets or set the ItemCheckerId for the claim.
        /// </summary>
        public int? ItemCheckerId { get; set; }

        /// <summary>
        /// Whether to show the declaration at the current claim stage.
        /// </summary>
        public bool DisplayDeclaration { get; set; }

        /// <summary>
        /// The reference number for the claim.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The currency symbol use for the base currency of the claim.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets the monetary amount the claimant will receive back
        /// </summary>
        public decimal AmountPayable { get; set; }

        /// <summary>
        /// Whether one click sign off is permitted
        /// </summary>
        public bool DisplayOneClickSignoff { get; set; }

        /// <summary>
        /// The number of expense items on the claim
        /// </summary>
        public int NumberOfItems { get; set; }

        /// <summary>
        /// The date the claim was submitted
        /// </summary>
        public DateTime? DateSubmitted { get; set; }

        /// <summary>
        /// The date the claim was Paid
        /// </summary>
        public DateTime? DatePaid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a claim can be unassigned from the current approver
        /// </summary>
        public bool CanBeUnassigned { get; set; }
    }
}
