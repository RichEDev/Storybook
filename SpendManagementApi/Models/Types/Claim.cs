using System.Collections.Generic;
using System.Linq;
using SpendManagementApi.Models.Types.Expedite;

namespace SpendManagementApi.Models.Types
{
    using System;
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Attributes.Validation;

    using Utilities;
    using SpendManagementApi.Common.Enums;

    using SpendManagementLibrary.Expedite;

    using Spend_Management;

    /// <summary>
    /// Represents an expense claim in the system. This is the mechanism by which claimants
    /// account for the money they have spent and wish to claim back. A claim contains a 
    /// list of <see cref="ExpenseItem">ExpenseItems</see>.
    /// </summary>
    public class Claim : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.cClaim, Claim>
    {
        /// <summary>
        /// The unique Id of this claim in the database.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Id { get; set; }

        /// <summary>
        /// The Claim Number.
        /// </summary>
        [Required, Range(1, int.MaxValue)]
        public int ClaimNumber { get; set; }

        /// <summary>
        /// The Employee Id.
        /// </summary>
        [Required, Range(1, int.MaxValue)]
        public new int EmployeeId { get; set; }

        /// <summary>
        /// The Account Id.
        /// </summary>
        [Required, Range(1, int.MaxValue)]
        public new int AccountId { get; set; }

        /// <summary>
        /// The Name given to this claim.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Name { get; set; }

        /// <summary>
        /// The description given to this claim.
        /// </summary>
        [MaxLength(2000, ErrorMessage = ApiResources.ErrorMaxLength + @"2000")]
        public string Description { get; set; }

        /// <summary>
        /// The SignoffGroup stage that this claim is currently at.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Stage { get; set; }

        /// <summary>
        /// Whether this Claim has been approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// The date that this claim was submitted (if it has been submitted).
        /// </summary>
        public DateTime? DateSubmitted { get; set; }

        /// <summary>
        /// Whether the claim has been submitted or not.
        /// </summary>
        public bool Submitted { get; set; }
    
        /// <summary>
        /// The date that this claim was paid (if it has been paid).
        /// </summary>
        public DateTime? DatePaid { get; set; }

        /// <summary>
        /// The current status of this claim.
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public ClaimStatus Status { get; set; }

        /// <summary>
        /// The Id of the team relating to this claim.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int TeamId { get; set; }

        /// <summary>
        /// The Id of the checker, whomever it is.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int CheckerId { get; set; }

        /// <summary>
        /// Whether the approval stage should be split.
        /// </summary>
        public bool SplitApprovalStage { get; set; }

        /// <summary>
        /// The Id of the currency that this claim is in.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int CurrencyId { get; set; }

        /// <summary>
        /// The type of this claim.
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public ClaimType ClaimType
        {
            get
            {
                if (HasCashItems && (HasCreditCardItems || HasPurchaseCardItems))
                {
                    return ClaimType.Mixed;
                }
                if (HasCashItems && !HasCreditCardItems && !HasPurchaseCardItems)
                {
                    return ClaimType.Cash;
                }
                if (!HasCashItems && HasCreditCardItems && !HasPurchaseCardItems)
                {
                    return ClaimType.Credit;
                }
                return ClaimType.Purchase;
            }
        }

        /// <summary>
        /// Gets the reference number for the claim.
        /// </summary>
        [MaxLength(11, ErrorMessage = ApiResources.ErrorMaxLength + @"11")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets whether the claim is at current, submitted or previous
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public ClaimStage ClaimStage
        {
            get
            {
                if (!DateSubmitted.HasValue && !DatePaid.HasValue)
                {
                    return ClaimStage.Current;
                }
                if (DateSubmitted.HasValue && !DatePaid.HasValue)
                {
                    return ClaimStage.Submitted;
                }
                return ClaimStage.Previous;
            }
        }

        /// <summary>
        /// Gets whether there is any history associated with this claim
        /// </summary>
        public bool HasClaimHistory { get; set; }

        /// <summary>
        /// Gets the approver currently responsible for authorising the claim
        /// </summary>
        public string CurrentApprover { get; set; }

        /// <summary>
        /// Gets the total number of stages the claim has to go through to be authorised
        /// </summary>
        [Range(0, int.MaxValue)]
        public int TotalStageCount { get; set; }

        /// <summary>
        /// Gets whether the claim includes any expense items that have been returned for amendment
        /// </summary>
        public bool HasReturnedItems { get; set; }

        /// <summary>
        /// Gets whether there are any cash card items on the claim
        /// </summary>
        public bool HasCashItems { get; set; }
        /// <summary>
        /// Gets whether there are any credit card items on the claim
        /// </summary>
        public bool HasCreditCardItems { get; set; }

        /// <summary>
        /// Gets whether there are any purchase card items on the claim
        /// </summary>
        public bool HasPurchaseCardItems { get; set; }

        /// <summary>
        /// Gets whether there are any flagged items on the claim
        /// </summary>
        public bool HasFlaggedItems { get; set; }

        /// <summary>
        /// Gets the total number of items on the claim
        /// </summary>
        [Range(0, int.MaxValue)]
        public int NumberOfItems { get; set; }

        /// <summary>
        /// Gets the earliest item date on the claim
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets the latest item date on the claim
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets the total monetary value of the claim
        /// </summary>
        [Range(typeof(decimal), "-10,000,000,000.00", "10,000,000,000.00")]
        public decimal Total { get; set; }

        /// <summary>
        /// Gets the monetary amount the claimant will receive back
        /// </summary>
        [Range(typeof(decimal), "-10,000,000,000.00", "10,000,000,000.00")]
        public decimal AmountPayable { get; set; }

        /// <summary>
        /// Gets the total number of receipts on the claim
        /// </summary>
        [Range(0, int.MaxValue)]
        public int NumberOfReceipts { get; set; }

        /// <summary>
        /// Gets the number of items that have not yet been approved
        /// </summary>
        [Range(0, int.MaxValue)]
        public int NumberOfUnapprovedItems { get; set; }

        /// <summary>
        /// Gets the total monetary value of purchase card items
        /// </summary>
        [Range(typeof(decimal), "-10,000,000,000.00", "10,000,000,000.00")]
        public decimal PurchaseCardTotal { get; set; }

        /// <summary>
        /// Gets the total monetary value of credit card items
        /// </summary>
        [Range(typeof(decimal), "-10,000,000,000.00", "10,000,000,000.00")]
        public decimal CreditCardTotal { get; set; }

        /// <summary>
        /// Returns a list of all of the Ids of each expense item in this claim.
        /// </summary>
        public List<int> ExpenseItems { get; set; }

        /// <summary>
        /// Returns a list of the Ids of each Envelope attached to this claim.
        /// </summary>
        public List<int> Envelopes { get; set; }

        /// <summary>
        /// Returns a list of the Ids of each Envelope that has an imcomplete status.
        /// </summary>
        public List<int> IncompleteEnvelopes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pay before validate is active.
        /// </summary>
        public bool PayBeforeValidate { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public Claim From(SpendManagementLibrary.cClaim dbType, IActionContext actionContext)
        {
            var claim = this.ConvertClaimDBTypeToApiType(dbType);

            // pick out the expenseItems
            claim.ExpenseItems = actionContext.Claims.getExpenseItemsFromDB(claim.Id).Select(x => x.Key).ToList();

            if (string.IsNullOrEmpty(dbType.ReferenceNumber))
            {
                return claim;
            }

            // pick out the envelopes
            const int minimumCompleteStatus = (int)EnvelopeStatus.ReceiptsAttached;
            var allEnvelopesForClaim = actionContext.Envelopes.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);
            claim.Envelopes = allEnvelopesForClaim.Select(x => x.EnvelopeId).ToList();
            claim.IncompleteEnvelopes = allEnvelopesForClaim.Where(x => (int)x.Status < minimumCompleteStatus).Select(x => x.EnvelopeId).ToList();

            return claim;
        }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>       
        /// <returns>An instance of <see cref="Claim"/>Claim</returns>
        public Claim From(SpendManagementLibrary.cClaim dbType, cClaims claims, Envelopes envelopes)
        {
            var claim = this.ConvertClaimDBTypeToApiType(dbType);

            // pick out the expenseItems
            claim.ExpenseItems = claims.getExpenseItemsFromDB(claim.Id).Select(x => x.Key).ToList();

            if (string.IsNullOrEmpty(dbType.ReferenceNumber))
            {
                return claim;
            }

            // pick out the envelopes
            const int minimumCompleteStatus = (int)EnvelopeStatus.ReceiptsAttached;
            var allEnvelopesForClaim = envelopes.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);
            claim.Envelopes = allEnvelopesForClaim.Select(x => x.EnvelopeId).ToList();
            claim.IncompleteEnvelopes = allEnvelopesForClaim.Where(x => (int)x.Status < minimumCompleteStatus).Select(x => x.EnvelopeId).ToList();

            return claim;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.cClaim To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.cClaim(AccountId, Id, ClaimNumber, EmployeeId, Name, Description, Stage,
                Approved, DatePaid.HasValue, DateSubmitted ?? DateTime.MinValue, DatePaid ?? DateTime.MinValue,
                (SpendManagementLibrary.ClaimStatus)Status, TeamId, CheckerId, DateSubmitted.HasValue,
                SplitApprovalStage, CreatedOn, CreatedById, ModifiedOn ?? DateTime.UtcNow, ModifiedById ?? 0,
                CurrencyId, ReferenceNumber, HasClaimHistory, CurrentApprover, TotalStageCount, HasReturnedItems,
                HasCashItems, HasCreditCardItems, HasPurchaseCardItems, HasFlaggedItems, NumberOfItems, StartDate,
                EndDate, Total, AmountPayable, NumberOfReceipts, NumberOfUnapprovedItems, CreditCardTotal, PurchaseCardTotal, PayBeforeValidate);
        }

        /// <summary>
        /// Converts properties of a data access layer type claim to an api type claim.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <returns>A claim</returns>
        private Claim ConvertClaimDBTypeToApiType(SpendManagementLibrary.cClaim dbType)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.claimid;
            ClaimNumber = dbType.claimno;
            EmployeeId = dbType.employeeid;
            AccountId = dbType.accountid;
            Name = dbType.name;
            Description = dbType.description;
            Stage = dbType.stage;
            Approved = dbType.approved;
            DateSubmitted = dbType.submitted ? dbType.datesubmitted : (DateTime?)null;
            Submitted = dbType.submitted;
            DatePaid = dbType.paid ? dbType.datepaid : (DateTime?)null;
            Status = (ClaimStatus)dbType.status;
            TeamId = dbType.teamid;
            CheckerId = dbType.checkerid;
            SplitApprovalStage = dbType.splitApprovalStage;
            CurrencyId = dbType.currencyid;
            ReferenceNumber = dbType.ReferenceNumber;
            HasClaimHistory = dbType.HasClaimHistory;
            CurrentApprover = dbType.CurrentApprover;
            TotalStageCount = dbType.TotalStageCount;
            HasReturnedItems = dbType.HasReturnedItems;
            HasCashItems = dbType.HasCashItems;
            HasCreditCardItems = dbType.HasCreditCardItems;
            HasPurchaseCardItems = dbType.HasPurchaseCardItems;
            HasFlaggedItems = dbType.HasFlaggedItems;
            NumberOfItems = dbType.NumberOfItems;
            StartDate = dbType.StartDate;
            EndDate = dbType.EndDate;
            Total = Convert.ToDecimal(dbType.Total.ToString("0.00"));
            AmountPayable = Convert.ToDecimal(dbType.AmountPayable.ToString("0.00"));
            NumberOfReceipts = dbType.NumberOfReceipts;
            NumberOfUnapprovedItems = dbType.NumberOfUnapprovedItems;
            PurchaseCardTotal = Convert.ToDecimal(dbType.PurchaseCardTotal.ToString("0.00"));
            CreditCardTotal = Convert.ToDecimal(dbType.CreditCardTotal.ToString("0.00"));
            CreatedOn = dbType.createdon;
            CreatedById = dbType.createdby;
            ModifiedOn = dbType.modifiedon;
            ModifiedById = dbType.modifiedby;
            this.PayBeforeValidate = dbType.PayBeforeValidate;

            return this;
        }

        private List<int> GetExpenseItems(int claimId, IActionContext actionContext = null, cClaims claims = null)
        {
            if (actionContext != null)
            {
                return actionContext.Claims.getExpenseItemsFromDB(claimId).Select(x => x.Key).ToList();
            }
            
            return claims?.getExpenseItemsFromDB(claimId).Select(x => x.Key).ToList();
        }
    }
}