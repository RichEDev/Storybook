namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BusinessLogic.Reasons;

    using Interfaces;
    using Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// Represents a reason by which a user can claim expenses back.
    /// </summary>
    public class ClaimReason : BaseExternalType, IApiFrontForDbObject<IReason, ClaimReason>
    {
        /// <summary>
        /// The unique Id for this ClaimReason object.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The name / label for this ClaimReason object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this ClaimReason object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// The Account code for VAT.
        /// </summary>
        [MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string AccountCodeVat { get; set; }
        
        /// <summary>
        /// The non-VAT account code.
        /// </summary>
        [MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string AccountCodeNoVat { get; set; }

        /// <summary>
        /// Archived value of a reason.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType">The DAL type.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>This, the API type.</returns>
        public ClaimReason From(IReason dbType, IActionContext actionContext)
        {
            Id = dbType.Id;
            Label = dbType.Name;
            Description = dbType.Description;
            AccountCodeVat = dbType.AccountCodeVat ?? string.Empty;
            AccountCodeNoVat = dbType.AccountCodeNoVat ?? string.Empty;

            AccountId = actionContext.AccountId;
            CreatedById = dbType.CreatedBy ?? 0;
            CreatedOn = dbType.CreatedOn ?? new DateTime(1900, 01, 01);
            ModifiedById = dbType.ModifiedBy ?? -1;
            ModifiedOn = dbType.ModifiedOn;
            Archived = dbType.Archived;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public IReason To(IActionContext actionContext)
        {
            return new Reason(this.Id, this.Archived, this.Description, this.Label, this.AccountCodeVat, this.AccountCodeNoVat ?? string.Empty, this.CreatedById, this.CreatedOn, this.ModifiedById, this.ModifiedOn);
        }
    }
}