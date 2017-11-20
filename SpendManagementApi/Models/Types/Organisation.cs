namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    using Interfaces;
    using Employees;
    using Utilities;

    using SMLA = SpendManagementLibrary.Addresses;
    using SpendManagementApi.Common.Enums;

    /// <summary>
    /// A Organisation is a unit of financial information against which you record expenditure.<br/>
    /// An <see cref="Employee">Employee</see> or an <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.
    /// </summary>
    public class Organisation : ArchivableBaseExternalType, IRequiresValidation, IApiFrontForDbObject<SMLA.Organisation, Organisation>
    {
        /// <summary>
        /// The Id of this object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name / label for this Organisation object.
        /// </summary>
        [Required, MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string Label { get; set; }

        /// <summary>
        /// The Parent Organisation ID
        /// </summary>
        [Range(1, int.MaxValue)]
        public int? ParentOrganisationId { get; set; }

        /// <summary>
        /// A comment of this Organisation object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Comment { get; set; }

        /// <summary>
        /// The Organisation code
        /// </summary>
        [MaxLength(60, ErrorMessage = ApiResources.ErrorMaxLength + @"60")]
        public string Code { get; set; }

        /// <summary>
        /// The primary address ID
        /// </summary>
        [Range(1, int.MaxValue)]
        public int? PrimaryAddressId { get; set; }

        /// <summary>
        /// The first line of the address
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The Postcode
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// The outcome of an action relating to Organisations
        /// </summary>
        public OrganisationActionOutcome OrganisationActionOutcome { get; set;}

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <returns>This, the API type.</returns>
        public Organisation From(SMLA.Organisation dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.Id = dbType.Identifier;
            this.Label = dbType.Name;
            this.ParentOrganisationId = dbType.ParentOrganisationIdentifier;
            this.Comment = dbType.Comment;
            this.Code = dbType.Code;
            this.Archived = dbType.IsArchived;
            this.PrimaryAddressId = (dbType.PrimaryAddress == null ? (int?)null : dbType.PrimaryAddress.Identifier);

            this.CreatedById = dbType.CreatedBy;
            this.CreatedOn = dbType.CreatedOn;
            this.ModifiedById = (dbType.ModifiedBy == SMLA.Organisation.IntNull ? null : (int?)dbType.ModifiedBy);
            this.ModifiedOn = (dbType.ModifiedOn == SMLA.Organisation.DateTimeNull ? null : (DateTime?)dbType.ModifiedOn);
            this.OrganisationActionOutcome = OrganisationActionOutcome.Success;

            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public SMLA.Organisation To(IActionContext actionContext)
        {
            SMLA.PrimaryAddress primaryAddress = null;
            if (this.PrimaryAddressId != null)
            {
                var response = new Controllers.V1.AddressesV1Controller().Get((int)this.PrimaryAddressId);
                if (response != null)
                {
                    var address = response.Item;
                    primaryAddress = new SMLA.PrimaryAddress
                    {
                        Identifier = address.Id,
                        FriendlyName = SpendManagementLibrary.Addresses.
                                        Address.GenerateFriendlyName(address.AddressName, address.Line1, address.Postcode, address.City)
                    };
                }
            }

            return new SMLA.Organisation
            {
                Identifier = this.Id,
                Name = this.Label,
                ParentOrganisationIdentifier = this.ParentOrganisationId,
                Comment = this.Comment,
                Code = this.Code,
                IsArchived = this.Archived,
                PrimaryAddress = primaryAddress,

                CreatedBy = this.CreatedById,
                CreatedOn = this.CreatedOn,
                ModifiedBy = (this.ModifiedById == null ? SMLA.Organisation.IntNull : (int)this.ModifiedById),
                ModifiedOn = (this.ModifiedOn == null ? SMLA.Organisation.DateTimeNull : (DateTime)this.ModifiedOn)
            };
        }

        public void Validate(IActionContext actionContext)
        {
            if (this.ParentOrganisationId != null && SMLA.Organisations.Get(actionContext.AccountId, (int)this.ParentOrganisationId) == null)
            {
                throw new InvalidDataException(String.Format(ApiResources.ApiErrorANonExistentX, "Parent Organisation") + this.ParentOrganisationId);
            }

            if (this.PrimaryAddressId != null && actionContext.Addresses.GetAddressById((int)this.PrimaryAddressId) == null)
            {
                throw new InvalidDataException(String.Format(ApiResources.ApiErrorAnNonExistentX, "Address") + this.PrimaryAddressId);
            }
        }
    }
}