namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using SpendManagementApi.Common;
    using Attributes.Validation;
    using Common;
    using Interfaces;
    using Utilities;
    using SpendManagementLibrary;
    using RangeType = SpendManagementApi.Common.Enums.RangeType;
    using Requests;

    /// <summary>
    /// Defines the amount of money an employee will get paid per unit of distance, and within particular date ranges.
    /// </summary>
    public class MileageCategory : BaseExternalType, IRequiresValidation, IEquatable<MileageCategory>
    {
        #region properties

        /// <summary>
        /// The unique Id for this mileage category.
        /// </summary>
        public int MileageCategoryId { get; set; }

        /// <summary>
        /// The label for this mileage category.
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// The comment for this mileage category.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The ThresholdType for this mileage category.
        /// </summary>
        [Required]
        public SpendManagementApi.Common.Enums.ThresholdType ThresholdType { get; set; }

        /// <summary>
        /// Whether new journey totals should be calculated for this mileage category.
        /// </summary>
        public bool CalculateNewJourneyTotal { get; set; }

        /// <summary>
        /// A list of date ranges that define when this mileage category applies.
        /// </summary>
        public List<DateRange> DateRanges { get; set; }

        /// <summary>
        /// The unit of measure for this mileage category.
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public MileageUOM UnitOfMeasure { get; set; }

        internal bool CatValid { get; set; }

        internal string CatValidComment { get; set; }

        /// <summary>
        /// The Id of the currency that applies to this mileage category.
        /// </summary>
        public int Currency { get; set; }

        /// <summary>
        /// The user rates table.
        /// </summary>
        [Required]
        public string NhsMileageCode { get; set; }

        /// <summary>
        /// The user rates from engine size.
        /// </summary>
        public int StartEngineSize { get; set; }

        /// <summary>
        /// The user rates to engine size.
        /// </summary>
        public int EndEngineSize { get; set; }

        /// <summary>
        /// The Id of the financial year for this mileage category.
        /// </summary>
        public int? FinancialYearId { get; set; }

        /// <summary>
        /// The tooltip for the mileage category
        /// </summary>
        public string TooltipInfo { get; set; }

        /// <summary>
        /// Gets or sets the home to office deduction rules.
        /// </summary>
        public string HomeToOfficeDeductionRules { get; set; }

        #endregion

        public bool Equals(MileageCategory other)
        {
            if (other == null)
            {
                return false;
            }
            return this.CalculateNewJourneyTotal.Equals(other.CalculateNewJourneyTotal)
                   && this.Label.Equals(other.Label)
                   && this.Comment.Equals(other.Comment)
                   && this.Currency.Equals(other.Currency) && this.DateRanges.SequenceEqual(other.DateRanges)
                   && this.EndEngineSize.Equals(other.EndEngineSize)
                   && this.FinancialYearId.Equals(other.FinancialYearId)
                   && this.NhsMileageCode.Equals(other.NhsMileageCode)
                   && this.StartEngineSize.Equals(other.StartEngineSize)
                   && this.ThresholdType.Equals(other.ThresholdType) && this.UnitOfMeasure.Equals(other.UnitOfMeasure);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MileageCategory);
        }

        /// <summary>
        /// Validates the <see cref="MileageCategory">MileageCategory</see>
        /// </summary>
        /// <param name="actionContext"></param>
        /// <exception cref="ApiException"></exception>
        public void Validate(IActionContext actionContext)
        {
            if (string.IsNullOrEmpty(this.NhsMileageCode) && actionContext.Accounts.GetAccountByID(actionContext.AccountId).IsNHSCustomer)
            {
                throw new ApiException(
                    ApiResources.MileageCategories_InvalidNhsMileageCode,
                    ApiResources.MileageCategories_InvalidNhsMileageCodeMessage);
            }
            if (actionContext.MileageCategories.getMileageCatByName(this.Label) != null)
            {
                throw new ApiException(ApiResources.MileageCategories_InvalidCarSize, ApiResources.MileageCategories_InvalidCarSizeMessage);
            }
            this.DateRanges.ForEach(r => Helper.ValidateIfNotNull(r, actionContext, this.AccountId));
        }
    }

    /// <summary>
    /// Defines a time period of days where a <see cref="MileageCategory">MileageCategory</see> applies.
    /// </summary>
    public class DateRange : BaseExternalType, IEquatable<DateRange>, IRequiresValidation
    {
        #region properties

        /// <summary>
        /// The unique date Id.
        /// </summary>
        public int MileageDateId { get; set; }

        /// <summary>
        /// The Id of the mileage category.
        /// </summary>
        public int MileageCategoryId { get; set; }

        /// <summary>
        /// The first date value.
        /// </summary>
        public DateTime? DateValue1 { get; set; }

        /// <summary>
        /// The second date value.
        /// </summary>
        public DateTime? DateValue2 { get; set; }

        /// <summary>
        /// The list of thresholds that apply to this date range.
        /// </summary>
        public List<Threshold> Thresholds { get; set; }

        /// <summary>
        /// The type of date range.
        /// </summary>
        public SpendManagementApi.Common.Enums.DateRangeType DateRangeType { get; set; }

        #endregion

        public bool Equals(DateRange other)
        {
            if (other == null)
            {
                return false;
            }
            return this.DateCompare(DateValue1, other.DateValue1) && this.DateCompare(this.DateValue2, other.DateValue2)
                   //&& this.Thresholds.SequenceEqual(other.Thresholds)
                   && this.DateRangeType.Equals(other.DateRangeType);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DateRange);
        }

        public void Validate(IActionContext actionContext)
        {
            if (this.Thresholds != null && this.Thresholds.Count > 0)
            {
                this.Thresholds.ForEach(t => Helper.ValidateIfNotNull(t, actionContext, this.AccountId));
            }
        }
    }

    /// <summary>
    /// A threshold defines details for paying an expense, up to a certain value, before another threshold applies.
    /// </summary>
    public class Threshold : BaseExternalType, IEquatable<Threshold>, IRequiresValidation
    {
        #region Properties

        /// <summary>
        /// The unique Id of this threshold.
        /// </summary>
        public int MileageThresholdId { get; set; }

        /// <summary>
        /// The Id of the related DateRange.
        /// </summary>
        public int MileageDateId { get; set; }

        /// <summary>
        /// The Id of the related MileageCategory.
        /// </summary>
        public int MileageCategoryId { get; set; }

        /// <summary>
        /// The first range value.
        /// </summary>
        public decimal? RangeValue1 { get; set; }

        /// <summary>
        /// The second range value.
        /// </summary>
        public decimal? RangeValue2 { get; set; }

        /// <summary>
        /// The RangeType of this threshold.
        /// </summary>
        public RangeType RangeType { get; set; }

        /// <summary>
        /// The first passenger rate for this threshold.
        /// </summary>
        public decimal Passenger1Rate { get; set; }

        /// <summary>
        /// The nth passenger rate for this threshold.
        /// </summary>
        public decimal PassengerXRate { get; set; }

        /// <summary>
        /// The allowance given for heavy and bulky equipment
        /// </summary>
        public decimal HeavyBulkyEquipmentRate { get; set; }

        /// <summary>
        /// Gets or sets the fuel rates for this threshold.
        /// </summary>
        public IEnumerable<VehicleJourneyRateThresholdRate> Rates { get; set; }

        #endregion

        public bool Equals(Threshold other)
        {
            if (other == null)
            {
                return false;
            }
            return this.HeavyBulkyEquipmentRate.Equals(other.HeavyBulkyEquipmentRate)
                   && this.Passenger1Rate.Equals(other.Passenger1Rate)
                   && this.PassengerXRate.Equals(other.PassengerXRate)
                   && this.RangeType.Equals(other.RangeType) && this.RangeValue1.Equals(other.RangeValue1)
                   && this.RangeValue2.Equals(other.RangeValue2);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Threshold);
        }

        public void Validate(IActionContext actionContext)
        {

        }
    }

    /// <summary>
    /// A Fuelrate defines the vehicle journey rate threshold rate.
    /// </summary>
    public class FuelRate : BaseExternalType, IRequiresValidation, IEquatable<FuelRate>
    {
        /// <summary>
        /// The unique Id of this threshold rate.
        /// </summary>
        public int? MileageThresholdRateId { get; set; }

        /// <summary>
        /// The Id of the mileage threshold.
        /// </summary>
        public int? MileageThresholdId { get; set; }

        /// <summary>
        /// The vehicle engine type id.
        /// </summary>
        public SpendManagementApi.Common.Enums.VehicleEngineType VehicleEngineTypeId { get; set; }

        /// <summary>
        /// The rate per unit.
        /// </summary>
        public decimal? RatePerUnit { get; set; }

        /// <summary>
        /// The vat amount.
        /// </summary>
        public decimal? AmountForVat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FuelRate other)
        {
            if (other == null)
            {
                return false;
            }
            return this.MileageThresholdRateId.Equals(other.MileageThresholdRateId)
                && this.MileageThresholdId.Equals(other.MileageThresholdId)
                && this.VehicleEngineTypeId.Equals(other.VehicleEngineTypeId)
                && this.RatePerUnit.Equals(other.RatePerUnit)
                && this.AmountForVat.Equals(other.AmountForVat);
        }

        /// <summary>
        /// Compares fuel rates.
        /// </summary>
        /// <param name="obj">Object to copmare.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FuelRate);
        }

        /// <summary>
        /// Validates a <see cref="FuelRate">FuelRate</see>
        /// </summary>
        /// <param name="actionContext"></param>
        public void Validate(IActionContext actionContext)
        {

        }
    }

    internal static class MileageCategoryExtension
    {
        internal static VehicleJourneyRateThresholdRate Cast<TRes>(
    this FuelRate fuelRate)
    where TRes : VehicleJourneyRateThresholdRate
        {
            return new VehicleJourneyRateThresholdRate
            {
                MileageThresholdRateId = fuelRate.MileageThresholdRateId,
                MileageThresholdId = fuelRate.MileageThresholdId,
                VehicleEngineTypeId = (int)fuelRate.VehicleEngineTypeId,
                RatePerUnit = fuelRate.RatePerUnit,
                AmountForVat = fuelRate.AmountForVat,
                CreatedOn = fuelRate.CreatedOn,
                CreatedBy = fuelRate.CreatedById,
                ModifiedOn = fuelRate.ModifiedOn,
                ModifiedBy = fuelRate.ModifiedById
            };
        }

        internal static cMileageThreshold Cast<TRes>(
            this Threshold mileageThreshold)
            where TRes : cMileageThreshold
        {
            return new cMileageThreshold(
                mileageThreshold.MileageThresholdId,
                mileageThreshold.MileageDateId,
                mileageThreshold.RangeValue1,
                mileageThreshold.RangeValue2,
                (SpendManagementLibrary.RangeType)mileageThreshold.RangeType,
                mileageThreshold.Passenger1Rate,
                mileageThreshold.PassengerXRate,
                mileageThreshold.CreatedOn,
                mileageThreshold.CreatedById,
                mileageThreshold.ModifiedOn,
                mileageThreshold.ModifiedById,
                mileageThreshold.HeavyBulkyEquipmentRate);
        }

        internal static cMileageDaterange Cast<TRes>(
            this DateRange mileageDateRange)
            where TRes : cMileageDaterange
        {
            return new cMileageDaterange(
                mileageDateRange.MileageDateId,
                mileageDateRange.MileageCategoryId,
                mileageDateRange.DateValue1,
                mileageDateRange.DateValue2,
                new List<cMileageThreshold>(),
                (DateRangeType)Enum.Parse(typeof(DateRangeType), mileageDateRange.DateRangeType.ToString()),
                mileageDateRange.CreatedOn,
                mileageDateRange.CreatedById,
                mileageDateRange.ModifiedOn,
                mileageDateRange.ModifiedById);
        }

        /// <summary>
        /// Creates an internal mileage category object from external object data
        /// </summary>
        /// <typeparam name="TRes">Type being converted to</typeparam>
        /// <param name="employeeId">Employee Id</param>
        /// <returns>Converted mileage category</returns>
        internal static cMileageCat Cast<TRes>(
            this MileageCategory mileageCategory) where TRes : cMileageCat
        {
            if (mileageCategory == null)
            {
                return null;
            }

            MileageUOM mileageUom = MileageUOM.Mile;
            Enum.TryParse(mileageCategory.UnitOfMeasure.ToString(), out mileageUom);

            List<cMileageDaterange> MileageDateranges = null;

            if (mileageCategory.DateRanges != null)
            {
                MileageDateranges = mileageCategory.DateRanges.Select(date => date.Cast<cMileageDaterange>()).ToList();
            }

            return new cMileageCat(
                mileageCategory.MileageCategoryId,
                mileageCategory.Label,
                mileageCategory.Comment,
                (ThresholdType)mileageCategory.ThresholdType,
                mileageCategory.CalculateNewJourneyTotal,
                MileageDateranges,
                mileageUom,
                mileageCategory.CatValid,
                mileageCategory.CatValidComment,
                mileageCategory.Currency,
                mileageCategory.CreatedOn,
                mileageCategory.CreatedById,
                mileageCategory.ModifiedOn,
                mileageCategory.ModifiedById,
                mileageCategory.NhsMileageCode,
                mileageCategory.StartEngineSize,
                mileageCategory.EndEngineSize,
                mileageCategory.FinancialYearId);
        }

        /// <summary>
        /// Converts the internal object to an external object
        /// </summary>
        /// <typeparam name="TRes">Mileage Category</typeparam>
        /// <param name="mileageCat">Current internal object</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>Returns the converted external object</returns>
        internal static TRes Cast<TRes>(this cMileageCat mileageCat, ICurrentUserBase currentUser) where TRes : MileageCategory, new()
        {
            if (mileageCat == null)
            {
                return null;
            }

            var rates = VehicleJourneyRateThresholdRate.GetAll(currentUser);
            var none = new VehicleJourneyRateThresholdRate { RatePerUnit = 0, AmountForVat = 0 };

            return new TRes
            {
                MileageCategoryId = mileageCat.mileageid,
                Label = mileageCat.carsize,
                AccountId = currentUser.AccountID,
                CalculateNewJourneyTotal = mileageCat.calcmilestotal,
                Comment = mileageCat.comment,
                Currency = mileageCat.currencyid,
                CatValid = mileageCat.catvalid,
                CatValidComment = mileageCat.catvalidcomment,
                FinancialYearId = mileageCat.FinancialYearID,
                NhsMileageCode = mileageCat.UserRatestable,
                StartEngineSize = mileageCat.UserRatesFromEngineSize,
                EndEngineSize = mileageCat.UserRatesToEngineSize,
                ThresholdType = (SpendManagementApi.Common.Enums.ThresholdType)mileageCat.thresholdType,
                UnitOfMeasure = mileageCat.mileUom,
                CreatedById = mileageCat.createdby,
                CreatedOn = mileageCat.createdon,
                ModifiedById = mileageCat.modifiedby,
                ModifiedOn = mileageCat.modifiedon,
                DateRanges = mileageCat.dateRanges.Select(date =>
                new DateRange
                {
                    AccountId = currentUser.AccountID,
                    CreatedById = date.createdby,
                    CreatedOn = date.createdon,
                    DateRangeType = (SpendManagementApi.Common.Enums.DateRangeType)Enum.Parse(typeof(SpendManagementApi.Common.Enums.DateRangeType), ((int)date.daterangetype).ToString()),
                    DateValue1 = date.dateValue1,
                    DateValue2 = date.dateValue2,
                    MileageDateId = date.mileagedateid,
                    ModifiedOn = date.modifiedon,
                    ModifiedById = date.modifiedby,
                    Thresholds = date.thresholds.Select(threshold =>
                    {
                        var petrol = rates.FirstOrDefault(r => r.MileageThresholdId == threshold.MileageThresholdId && r.VehicleEngineTypeId == 1) ?? none;
                        var diesel = rates.FirstOrDefault(r => r.MileageThresholdId == threshold.MileageThresholdId && r.VehicleEngineTypeId == 2) ?? none;
                        var lpg = rates.FirstOrDefault(r => r.MileageThresholdId == threshold.MileageThresholdId && r.VehicleEngineTypeId == 3) ?? none;
                        var hybrid = rates.FirstOrDefault(r => r.MileageThresholdId == threshold.MileageThresholdId && r.VehicleEngineTypeId == 4) ?? none;
                        var electric = rates.FirstOrDefault(r => r.MileageThresholdId == threshold.MileageThresholdId && r.VehicleEngineTypeId == 5) ?? none;
                        var dieselEuroV = rates.FirstOrDefault(r => r.MileageThresholdId == threshold.MileageThresholdId && r.VehicleEngineTypeId == 6) ?? none;

                        return new Threshold
                        {
                            AccountId = currentUser.AccountID,
                            CreatedById = threshold.CreatedBy,
                            CreatedOn = threshold.CreatedOn,
                            HeavyBulkyEquipmentRate = threshold.HeavyBulkyEquipment,
                            MileageThresholdId = threshold.MileageThresholdId,
                            ModifiedById = threshold.ModifiedBy,
                            ModifiedOn = threshold.ModifiedOn,
                            Passenger1Rate = threshold.Passenger,
                            PassengerXRate = threshold.PassengerX,
                            RangeType = (RangeType)threshold.RangeType,
                            RangeValue1 = threshold.RangeValue1,
                            RangeValue2 = threshold.RangeValue2,
                            Rates = rates.Where(rate => rate.MileageThresholdId == threshold.MileageThresholdId)
                        };
                    }).ToList()
                }).ToList()
            };
        }

        /// <summary>
        /// Converts a MileageRequest to a API Mileage Type
        /// </summary>
        /// <typeparam name="TRes">The <see cref="MilegeCategory"></see></typeparam>
        /// <param name="request">the Request</param>
        /// <param name="currentUser">The currenct user</param>
        /// <returns></returns>
        internal static TRes Cast<TRes>(this MileageCategoryRequest request, ICurrentUserBase currentUser) where TRes : MileageCategory, new()
        {
            return new TRes
            {
                MileageCategoryId = request.MileageCategoryId,
                Label = request.Label,
                AccountId = currentUser.AccountID,
                CalculateNewJourneyTotal = request.CalculateNewJourneyTotal,
                Comment = request.Comment,
                Currency = request.Currency,
                CatValidComment = request.Comment,
                FinancialYearId = request.FinancialYearId,
                NhsMileageCode = request.NhsMileageCode,
                StartEngineSize = request.StartEngineSize,
                EndEngineSize = request.EndEngineSize,
                ThresholdType = request.ThresholdType,
                UnitOfMeasure = request.UnitOfMeasure,
                DateRanges = null
            };
        }
    }
}