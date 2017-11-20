﻿namespace SpendManagementApi.Models.Types.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Attributes.Validation;
    using Interfaces;
    using Utilities;
    
    /// <summary>
    /// Represents a physical envelope that may be sent 
    /// to a claimant and returned with receipts inside.
    /// </summary>
    public class Envelope : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.Envelope, Envelope>
    {
        /// <summary>
        /// The unqiue primary key of this envelope.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The AccountId of the account that 
        /// this envelope has been assigned to.
        /// </summary>
        public new int? AccountId { get; set; }

        /// <summary>
        /// The ClaimId of the claim that this 
        /// envelope contains receipts for.
        /// </summary>
        public int? ClaimId { get; set; }

        /// <summary>
        /// The custom envelope number that is used by the claimant
        /// to generate a claim reference number (CRN).
        /// </summary>
        [MaxLength(10, ErrorMessage = ApiResources.ErrorMaxLength + @"10")]
        public string EnvelopeNumber { get; set; }

        /// <summary>
        /// The claim reference number generated by the software 
        /// for the return of receipts in a single envelope.
        /// </summary>
        [MaxLength(12, ErrorMessage = ApiResources.ErrorMaxLength + @"12")]
        public string ClaimReferenceNumber { get; set; }

        /// <summary>
        /// The <see cref="EnvelopeStatus">status</see> of the envelope.
        /// </summary>
        [ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public EnvelopeStatus Status { get; set; }

        /// <summary>
        /// The type of envelope (from the EnvelopeTypes table).
        /// </summary>
        public int? EnvelopeType { get; set; }

        /// <summary>
        /// The date the envelope has been sent to the client.
        /// </summary>
        public DateTime? DateIssuedToClaimant { get; set; }

        /// <summary>
        /// The date the envelope has been attached to a claim.
        /// </summary>
        public DateTime? DateAssignedToClaim { get; set; }

        /// <summary>
        /// The date the envelope has been 
        /// returned to SEL by the client.
        /// </summary>
        public DateTime? DateReceived { get; set; }

        /// <summary>
        /// The date SEL have completed attaching the 
        /// contents to claims.
        /// </summary>
        public DateTime? DateAttachCompleted { get; set; }

        /// <summary>
        /// The date the envelope has or will be destroyed.
        /// </summary>
        public DateTime? DateDestroyed { get; set; }

        /// <summary>
        /// Whether the claimant has declared that the envelope is lost in the post,
        /// in case we later receive it and scan it and the client wonders why any validation fails.
        /// </summary>
        public bool DeclaredLostInPost { get; set; }
        
        /// <summary>
        /// Any overpayment charge applied to the envelope.
        /// </summary>
        public decimal? OverpaymentCharge { get; set; }

        /// <summary>
        /// The physical state of the envelope. 
        /// </summary>
        public List<int> PhysicalState { get; set; }

        /// <summary>
        /// A url for an image showing the physical state of the envelope.
        /// </summary>
        [MaxLength(100, ErrorMessage = ApiResources.ErrorMaxLength + @"100")]
        public string PhysicalStateProofUrl { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The action context</param>
        /// <returns>An api Type</returns>
        public Envelope From(SpendManagementLibrary.Expedite.Envelope dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            return new Envelope
            {
                Id = dbType.EnvelopeId,
                AccountId = dbType.AccountId,
                ClaimId = dbType.ClaimId,
                EnvelopeNumber = dbType.EnvelopeNumber,
                ClaimReferenceNumber = dbType.ClaimReferenceNumber,
                Status = (EnvelopeStatus) dbType.Status,
                EnvelopeType = dbType.Type.EnvelopeTypeId,
                DateIssuedToClaimant = dbType.DateIssuedToClaimant,
                DateAssignedToClaim = dbType.DateAssignedToClaim,
                DateReceived = dbType.DateReceived,
                DateAttachCompleted = dbType.DateAttachCompleted,
                DateDestroyed = dbType.DateDestroyed,
                OverpaymentCharge = dbType.OverpaymentCharge,
                PhysicalState = dbType.PhysicalState.Select(x => x.EnvelopePhysicalStateId).ToList(),
                PhysicalStateProofUrl = dbType.PhysicalStateProofUrl,
                ModifiedById = dbType.LastModifiedBy,
                DeclaredLostInPost = dbType.DeclaredLostInPost
            };
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.Envelope To(IActionContext actionContext)
        {
            var item = new SpendManagementLibrary.Expedite.Envelope
            {
                EnvelopeId = Id,
                AccountId = AccountId,
                ClaimId = ClaimId,
                EnvelopeNumber = EnvelopeNumber,
                ClaimReferenceNumber = ClaimReferenceNumber,
                Status = (SpendManagementLibrary.Enumerators.Expedite.EnvelopeStatus) Status,
                DateIssuedToClaimant = DateIssuedToClaimant,
                DateAssignedToClaim = DateAssignedToClaim,
                DateReceived = DateReceived,
                DateAttachCompleted = DateAttachCompleted,
                DateDestroyed = DateDestroyed,
                OverpaymentCharge = OverpaymentCharge,
                PhysicalState = PhysicalState.Select(x => new SpendManagementLibrary.Expedite.EnvelopePhysicalState { EnvelopePhysicalStateId = x }).ToList(),
                PhysicalStateProofUrl = PhysicalStateProofUrl,
                LastModifiedBy = actionContext.EmployeeId,
                DeclaredLostInPost = DeclaredLostInPost
            };

            if (EnvelopeType.HasValue)
            {
                item.Type = new SpendManagementLibrary.Expedite.EnvelopeType { EnvelopeTypeId = EnvelopeType.Value };
            }

            return item;
        }
    }
}
