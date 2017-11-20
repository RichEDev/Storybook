namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Eventing.Reader;
    using System.Linq;

    using Antlr.Runtime.Misc;

    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    using Spend_Management;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using Expedite;

    using Interfaces;

    using SpendManagementApi.Repositories;

    using SpendManagementLibrary.Flags;

    using Spend_Management.expenses.code;

    /// <summary>
    /// Represents an expenses sub category - so for an example, for an <see cref="ExpenseCategory">ExpenseCategory</see> called "Travel", 
    /// there could be ExpenseSubCategories called"mileage", "pedal cycle", "flights"  and so on.
    /// </summary>
    public class ExpenseSubCategory : BaseExternalType, IEquatable<ExpenseSubCategory>
    {
        /// <summary>
        /// Expense Sub Category constructor
        /// </summary>
        public ExpenseSubCategory()
        {
            this.Countries = new List<CountrySubCat>();
            this.VatRates = new List<SubCatVatRate>();
            this.UserDefined = new List<UserDefinedFieldValue>();
            this.UserDefinedFields = new List<UserDefinedFieldType>();
            this.ValidDates = new List<SubCatDates>();
        }

        #region Public Properties
        
        /// <summary>
        /// The unique Id of the sub category.
        /// </summary>
        public int SubCatId { get; set; }

        /// <summary>
        /// The Id of the parent category.
        /// </summary>
        [Required]
        public int? ParentCategoryId { get; set; }

        /// <summary>
        /// The name of the sub category.
        /// </summary>
        public string SubCat { get; set; }

        /// <summary>
        /// The description of the sub category.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Allowance Amount of the sub category.
        /// </summary>
        public decimal AllowanceAmount { get; set; }

        /// <summary>
        /// The Account Code for the sub category.
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// Whether to add the values for this category as Net.
        /// </summary>
        public bool AddAsNet { get; set; }

        /// <summary>
        /// The P11dCategory for the sub category.
        /// </summary>
        public int? PdCatId { get; set; }
        
        /// <summary>
        /// The calculation type for the sub category.
        /// </summary>
        public SpendManagementApi.Common.Enums.CalculationType CalculationType { get; set; }

        /// <summary>
        /// The comment to be shown to claimants.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Whether the sub category is re-imbursable.
        /// </summary>
        public bool Reimbursable { get; set; }

        /// <summary>
        /// The Alternate Account Code for the sub category.
        /// </summary>
        public string AlternateAccountCode { get; set; }

        /// <summary>
        /// The Abbreviation for the sub category.
        /// </summary>
        public string ShortSubCategory { get; set; }

        /// <summary>
        /// Whether VAT is applicable for the sub category.
        /// </summary>
        public bool VatApplicable
        {
            get
            {
                return VatRates.Any();
            }
        }

        /// <summary>
        /// The applicable VAT rates for the sub category.
        /// </summary>
        public List<SubCatVatRate> VatRates { get; set; }

        /// <summary>
        /// Whether to show the number of miles for the sub category.
        /// </summary>
        public bool MileageApplicable { get; set; }

        /// <summary>
        /// Whether to show the number of staff for the sub category.
        /// </summary>
        public bool StaffApplicable { get; set; }

        /// <summary>
        /// Whether to show the number of others for the sub category.
        /// </summary>
        public bool OthersApplicable { get; set; }

        /// <summary>
        /// Whether to show the tip for the sub category.
        /// </summary>
        public bool TipApplicable { get; set; }

        /// <summary>
        /// Gets or sets the tip limit.
        /// </summary>
        public decimal? TipLimit{get; set; }

        /// <summary>
        /// Whether to the show the number of personal miles for the sub category.
        /// </summary>
        public bool PersonalMilesApplicable { get; set; }

        /// <summary>
        /// Whether to show the number of Business Miles for the sub category.
        /// </summary>
        public bool BusinessMilesApplicable { get; set; }

        /// <summary>
        /// Whether to show the Attendees list for the sub category.
        /// </summary>
        public bool AttendeesApplicable { get; set; }

        /// <summary>
        /// Whether to show the event in home city for the sub category.
        /// </summary>
        public bool EventInHomeApp { get; set; }

        /// <summary>
        /// Whether to show normal receipt for the sub category.
        /// </summary>
        public bool ReceiptApplicable { get; set; }

        /// <summary>
        /// Whether to show the number of spouses / partners for the sub category.
        /// </summary>
        public bool NoPersonalGuestApplicable { get; set; }

        /// <summary>
        /// Whether to show passengers for the sub category.
        /// </summary>
        public bool PassengersApplicable { get; set; }

        /// <summary>
        /// Whether number of passengers applicable for the sub category.
        /// </summary>
        public bool NoPassengersApplicable { get; set; }

        /// <summary>
        /// Whether to show names of passengers for the sub category.
        /// </summary>
        public bool PassengersNameApplicable { get; set; }

        /// <summary>
        /// Whether entertainment is to be split for the sub category.
        /// </summary>
        public bool SplitEntertainment { get; set; }

        /// <summary>
        /// The associated entertainment subcategory Id for the sub category.
        /// </summary>
        public int EntertainmentId { get; set; }

        /// <summary>
        /// Whether number of nights applicable applies to the sub category.
        /// </summary>
        public bool NoNightsApplicable { get; set; }

        /// <summary>
        /// Whether attendee list is mandatory for the sub category.
        /// </summary>
        public bool AttendeesMandatory { get; set; }

        /// <summary>
        /// Whether number of directors applicable for the sub category.
        /// </summary>
        public bool NoDirectorsApplicable { get; set; }

        /// <summary>
        /// Whether hotel name is applicable for the sub category.
        /// </summary>
        public bool HotelApplicable { get; set; }

        /// <summary>
        /// Whether the number of rooms is applicable for the sub category.
        /// </summary>
        public bool NoRoomsApplicable { get; set; }

        /// <summary>
        /// Whether the hotel number is mandatory for the sub category.
        /// </summary>
        public bool HotelMandatory { get; set; }

        /// <summary>
        /// Whether the VAT number is shown for the sub category.
        /// </summary>
        public bool VatNumberApplicable { get; set; }

        /// <summary>
        /// Whether the VAT number is mandatory for the sub category.
        /// </summary>
        public bool VatNumberMandatory { get; set; }

        /// <summary>
        /// Whether the number of remote workers is shown for the sub category.
        /// </summary>
        public bool NoRemoteWorkersApplicable { get; set; }

        /// <summary>
        /// Whether to split to partners/ spouses for the sub category.
        /// </summary>
        public bool SplitPersonal { get; set; }

        /// <summary>
        /// Whether to show the split to remote workers for the sub category.
        /// </summary>
        public bool SplitRemote { get; set; }

        /// <summary>
        /// The sub category to split personal amount to.
        /// </summary>
        public int PersonalId { get; set; }

        /// <summary>
        /// The sub category id to split remote worker amount to.
        /// </summary>
        public int RemoteId { get; set; }

        /// <summary>
        /// Whether the reason is applicable for the sub category.
        /// </summary>
        public bool ReasonApplicable { get; set; }

        /// <summary>
        /// Whether other details are applicable for the sub category.
        /// </summary>
        public bool OtherDetailsApplicable { get; set; }

        /// <summary>
        /// The list of user defined fields associated with this sub category.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefined { get; set; }

        /// <summary>
        /// Whether the 'From' field is applicable for the sub category.
        /// </summary>
        public bool FromApplicable { get; set; }

        /// <summary>
        /// Whether the 'To' field is applicable for the sub category.
        /// </summary>
        public bool ToApplicable { get; set; }

        /// <summary>
        /// Gets or sets the list of country - subcategory associations for the sub category.
        /// </summary>
        public List<CountrySubCat> Countries { get; set; }

        /// <summary>
        /// List of allowances for the sub category.
        /// </summary>
        public List<int> Allowances { get; set; }

        /// <summary>
        /// List of associated user defined functions for the sub category.
        /// </summary>
        public List<int> AssociatedUdfs { get; set; }

        /// <summary>
        /// List of expense items to split for the sub category.
        /// </summary>
        public List<ExpenseSubCategory> Split { get; set; }

        /// <summary>
        /// Whether Company is applicable for the sub category.
        /// </summary>
        public bool CompanyApplicable { get; set; }

        /// <summary>
        /// Whether home to location mileage is enabled for the sub category.
        /// </summary>
        public bool EnableHomeToLocationMileage { get; set; }

        /// <summary>
        /// The Home-to-Location type for the sub category.
        /// </summary>
        public SpendManagementApi.Common.Enums.HomeToLocationType HomeToLocationType { get; set; }

        /// <summary>
        /// The Mileage Category for the sub category, if one exists.
        /// </summary>
        public int? MileageCategory { get; set; }

        /// <summary>
        /// Whether the mileage is relocation mileage.
        /// </summary>
        public bool IsRelocationMileage { get; set; }

        /// <summary>
        /// The Re-imbursable Sub category id for the sub category.
        /// </summary>
        public int? ReimbursableSubCatId { get; set; }

        /// <summary>
        /// Whether to allow heavy bulky mileage for the sub category.
        /// </summary>
        public bool AllowHeavyBulkyMileage { get; set; }

        /// <summary>
        /// Whether home to office is zero for the sub category.
        /// </summary>
        public bool HomeToOfficeAsZero { get; set; }

        /// <summary>
        /// The start date for the sub category.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date for the sub category.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Whether expense items for this subcat should be validated.
        /// </summary>
        public bool Validate { get; set; }

        /// <summary>
        /// The requirements against which items for this should be validated.
        /// </summary>
        public List<ExpenseValidationCriterion> ValidationRequirements { get; set; }

        /// <summary>
        /// The number of fixed miles to deduct
        /// </summary>
        public float? HomeToOfficeFixedMiles { get; set; }

        /// <summary>
        /// The public transport rate to deduct
        /// </summary>
        public int? PublicTransportRate { get; set; }

        /// <summary>
        /// Enable Duty of care check at expense item level, for Mileage expense item type.
        /// </summary>
        public bool EnableDoc { get;set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the maximum limit without receipt.
        /// </summary>
        public decimal MaximumLimitWithoutReceipt { get; set; }

        /// <summary>
        /// Gets or sets the maximum limit.
        /// </summary>
        public decimal MaximumLimit { get; set; }

        /// <summary>
        /// Gets or sets UDFs(item specific ones only) for respective sub category.
        /// </summary>
        public List<UserDefinedFieldType> UserDefinedFields { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List"/> or <seealso cref="SubCatDates"/> which define the valid ranges for this <seealso cref="ExpenseSubCategory"/>
        /// </summary>
        public List<SubCatDates> ValidDates { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExpenseSubCategory);
        }

        public bool Equals(ExpenseSubCategory other)
        {
            if (other == null)
            {
                return false;
            }
         
            List<int> subcatIds = other.Split.Select(subcat => subcat.SubCatId).ToList();
            List<int> typeSubCatIds = this.Split.Select(subcat => subcat.SubCatId).ToList();

            return this.AccountCode.Equals(other.AccountCode) && this.AddAsNet.Equals(other.AddAsNet)
                   && this.AllowHeavyBulkyMileage.Equals(other.AllowHeavyBulkyMileage)
                   && this.AllowanceAmount.Equals(other.AllowanceAmount)
                   && (this.Allowances ?? new List<int>()).SequenceEqual(other.Allowances ?? new List<int>())
                   && this.AlternateAccountCode.Equals(other.AlternateAccountCode)
                   && (this.AssociatedUdfs ?? new List<int>()).SequenceEqual(other.AssociatedUdfs ?? new List<int>())
                   && this.AttendeesApplicable.Equals(other.AttendeesApplicable)
                   && this.AttendeesMandatory.Equals(other.AttendeesMandatory)
                   && this.BusinessMilesApplicable.Equals(other.BusinessMilesApplicable)
                   && this.CalculationType.Equals(other.CalculationType)
                   && this.ParentCategoryId.Equals(other.ParentCategoryId) && this.Comment.Equals(other.Comment)
                   && this.CompanyApplicable.Equals(other.CompanyApplicable)
                   && (this.Countries ?? new List<CountrySubCat>()).Select(c => c.CountryId)
                          .SequenceEqual((other.Countries ?? new List<CountrySubCat>()).Select(c => c.CountryId))
                   && (this.Countries ?? new List<CountrySubCat>()).Select(c => c.AccountCode)
                          .SequenceEqual((other.Countries ?? new List<CountrySubCat>()).Select(c => c.AccountCode))
                   && this.Description.Equals(other.Description)
                   && this.EnableHomeToLocationMileage.Equals(other.EnableHomeToLocationMileage)
                   && this.DateCompare(this.EndDate, other.EndDate)
                   && this.EntertainmentId.Equals(other.EntertainmentId)
                   && this.EventInHomeApp.Equals(other.EventInHomeApp)
                   && this.FromApplicable.Equals(other.FromApplicable)
                   && this.HomeToLocationType.Equals(other.HomeToLocationType)
                   && this.HomeToOfficeAsZero.Equals(other.HomeToOfficeAsZero)
                   && this.HomeToOfficeFixedMiles.Equals(other.HomeToOfficeFixedMiles)
                   && this.HotelApplicable.Equals(other.HotelApplicable)
                   && this.HotelMandatory.Equals(other.HotelMandatory)
                   && this.IsRelocationMileage.Equals(other.IsRelocationMileage)
                   && this.MileageApplicable.Equals(other.MileageApplicable)
                   && (this.MileageCategory ?? 0).Equals(other.MileageCategory ?? 0)
                   && this.NoDirectorsApplicable.Equals(other.NoDirectorsApplicable)
                   && this.NoNightsApplicable.Equals(other.NoNightsApplicable)
                   && this.NoPassengersApplicable.Equals(other.NoPassengersApplicable)
                   && this.NoPersonalGuestApplicable.Equals(other.NoPersonalGuestApplicable)
                   && this.NoRemoteWorkersApplicable.Equals(other.NoRemoteWorkersApplicable)
                   && this.NoRoomsApplicable.Equals(other.NoRoomsApplicable)
                   && this.OtherDetailsApplicable.Equals(other.OtherDetailsApplicable)
                   && this.OthersApplicable.Equals(other.OthersApplicable)
                   && this.PersonalMilesApplicable.Equals(other.PersonalMilesApplicable)
                   && this.PassengersApplicable.Equals(other.PassengersApplicable)
                   && this.PassengersNameApplicable.Equals(other.PassengersNameApplicable)
                   && this.PdCatId.Equals(other.PdCatId) && this.PersonalId.Equals(other.PersonalId)
                   && this.ReasonApplicable.Equals(other.ReasonApplicable)
                   && this.ReceiptApplicable.Equals(other.ReceiptApplicable)
                   && this.Reimbursable.Equals(other.Reimbursable)
                   && this.ReimbursableSubCatId.Equals(other.ReimbursableSubCatId)
                   && this.RemoteId.Equals(other.RemoteId) && this.ShortSubCategory.Equals(other.ShortSubCategory)
                   && this.SubCat.Equals(other.SubCat)
                   && subcatIds.Equals(typeSubCatIds)
                   && this.SplitEntertainment.Equals(other.SplitEntertainment)
                   && this.SplitPersonal.Equals(other.SplitPersonal) && this.SplitRemote.Equals(other.SplitRemote)
                   && this.StaffApplicable.Equals(other.StaffApplicable)
                   && this.DateCompare(this.StartDate, other.StartDate)
                   && this.TipApplicable.Equals(other.TipApplicable) && this.ToApplicable.Equals(other.ToApplicable)
                   && this.VatApplicable.Equals(other.VatApplicable)
                   && this.VatNumberApplicable.Equals(other.VatNumberApplicable)
                   && this.VatNumberMandatory.Equals(other.VatNumberMandatory)
                   && (this.VatRates ?? new List<SubCatVatRate>()).SequenceEqual(
                       other.VatRates ?? new List<SubCatVatRate>()) && this.Validate.Equals(other.Validate)
                   && this.ValidationRequirements.Equals(other.ValidationRequirements)
                   && this.EnableDoc.Equals(other.EnableDoc);
        }
    }

    /// <summary>
    /// A class to represent start and end dates for Sub cat item roles
    /// </summary>
    public class SubCatDates
    {
        /// <summary>
        /// Gets or sets the start date for this item
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this item
        /// </summary>
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// Represents a join between a <see cref="Country">Country</see> and an <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.
    /// </summary>
    public class CountrySubCat : IApiFrontForDbObject<cCountrySubcat, CountrySubCat>
    {
        /// <summary>
        /// The Id of the <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.
        /// </summary>
        public int SubCatId { get; set; }
        
        /// <summary>
        /// The Id of the <see cref="Country">Country</see>.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// The Account Code.
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// Populates this Item from a DAL type.
        /// </summary>
        /// <param name="dbType">The database access layer type.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>This, a fully populated object</returns>
        public CountrySubCat From(cCountrySubcat dbType, IActionContext actionContext)
        {
            SubCatId = dbType.SubcatId;
            CountryId = dbType.countryid;
            AccountCode = dbType.accountcode;
            return this;
        }

        /// <summary>
        /// Converts this Item to it's DAL type.
        /// </summary>
        /// <returns></returns>
        public cCountrySubcat To(IActionContext actionContext)
        {
            return new cCountrySubcat(SubCatId, CountryId, AccountCode);
        }
    }

    /// <summary>
    /// Represents a basic version of a subcat
    /// </summary>
    public class ExpenseSubCategoryItemRoleBasic : BaseExternalType
    {
        #region properties

        /// <summary>
        /// The subcat id
        /// </summary>
        public int SubcatId { get; set; }

        /// <summary>
        /// The subcat name
        /// </summary>
        public string Subcat { get; set; }

        /// <summary>
        /// The Short Subcat Name
        /// </summary>
        public string ShortSubcat { get; set; }

        /// <summary>
        /// Maximum
        /// </summary>
        public decimal Maximum { get; set; }

        /// <summary>
        /// The Receipt Maximum
        /// </summary>
        public decimal ReceiptMaximum { get; set; }

        /// <summary>
        /// The CategoryId
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The calculation type
        /// </summary>
        public CalculationType CalculationType { get; set; }

        /// <summary>
        /// Whether the 'From' field is applicable for the sub category.
        /// </summary>
        public bool FromApp { get; set; }

        /// <summary>
       ///  Whether the 'To' field is applicable for the sub category.
        /// </summary>
        public bool ToApp { get; set; }

        /// <summary>
        /// The Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The symbol of the currency
        /// </summary>
        public string CurrencySymbol { get; set; }

        #endregion properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ExpenseSubCategoryItemRoleBasic other)
        {
            if (other == null)
            {
                return false;
            }

            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExpenseSubCategoryItemRoleBasic);
        }
    }


    public class ExpenseSubCategoryNames : BaseExternalType
    {
        /// <summary>
        /// The Subcat Id.
        /// </summary>
        public int SubCatId { get; set; }

        /// <summary>
        /// The Subcat name.
        /// </summary>
        public string Name { get; set; }
    }


    public class ExpenseSubCategoryBasic : BaseExternalType
    {
        /// <summary>
        /// The subcat id
        /// </summary>
        public int SubcatId { get; set; }

        /// <summary>
        /// The subcat name
        /// </summary>
        public string Subcat { get; set; }

        /// <summary>
        /// Whether VAT is applicable for the sub category.
        /// </summary>
        public bool VatApp { get; set; }

        /// <summary>
        /// The calculation type
        /// </summary>
        public CalculationType CalculationType { get; set; }

        /// <summary>
        /// P11D Category Id
        /// </summary>
        public int P11DCategoryId { get; set; }

        /// <summary>
        /// Category Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Subcat start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Subcat end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Converts a <see cref="SubCatBasic"></see> SubCatBasic to <see cref="ExpenseSubCategoryBasic" </see> ExpenseSubCategoryBasic API type
        /// </summary>
        /// <param name="subcatBasic">The <see cref="SubCatBasic"></see> SubCatBasic</param>
        /// <param name="actionContext">The actionContext which contains DAL classes</param>
        /// <returns></returns>
           public ExpenseSubCategoryBasic From(SubcatBasic subcatBasic, IActionContext actionContext)
           {
               SubcatId = subcatBasic.SubcatId;
               Subcat = subcatBasic.Subcat;
               VatApp = subcatBasic.VatApp;
               CalculationType = subcatBasic.CalculationType;
               P11DCategoryId = subcatBasic.P11DCategoryId;
               CategoryId = subcatBasic.CategoryId;
               StartDate = subcatBasic.StartDate;
               EndDate = subcatBasic.EndDate;        
            return this;
        }
    }

    internal static class ExpenseSubCategoryExtension
    {
        internal static cSubcat Cast<TRes>(
            this ExpenseSubCategory expSubCat,
            cSubcat origExpSubCat = null,
            IActionContext actionContext = null) where TRes : cSubcat, new()
        {
            if (expSubCat == null)
                return null;

            var sortedListUdfs = new SortedList<int, object>();

            foreach (UserDefinedFieldValue userDefinedFieldValue in expSubCat.UserDefined)
            {
                sortedListUdfs.Add(userDefinedFieldValue.Id, userDefinedFieldValue.Value);
            }

            List<int> subCatIds = expSubCat.Split.Select(subcat => subcat.SubCatId).ToList();

            return new cSubcat(
                    expSubCat.SubCatId,
                    expSubCat.ParentCategoryId.Value,
                    expSubCat.SubCat,
                    expSubCat.Description,
                    expSubCat.MileageApplicable,
                    expSubCat.StaffApplicable,
                    expSubCat.OthersApplicable,
                    expSubCat.TipApplicable,
                    expSubCat.PersonalMilesApplicable,
                    expSubCat.BusinessMilesApplicable,
                    expSubCat.AllowanceAmount,
                    expSubCat.AccountCode,
                    expSubCat.AttendeesApplicable,
                    expSubCat.AddAsNet,
                    expSubCat.PdCatId.HasValue ? expSubCat.PdCatId.Value : 0,
                    expSubCat.EventInHomeApp,
                    expSubCat.ReceiptApplicable,
                (CalculationType)
                Enum.Parse(
                    typeof(CalculationType),
                    ((int)expSubCat.CalculationType).ToString(CultureInfo.InvariantCulture)),
                    expSubCat.PassengersApplicable,
                    expSubCat.NoPassengersApplicable,
                    expSubCat.PassengersNameApplicable,
                    expSubCat.Comment,
                    expSubCat.SplitEntertainment,
                    expSubCat.EntertainmentId,
                    expSubCat.Reimbursable,
                    expSubCat.NoNightsApplicable,
                    expSubCat.AttendeesMandatory,
                    expSubCat.NoDirectorsApplicable,
                    expSubCat.HotelApplicable,
                    expSubCat.NoRoomsApplicable,
                    expSubCat.HotelMandatory,
                    expSubCat.VatNumberApplicable,
                    expSubCat.VatNumberMandatory,
                    expSubCat.NoPersonalGuestApplicable,
                    expSubCat.NoRemoteWorkersApplicable,
                    expSubCat.AlternateAccountCode,
                    expSubCat.SplitPersonal,
                    expSubCat.SplitRemote,
                    expSubCat.PersonalId,
                    expSubCat.RemoteId,
                    expSubCat.ReasonApplicable,
                    expSubCat.OtherDetailsApplicable,
                    sortedListUdfs,
                    expSubCat.CreatedOn,
                    expSubCat.CreatedById,
                    expSubCat.ModifiedOn,
                    expSubCat.ModifiedById,
                    expSubCat.ShortSubCategory,
                    expSubCat.FromApplicable,
                    expSubCat.ToApplicable,
                expSubCat.Countries == null
                    ? new List<cCountrySubcat>()
                    : expSubCat.Countries.Select(c => c.To(actionContext)).ToList(),
                    expSubCat.Allowances ?? new ListStack<int>(),
                    expSubCat.AssociatedUdfs ?? new List<int>(),
                    subCatIds,
                    expSubCat.CompanyApplicable,
                    expSubCat.VatRates.Select(v => v.Cast()).ToList(),
                    expSubCat.EnableHomeToLocationMileage,
                    (HomeToLocationType)expSubCat.HomeToLocationType,
                    expSubCat.MileageCategory,
                    expSubCat.IsRelocationMileage,
                    expSubCat.ReimbursableSubCatId,
                    expSubCat.AllowHeavyBulkyMileage,
                    expSubCat.HomeToOfficeAsZero,
                    expSubCat.HomeToOfficeFixedMiles,
                    expSubCat.PublicTransportRate,
                    expSubCat.StartDate,
                    expSubCat.EndDate,
                    expSubCat.Validate,
                    expSubCat.ValidationRequirements.Select(v => v.To(actionContext)).ToList(),
                    expSubCat.EnableDoc);
        }

        internal static TRes Cast<TRes>(
            this cSubcat cSubCat,
            cCategories categories,
            int accountId,
            IActionContext actionContext) where TRes : ExpenseSubCategory, new()
        {
            if (cSubCat == null)
            {
                return null;
            }

            var userDefinedFields = new List<UserDefinedFieldValue>();

            if (cSubCat.userdefined != null)
            {       
                // Return only UDF values for UDFs assoicated with the Sub Category
                foreach (var udfValue in cSubCat.userdefined.ToUserDefinedFieldValueList())
                {
                    if (cSubCat.associatedudfs.Any(x => x == udfValue.Id))
                    {
                        userDefinedFields.Add(udfValue);
                    }
                }
            }
       
            // Get all acitve UDFs which are not item-specific
            var itemSpecificUdfs = new List<UserDefinedFieldType>();
            var expenseItemGeneralUserDefinedFields = actionContext.ExpenseItems.GetExpenseItemDefinitionUDFs(accountId);
            if (expenseItemGeneralUserDefinedFields != null)
            {
                foreach (SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue userDefinedField in expenseItemGeneralUserDefinedFields)
                {
                    var userDefinedFiledValue = (cUserDefinedField)userDefinedField.Value;
                    if (userDefinedFiledValue.Specific && cSubCat.associatedudfs.Any(x => x == userDefinedFiledValue.userdefineid))
                    {
                        var userDefinedFieldType = new UserDefinedFieldType().From(userDefinedFiledValue, actionContext);
                        itemSpecificUdfs.Add(userDefinedFieldType);
                    }
                }
            }

            var expenseSubCategories = new List<ExpenseSubCategory>();
            var user = cMisc.GetCurrentUser();
            var expenseSubCategoryRepository = new ExpenseSubCategoryRepository(user, actionContext);

            foreach (int subCat in cSubCat.subcatsplit)
            {
                expenseSubCategories.Add(expenseSubCategoryRepository.Get(subCat));
            }

            decimal? tipLimit = null;

            if (cSubCat.tipapp)
            {
                tipLimit = CalculateTipLimit(cSubCat.subcatid, actionContext, user);
            }

            return new TRes
                {
                    SubCatId = cSubCat.subcatid,
                    SubCat = cSubCat.subcat,
                    AccountId = accountId,
                    AccountCode = cSubCat.sAccountcode,
                    AddAsNet = cSubCat.addasnet,
                    AllowanceAmount = cSubCat.allowanceamount,
                    Allowances = cSubCat.allowances,
                    AllowHeavyBulkyMileage = cSubCat.allowHeavyBulkyMileage,
                    AlternateAccountCode = cSubCat.alternateaccountcode,
                    AssociatedUdfs = cSubCat.associatedudfs,
                    AttendeesApplicable = cSubCat.attendeesapp,
                    AttendeesMandatory = cSubCat.attendeesmand,
                    BusinessMilesApplicable = cSubCat.bmilesapp,
                    CalculationType = (SpendManagementApi.Common.Enums.CalculationType)cSubCat.calculation,
                    ParentCategoryId = cSubCat.categoryid,
                    Comment = cSubCat.comment,
                    CompanyApplicable = cSubCat.companyapp,
                           Countries =
                               cSubCat.countries.Select(x => new CountrySubCat().From(x, actionContext)).ToList(),
                    CreatedOn = cSubCat.createdon,
                    CreatedById = cSubCat.createdby,
                    Description = cSubCat.description,
                    EnableHomeToLocationMileage = cSubCat.EnableHomeToLocationMileage,
                    EndDate = DateTime.MaxValue,
                    EntertainmentId = cSubCat.entertainmentid,
                    EventInHomeApp = cSubCat.eventinhomeapp,
                    FromApplicable = cSubCat.fromapp,
                           HomeToLocationType =
                               (SpendManagementApi.Common.Enums.HomeToLocationType)cSubCat.HomeToLocationType,
                    HomeToOfficeAsZero = cSubCat.HomeToOfficeAlwaysZero,
                    HomeToOfficeFixedMiles = cSubCat.HomeToOfficeFixedMiles,
                    PublicTransportRate = cSubCat.PublicTransportRate,
                    HotelApplicable = cSubCat.hotelapp,
                    HotelMandatory = cSubCat.hotelmand,
                    IsRelocationMileage = cSubCat.IsRelocationMileage,
                    MileageApplicable = cSubCat.mileageapp,
                    MileageCategory = cSubCat.MileageCategory,
                    ModifiedOn = cSubCat.modifiedon,
                    ModifiedById = cSubCat.modifiedby,
                    NoDirectorsApplicable = cSubCat.nodirectorsapp,
                    NoNightsApplicable = cSubCat.nonightsapp,
                    NoPassengersApplicable = cSubCat.nopassengersapp,
                    NoPersonalGuestApplicable = cSubCat.nopersonalguestsapp,
                    NoRemoteWorkersApplicable = cSubCat.noremoteworkersapp,
                    NoRoomsApplicable = cSubCat.noroomsapp,
                    OtherDetailsApplicable = cSubCat.otherdetailsapp,
                    OthersApplicable = cSubCat.othersapp,
                    PersonalMilesApplicable = cSubCat.pmilesapp,
                    PassengersApplicable = cSubCat.passengersapp,
                    PassengersNameApplicable = cSubCat.passengernamesapp,
                    PdCatId = cSubCat.pdcatid,
                    PersonalId = cSubCat.personalid,
                    ReasonApplicable = cSubCat.reasonapp,
                    ReceiptApplicable = cSubCat.receiptapp,
                    Reimbursable = cSubCat.reimbursable,
                    ReimbursableSubCatId = cSubCat.reimbursableSubcatID,
                    RemoteId = cSubCat.remoteid,
                    ShortSubCategory = cSubCat.shortsubcat,
                    SplitEntertainment = cSubCat.splitentertainment,
                    SplitPersonal = cSubCat.splitpersonal,
                    SplitRemote = cSubCat.splitremote,
                    StaffApplicable = cSubCat.staffapp,
                    StartDate = DateTime.MinValue,
                    Split = expenseSubCategories,
                    TipApplicable = cSubCat.tipapp,
                    TipLimit = tipLimit,
                    ToApplicable = cSubCat.toapp,
                    UserDefined = userDefinedFields,
                    UserDefinedFields = itemSpecificUdfs,
                    VatNumberApplicable = cSubCat.vatnumberapp,
                    VatNumberMandatory = cSubCat.vatnumbermand,
                           VatRates =
                               (cSubCat.vatrates ?? new List<cSubcatVatRate>()).Select(v => v.Cast<SubCatVatRate>())
                               .ToList(),
                    Validate = cSubCat.Validate,
                           ValidationRequirements =
                               cSubCat.ValidationRequirements.Select(
                                   v => new ExpenseValidationCriterion().From(v, actionContext)).ToList(),
                    EnableDoc = cSubCat.EnableDoC
                };
        }

        /// <summary>
        /// Returns a tip limit if there is a blocking flag for the subcat
        /// </summary>
        /// <param name="subcatId">The Id of the subcat</param>
        /// <param name="actionContext">The actioncontext</param>
        /// <param name="user">The current user</param>
        /// <returns>The tip limit, else null if no tip limit is applicable</returns>
        private static decimal? CalculateTipLimit(int subcatId, IActionContext actionContext, CurrentUser user)
        {
            var employee = actionContext.Employees.GetEmployeeById(user.EmployeeID);

            Dictionary<int, cRoleSubcat> roleSubcats = actionContext.Employees.getResultantRoleSet(employee);

            int itemRoleId = 0;
            cRoleSubcat rolesubcat;

            if (roleSubcats.TryGetValue(subcatId, out rolesubcat))
            {
                itemRoleId = rolesubcat.roleid;
            }

            FlagManagement flagManagement = new FlagManagement(user.AccountID);
          
            TipFlag tipFlag = (TipFlag)flagManagement.GetFlagByTypeRoleAndExpenseItem(FlagType.TipLimitExceeded, itemRoleId, subcatId);

            if (tipFlag != null && tipFlag.Action == FlagAction.BlockItem)
            {
                return tipFlag.TipLimit;
            }
            else
            {
                return null;
            }
        }
    }

    internal static class ExpenseSubCategoryNamesByIdsExtension
    {
        //Casts a Dictionary<int, string> SubcatItemRoleBasic to as list of ExpenseSubCategoryNames for the response.
        public static TResult Cast<TResult>(this Dictionary<int, string> expenseSubCategoryNames)
               where TResult : List<ExpenseSubCategoryNames>, new()
        {
            List<ExpenseSubCategoryNames> subcatNames =
                expenseSubCategoryNames.Select(
                    item => new ExpenseSubCategoryNames { SubCatId = item.Key, Name = item.Value }).ToList();
            return (TResult)subcatNames;
        }
    }

    internal static class ExpenseSubCategoryItemRoleBasicExtension
    {
        //Casts a List of SubcatItemRoleBasic to as list of ExpenseSubCategoryItemRoleBasic for the response.
        public static TResult Cast<TResult>(this IList<SubcatItemRoleBasic> subcatItemRoleBasic)
               where TResult : List<ExpenseSubCategoryItemRoleBasic>, new()
        {
            List<ExpenseSubCategoryItemRoleBasic> subCategoryItemRoleBasic =
                subcatItemRoleBasic.Select(
                    subcat =>
                    new ExpenseSubCategoryItemRoleBasic
                    {
                        SubcatId = subcat.SubcatId,
                        Subcat = subcat.Subcat,
                        ShortSubcat = subcat.ShortSubcat,
                        Description = subcat.Description,
                        ReceiptMaximum = subcat.ReceiptMaximum,
                        Maximum = subcat.Maximum,
                        CategoryId = subcat.CategoryId,
                        CalculationType = subcat.CalculationType,
                        FromApp = subcat.FromApp,
                        ToApp = subcat.ToApp,
                        CurrencySymbol = subcat.CurrencySymbol
                    }).ToList();
            return (TResult)subCategoryItemRoleBasic;
        }      
    }
}