
namespace SpendManagementApi.Models.Types
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Attributes.Validation;
    using Interfaces;
    using Utilities;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using Spend_Management;

    using ClaimStage = SpendManagementApi.Common.Enums.ClaimStage;
    using ClaimStatus = SpendManagementApi.Common.Enums.ClaimStatus;
    using SpendManagementApi.Common.Enums;
    using System;

    /// <summary>
    /// A simple version of the Claim class
    /// </summary>
    public class ClaimBasic : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.ClaimBasic, ClaimBasic>
    {
        /// <summary>
        /// The employee id number.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public new int EmployeeId { get; set; }

        /// <summary>
        /// The claim id number.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int ClaimId { get; set; }

        /// <summary>
        /// The claim number.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int ClaimNumber { get; set; }

        /// <summary>
        /// The given claim name.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string ClaimName { get; set; }

        /// <summary>
        /// The claim description
        /// </summary>
        [Required, MaxLength(2000, ErrorMessage = ApiResources.ErrorMaxLength + @"2000")]
        public string Description { get; set; }
        
        /// <summary>
        /// The employee name
        /// </summary>
        [MaxLength(350, ErrorMessage = ApiResources.ErrorMaxLength + @"350")]
        public string EmployeeName { get; set; }

        /// <summary>
        /// The base currency of the claim.
        /// </summary>
        [DataMember]
        [Range(1, int.MaxValue)]
        public int BaseCurrency { get; set; }

        /// <summary>
        /// The current stage of the claim in the sign off group
        /// </summary>
        [Required]
        public int Stage { get; set; }

        /// <summary>
        /// Gets the total number of stages the claim has to go through to be authorised 
        /// </summary>
        public int TotalStageCount { get; set; }

        /// <summary>
        /// Gets or sets the stage.
        /// The SignoffGroup stage that this claim is currently at. 
        /// </summary>
        public ClaimStage ClaimStage { get; set; }

        /// <summary>
        /// The current status of the claim.
        /// </summary>
        [Required]
        [ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public ClaimStatus Status { get; set; }

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
        [MaxLength(11, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The currency symbol use for the base currency of the claim.
        /// </summary>
        [MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// The currency label for the base currency of the claim.
        /// </summary>
        public string CurrencyLabel { get; set; }

        /// <summary>
        /// Gets the monetary amount the claimant will receive back
        /// </summary>
        [Range(typeof(decimal), "-10,000,000,000.00", "10,000,000,000.00")]
        public decimal AmountPayable { get; set; }

        /// <summary>
        /// Gets the total amount the claimant will receive back
        /// </summary>
        [Range(typeof(decimal), "-10,000,000,000.00", "10,000,000,000.00")]
        public decimal Total { get; set; }

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
        /// Gets or sets the date paid.
        /// </summary>
        public DateTime? DatePaid { get; set; }

        /// <summary>
        /// The outcome of the claimbasic action
        /// </summary>
        public ClaimBasicOutcome ClaimBasicOutcome { get; set; }

        /// <summary>
        /// Gets or sets the list of envelope information.
        /// </summary>
        public List<ClaimEnvelopeInfo> ClaimEnvelopeInfo { get; set; }

        /// <summary>
        /// Gets the approver currently responsible for authorising the claim 
        /// </summary>
        public string CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the claimant name
        /// </summary>
        public string Claimant { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a claim can be unassigned from the current approver
        /// </summary>
        public bool CanBeUnassigned { get; set; }

        /// <summary>
        /// Creates an API type from the SpendManagementLibrary type.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public ClaimBasic From(SpendManagementLibrary.ClaimBasic dbType, IActionContext actionContext)
        {
            this.Approved = dbType.Approved;
            this.BaseCurrency = dbType.BaseCurrency;
            this.CheckerId = dbType.CheckerId;
            this.ItemCheckerId = dbType.ItemCheckerId;
            this.ClaimId = dbType.ClaimId;
            this.ClaimName = dbType.ClaimName;
            this.ClaimNumber = dbType.ClaimNumber;
            this.Description = dbType.Description;
            this.CurrencySymbol = dbType.CurrencySymbol;
            this.DisplayDeclaration = dbType.DisplayDeclaration;
            this.EmployeeId = dbType.EmployeeId;
            this.EmployeeName = dbType.EmployeeName;
            this.ReferenceNumber = dbType.ReferenceNumber;
            this.Stage = dbType.Stage;
            this.Status = (ClaimStatus)dbType.Status;
            this.Submitted = dbType.Submitted;
            this.AmountPayable = dbType.AmountPayable;
            this.DisplayOneClickSignoff = dbType.DisplayOneClickSignoff;
            this.NumberOfItems = dbType.NumberOfItems;
            this.DateSubmitted = dbType.DateSubmitted;
            this.DatePaid = dbType.DatePaid;
            this.ClaimBasicOutcome = ClaimBasicOutcome.Success;
            this.CanBeUnassigned = dbType.CanBeUnassigned;

            var claimObject = new cClaims(actionContext.AccountId);
            var claim = claimObject.getClaimById(this.ClaimId);
            this.Total = claim.Total;
            this.AmountPayable = claim.AmountPayable;
            this.Stage = claim.stage;
            this.TotalStageCount = claim.TotalStageCount;
            this.ClaimStage = (ClaimStage)claim.ClaimStage;
            this.ReferenceNumber = claim.ReferenceNumber;
            this.ClaimEnvelopeInfo = claimObject.GetClaimEnvelopeNumbers(this.ClaimId);
            this.CurrentApprover = claim.CurrentApprover;   
            var claimant = actionContext.Employees.GetEmployeeById(claim.employeeid);
            if (claimant != null)
            {
                this.Claimant = $"{claimant.Surname}, {claimant.Title} {claimant.Forename}";
            }


            return this;
        }

        /// <summary>
        /// Returns a SpendManagementLibrary representation of the API object.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>A new <see cref="SpendManagementLibrary.ClaimBasic">SpendManagementLibrary ClaimBasic object</see></returns>
        public SpendManagementLibrary.ClaimBasic To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.ClaimBasic
                        {
                            Approved = this.Approved,
                            BaseCurrency = this.BaseCurrency,
                            CheckerId = this.CheckerId,
                            ItemCheckerId =  this.ItemCheckerId,
                            ClaimId = this.ClaimId,
                            ClaimName = this.ClaimName,
                            ClaimNumber = this.ClaimNumber,
                            Description = this.Description,
                            CurrencySymbol = this.CurrencySymbol,
                            DisplayDeclaration = this.DisplayDeclaration,
                            EmployeeId = this.EmployeeId,
                            EmployeeName = this.EmployeeName,
                            ReferenceNumber = this.ReferenceNumber,
                            Stage = this.Stage,
                            Status = (SpendManagementLibrary.ClaimStatus)this.Status,
                            Submitted = this.Submitted,
                            AmountPayable = this.AmountPayable,     
                            DisplayOneClickSignoff = this.DisplayOneClickSignoff,
                            NumberOfItems = this.NumberOfItems
                        };
        }    
    }
}