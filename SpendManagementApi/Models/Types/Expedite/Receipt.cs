namespace SpendManagementApi.Models.Types.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Utilities;
    using SpendManagementLibrary.Expedite;

    /// <summary>
    /// Represents an uploaded file that can be linked to a Claim Line (savedexpense).
    /// A recieipt can be attached to multiple claim lines, or the header of a claim,
    /// or if these cannot be determined, then a user or even just the account.
    /// </summary>
    public class Receipt : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.Receipt, Receipt>
    {
        /// <summary>
        /// The unique Id of this receipt.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The file extension of this receipt.
        /// </summary>
        [Required(ErrorMessage = ApiResources.ApiErrorExtensionAndDataRequired)]
        public string Extension { get; set; }

        /// <summary>
        /// The method by which this receipt was created.
        /// </summary>
        public ReceiptCreationMethod CreationMethod { get; set; }

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
        /// The Id of the Employee. Having this property set means 
        /// the receipt is attached to the employee.
        /// </summary>
        public new int? EmployeeId { get; set; }

        /// <summary>
        /// If this receipt was modified by an expedite user, 
        /// this contains the name of the staff member.
        /// </summary>
        public string ExpediteUserName { get; set; }

        /// <summary>
        /// The database Id of the Envelope in which this 
        /// receipt arrived, if it was sent to Expedite.
        /// </summary>
        public int EnvelopeId { get; set; }

        /// <summary>
        /// The actual data - to be populated when POSTING a receipt.
        /// </summary>
        [Required(ErrorMessage = ApiResources.ApiErrorExtensionAndDataRequired)]
        public string Data { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the >data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public Receipt From(SpendManagementLibrary.Expedite.Receipt dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.ReceiptId;
            Extension = dbType.Extension;
            CreationMethod = (ReceiptCreationMethod)dbType.CreationMethod;
            ClaimLines = dbType.OwnershipInfo.ClaimLines ?? new List<int>();
            ClaimId = dbType.OwnershipInfo.ClaimId;
            EmployeeId = dbType.OwnershipInfo.EmployeeId;
            CreatedOn = dbType.CreatedOn;
            CreatedById = dbType.CreatedBy;
            ModifiedOn = dbType.ModifiedOn;
            ModifiedById = dbType.ModifiedBy;

            if (dbType.ExpediteInfo != null)
            {
                ExpediteUserName = dbType.ExpediteInfo.ExpediteUserName;
                EnvelopeId = dbType.ExpediteInfo.EnvelopeId;
            }

            return this;
        }

        /// <summary>
        /// Converts to a API type from a base class of Receipt - OrphanedReceipt, which only has a few properties.
        /// </summary>
        /// <param name="dbType">An OrphanedReceipt.</param>
        /// <param name="actionContext">The actionContext which contains the DAL classes.</param>
        /// <returns></returns>
        public Receipt From(OrphanedReceipt dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.ReceiptId;
            Extension = dbType.Extension;
            CreationMethod = (ReceiptCreationMethod)dbType.CreationMethod;
            CreatedOn = dbType.CreatedOn;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.Receipt To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.Expedite.Receipt
            {
                ReceiptId = Id,
                CreationMethod = (SpendManagementLibrary.Enumerators.Expedite.ReceiptCreationMethod)CreationMethod,
                Extension = Extension,
                ModifiedBy = ModifiedById ?? 0,
                ModifiedOn = ModifiedOn ?? DateTime.UtcNow,
                OwnershipInfo = new ReceiptOwnershipInfo
                {
                    ClaimLines = ClaimLines,
                    ClaimId = ClaimId,
                    EmployeeId = EmployeeId
                },
                ExpediteInfo = new ExpediteInfo
                {
                    EnvelopeId = EnvelopeId,
                    ExpediteUserName = ExpediteUserName
                }
            };
        }
    }
}
