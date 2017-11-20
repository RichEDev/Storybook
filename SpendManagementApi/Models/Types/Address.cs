namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SpendManagementLibrary;

    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Employees;

    using Utilities;

    using SpendManagementLibrary.Addresses;

    using Interfaces;

    using SpendManagementApi.Common.Enum;

    /// <summary>
    /// A Global Address represents a location on the planet.
    /// </summary>
    public class Address : ArchivableBaseExternalType,
                           IApiFrontForDbObject<SpendManagementLibrary.Addresses.Address, Address>
    {
        /// <summary>
        /// The address location id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The identifier from Postcode Anywhere
        /// </summary>
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// Denotes where the address came from
        /// </summary>
        public AddressSource AddressSource { get; set; }

        /// <summary>
        /// The name for the address, usually a company name when supplied.
        /// </summary>
        [MaxLength(250, ErrorMessage = ApiResources.ErrorMaxLength + @"250")]
        public string AddressName { get; set; }

        /// <summary>
        /// Gets or sets the friendly name or the address.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The first line of the address, usually a number and street.
        /// </summary>
        [Required, MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string Line1 { get; set; }

        /// <summary>
        /// The second line of the address.
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string Line2 { get; set; }

        /// <summary>
        /// The second line of the address.
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string Line3 { get; set; }

        /// <summary>
        /// The city.
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string City { get; set; }

        /// <summary>
        /// The county this address is in.
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string County { get; set; }

        /// <summary>
        /// The country (global country id) the address is in.
        /// </summary>
        public int Country { get; set; }

        /// <summary>
        /// The address's postal code.
        /// </summary>
        [MaxLength(32, ErrorMessage = ApiResources.ErrorMaxLength + @"32")]
        public string Postcode { get; set; }

        /// <summary>
        /// A list of the account-wide labels for this Address.
        /// Editing this list will have no effect. Use the PATCH resources.
        /// </summary>
        public List<int> AccountWideLabels { get; set; }

        /// <summary>
        /// Is the address favourited for the entire account.
        /// Use the PATCH resource to set this.
        /// </summary>
        public bool IsAccountWideFavourite { get; set; }

        /// <summary>
        /// Gets or sets whether the address has a favouriteId
        /// </summary>
        public int FavouriteId { get; set; }

        /// <summary>
        /// The primary Label out of the account wide labels, if there are any.
        /// You can set the primary account wide label by using the PATCH resource.
        /// </summary>
        public int? PrimaryAccountWideLabel { get; set; }

        /// <summary>
        /// A list of the manual recommended distances between between this Address and other Addresses.
        /// Editing this list will have no effect. Use the <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> resource.
        /// </summary>
        public List<int> RecommendedDistances { get; set; }


        /// <summary>
        /// Gets or sets whether the address is retriveable
        /// </summary>
        public bool IsRetriveable { get; set; }

        /// <summary>
        /// Gets or sets the address action outcome.
        /// </summary>
        public AddressActionOutcome AddressActionOutcome { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label id.
        /// </summary>
        public int LabelId { get; set; }

        /// <summary>
        /// Gets or sets the start date for the address.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the address.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is home address.
        /// </summary>
        public bool IsHomeAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is office address.
        /// </summary>
        public bool IsOfficeAddress { get; set; }

        /// <summary>
        /// Gets the step summary.
        /// </summary>
        public string StepSummary { get; set; }

        /// <summary>
        /// Gets the address friendly text.
        /// </summary>
        public string AddressFriendlyText { get; set; }

        /// <summary>
        /// The ESR assignment Id associated with the address
        /// </summary>
        public int EsrAssignmentId { get; set; }

        /// <summary>Convert from a data access layer Type to an api Type.</summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The ActionContext to use in conversions.</param>
        /// <returns>An api Type.</returns>
        public Address From(SpendManagementLibrary.Addresses.Address dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.Identifier;
            IsAccountWideFavourite = dbType.AccountWideFavourite;
            AddressName = dbType.AddressName;
            Archived = dbType.Archived;
            City = dbType.City;
            Country = dbType.Country;
            County = dbType.County;
            Line1 = dbType.Line1;
            Line2 = dbType.Line2;
            Line3 = dbType.Line3;
            Postcode = dbType.Postcode;
            ModifiedById = dbType.ModifiedBy;
            ModifiedOn = dbType.ModifiedOn;
            AddressSource = AddressSource.SEL;
            GlobalIdentifier = dbType.GlobalIdentifier;

            if (Id > 0)
            {
                IsRetriveable = true;
            }
                      
            // get the labels + primary
            var labels = actionContext.AddressLabels.GetAccountWideLabelsForAddress(Id);
            var primaryLabel = labels.FirstOrDefault(x => x.Primary);

            // populate labels + primary
            AccountWideLabels = labels.Select(x => x.AddressLabelID).ToList();
            if (primaryLabel != null)
            {
                PrimaryAccountWideLabel = primaryLabel.AddressLabelID;
            }
   
            Archived = dbType.Archived;

            return this;
        }

        /// <summary>Converts to a data access layer Type from an api Type.</summary>
        /// <returns>A data access layer Type.</returns>
        public SpendManagementLibrary.Addresses.Address To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.Addresses.Address
                       {
                           Identifier = Id,
                           AccountWideFavourite = IsAccountWideFavourite,
                           AddressName = AddressName,
                           Archived = Archived,
                           City = City,
                           Country = Country,
                           County = County,
                           CreationMethod =
                               SpendManagementLibrary.Addresses.Address
                               .AddressCreationMethod.ManualByClaimant,
                           Line1 = Line1,
                           Line2 = Line2,
                           Line3 = Line3,
                           Postcode = Postcode,
                           SubAccountIdentifier = actionContext.SubAccountId,
                           ModifiedBy = ModifiedById ?? 0,
                           ModifiedOn = ModifiedOn ?? DateTime.UtcNow,
                       };
        }
    }

    /// <summary>
    /// Ties an <see cref="Address">Address</see> to an Employee.
    /// </summary>
    public class HomeAddressLinkage : BaseExternalType, IRequiresValidation, IEquatable<HomeAddressLinkage>
    {
        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the start date for this Address.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this Address.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The Id of the <see cref="Employee">Employee</see> to be linked to the <see cref="Address">Address</see>.
        /// </summary>
        [Required(ErrorMessage = ApiResources.ApiErrorAddressLinkageMustHaveEmployee)]
        public new int EmployeeId { get; set; }

        /// <summary>
        /// The Id of the <see cref="Address">Address</see> to be linked to the <see cref="Employee">Employee</see>.
        /// </summary>
        [Required(ErrorMessage = ApiResources.ApiErrorAddressLinkageMustHaveAddress)]
        public int AddressId { get; set; }

        /// <summary>
        /// Validates this object.
        /// </summary>
        /// <param name="actionContext">The action context</param>
        /// <exception cref="InvalidDataException">Any validation errors.</exception>
        public void Validate(IActionContext actionContext)
        {
            if (StartDate != null && EndDate != null)
            {
                if (EndDate.Value.CompareTo(StartDate.Value) == -1)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorAddressStartEndDateMismatch);
                }
            }

            if (actionContext.Addresses.GetAddressById(AddressId) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressNotFound);
            }

            if (actionContext.Employees.GetEmployeeById(EmployeeId) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }
        }

        /// <summary>Checks for equality.</summary>
        /// <param name="other">Address to compare</param>
        /// <returns></returns>
        public bool Equals(HomeAddressLinkage other)
        {
            if (other == null)
            {
                return false;
            }

            return this.AccountId.Equals(other.AccountId) && this.Id.Equals(other.Id)
                   && this.EndDate.Equals(other.EndDate) && this.StartDate.Equals(other.StartDate);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as HomeAddressLinkage);
        }
    }

    /// <summary>
    /// A WorkAddressLinkage is a subtype of <see cref="HomeAddressLinkage">AddressLinkage</see>. 
    /// </summary>
    public class WorkAddressLinkage : HomeAddressLinkage
    {
        /// <summary>
        /// Gets or sets a value indicating whether the address is temporary.
        /// </summary>
        [Required(ErrorMessage = ApiResources.ApiErrorWorkAddressLinkageMustHaveTemp)]
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the address is active.
        /// </summary>
        [Required(ErrorMessage = ApiResources.ApiErrorWorkAddressLinkageMustHaveActive)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the address is rotational.
        /// </summary>
        public bool Rotational { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the address is the primary rotational address.
        /// </summary>
        public bool PrimaryRotational { get; internal set; }
    }

    internal static class HomeAddressConversion
    {
        internal static TResult Cast<TResult>(this cEmployeeHomeLocation homeAddress, int accountId)
            where TResult : HomeAddressLinkage, new()
        {
            return new TResult
                       {
                           Id = homeAddress.EmployeeLocationID,
                           AddressId = homeAddress.LocationID,
                           EmployeeId = homeAddress.EmployeeID,
                           StartDate = homeAddress.StartDate,
                           EndDate = homeAddress.EndDate,
                           AccountId = accountId,
                           CreatedById = homeAddress.CreatedBy,
                           CreatedOn = homeAddress.CreatedOn,
                           ModifiedById = homeAddress.ModifiedBy,
                           ModifiedOn = homeAddress.ModifiedOn
                       };
        }

        internal static cEmployeeHomeLocation Cast(this HomeAddressLinkage address)
        {
            var homeLocation = new cEmployeeHomeLocation(
                address.Id,
                address.EmployeeId,
                address.AddressId,
                address.StartDate,
                address.EndDate,
                address.CreatedOn,
                address.CreatedById,
                address.ModifiedOn,
                address.ModifiedById);
            return homeLocation;
        }
    }

    internal static class WorkAddressConversion
    {
        internal static TResult Cast<TResult>(this cEmployeeWorkLocation workAddress, int accountId)
            where TResult : WorkAddressLinkage, new()
        {
            return new TResult
                       {
                           Id = workAddress.EmployeeWorkAddressId,
                           AddressId = workAddress.LocationID,
                           EmployeeId = workAddress.EmployeeID,
                           StartDate = workAddress.StartDate,
                           EndDate = workAddress.EndDate,
                           IsActive = workAddress.Active,
                           IsTemporary = workAddress.Temporary,
                           AccountId = accountId,
                           CreatedById = workAddress.CreatedBy,
                           CreatedOn = workAddress.CreatedOn,
                           ModifiedById = workAddress.ModifiedBy,
                           ModifiedOn = workAddress.ModifiedOn,
                           EsrAssignmentLocationId = workAddress.EsrAssignmentLocationId,
                           Rotational = workAddress.Rotational,
                           PrimaryRotational = workAddress.PrimaryRotational
                       };
        }

        internal static cEmployeeWorkLocation Cast<TResult>(this WorkAddressLinkage workAddressLinkage)
            where TResult : cEmployeeWorkLocation
        {
            return new cEmployeeWorkLocation(
                workAddressLinkage.Id,
                workAddressLinkage.EmployeeId,
                workAddressLinkage.AddressId,
                workAddressLinkage.StartDate,
                workAddressLinkage.EndDate,
                workAddressLinkage.IsActive,
                workAddressLinkage.IsTemporary,
                workAddressLinkage.CreatedOn,
                workAddressLinkage.CreatedById,
                workAddressLinkage.ModifiedOn,
                workAddressLinkage.ModifiedById,
                workAddressLinkage.EsrAssignmentLocationId,
                workAddressLinkage.PrimaryRotational);
        }
    }
}