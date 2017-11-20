namespace SpendManagementApi.Models.Types
{
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// Represents a reason by which a user can claim expenses back.
    /// </summary>
    public class ClaimReason : BaseExternalType, IApiFrontForDbObject<cReason, ClaimReason>
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
        public ClaimReason From(cReason dbType, IActionContext actionContext)
        {
            Id = dbType.reasonid;
            Label = dbType.reason;
            Description = dbType.description;
            AccountCodeVat = dbType.accountcodevat ?? "";
            AccountCodeNoVat = dbType.accountcodenovat ?? "";

            AccountId = dbType.accountid;
            CreatedById = dbType.createdby;
            CreatedOn = dbType.createdon;
            ModifiedById = dbType.modifiedby ?? -1;
            ModifiedOn = dbType.modifiedon;
            Archived = dbType.Archive;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public cReason To(IActionContext actionContext)
        {
            return new cReason(AccountId ?? -1, Id, Label, Description, AccountCodeVat ?? "", AccountCodeNoVat ?? "", CreatedOn, CreatedById, ModifiedOn, ModifiedById, Archived);
        }
    }
}