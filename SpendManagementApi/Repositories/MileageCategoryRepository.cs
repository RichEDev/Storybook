namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.MobileControls;

    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;
    using Utilities;
    using Models.Requests;

    using SpendManagementApi.Common.Enums;

    using DateRangeType = SpendManagementLibrary.DateRangeType;
    using FinancialYear = SpendManagementLibrary.FinancialYears.FinancialYear;
    using RangeType = SpendManagementLibrary.RangeType;

    internal class MileageCategoryRepository : BaseRepository<MileageCategory>, ISupportsActionContext
    {
        private readonly cMileagecats _mileagecats;

        public MileageCategoryRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, group => group.MileageCategoryId, group => group.Label)
        {
            _mileagecats = this.ActionContext.MileageCategories;
        }

        public override IList<MileageCategory> GetAll()
        {
            List<int> mileageCatIds = _mileagecats.GetAllMileageIDs();
            List<cMileageCat> mileageCats = mileageCatIds.Select(cat => _mileagecats.GetMileageCatById(cat)).ToList();
            return mileageCats.Select(cat => cat.Cast<MileageCategory>(this.User)).ToList();
        }


        public override MileageCategory Get(int mileageCategoryId)
        {
            cMileageCat mileageCat = _mileagecats.GetMileageCatById(mileageCategoryId);
            return mileageCat.Cast<MileageCategory>(this.User);
        }

        /// <summary>
        /// Gets a list of <see cref="MileageCategory"> MileageCategory</see> for the supplied vehicleId/subcatId/
        /// If subcat has enforced mileage category then return this, not the vehicle's mileage categories
        /// </summary>
        /// <param name="vehicleId">
        /// The vehicle Id 
        /// </param>
        /// <param name="subCatId">
        /// The Id of the subcat for the expense 
        /// </param>
        /// <param name="expenseDate">
        /// The date of the expense
        /// </param>
        /// <param name="employeeId">
        /// The employee Id.
        /// </param>
        /// <returns>
        /// A list of <see cref="MileageCategoryBasic">MileageCategoryBasic</see>
        /// </returns>
        public List<MileageCategoryBasic> GetMileageCategoriesForVehicleOnExpense(int vehicleId, int subCatId, DateTime expenseDate, int employeeId = 0)
        {
            List<MileageCategoryBasic> mileageCategories = new List<MileageCategoryBasic>();
            var mileageCat = new MileageCategoryBasic();

            if (subCatId > 0)
            {
                var empId = employeeId > 0 ? employeeId : this.User.EmployeeID;

                var subCatRepo = new ExpenseSubCategoryRepository(User, ActionContext);
                var subCat = subCatRepo.Get(subCatId);

                //check if subcat has enforced mileage category
                if (subCat.MileageCategory.HasValue)
                {
                    cMileageCat mileageCategory = this._mileagecats.GetMileageCatById(subCat.MileageCategory.Value);

                    mileageCat = this.CreateMileageBasic(vehicleId, subCatId, expenseDate, mileageCategory, empId);

                    mileageCategories.Add(mileageCat);
                }
                else
                {
                    var mileageCats = this._mileagecats.GetByVehicleId(
                        vehicleId,
                        this.User,
                        this.ActionContext.EmployeeCars);

                    foreach (cMileageCat category in mileageCats)
                    {
                        mileageCat = this.CreateMileageBasic(vehicleId, subCatId, expenseDate, category, empId);
                        mileageCategories.Add(mileageCat);
                    }
                }

                return mileageCategories;
            }

            return null;
        }

        /// <summary>
        /// Saves a <see cref="MileageCategory">MileageCategory</see> 
        /// </summary>
        /// <param name="MileageCategoryRequest">The MileageCategoryRequest</param>
        /// <returns>The <see cref="MileageCategory">MileageCategory</see></returns>
        public MileageCategory SaveMileageCategory(MileageCategoryRequest request)
        {
            if (request.MileageCategoryId != 0)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists, ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            var mileageCategory = request.Cast<MileageCategory>(this.User);

            base.Add(mileageCategory);
            return SaveUpdateMileageCategory(mileageCategory);
        }

        /// <summary>
        /// Updates a <see cref="MileageCategory">MileageCategory</see> 
        /// </summary>
        /// <param name="MileageCategoryRequest">The MileageCategoryRequest</param>
        /// <returns>The <see cref="MileageCategory">MileageCategory</see></returns>
        public MileageCategory UpdateMileageCategory(MileageCategoryRequest request)
        {   
            DoesMileageCategoryExist(request.MileageCategoryId);

            var mileageCategory = request.Cast<MileageCategory>(this.User);

            mileageCategory.ModifiedById = User.EmployeeID;
            mileageCategory.ModifiedOn = DateTime.Now;
        
            return SaveUpdateMileageCategory(mileageCategory);
        }

        private MileageCategory DoesMileageCategoryExist(int mileageCategoryId)
        {
            var mileageCat = Get(mileageCategoryId);

            if (mileageCat == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorANonExistentX, "MileageCategory"),
                    string.Format(ApiResources.ApiErrorRecordDoesntExistMessage));
            }

            return mileageCat;
        }

        private MileageCategory SaveUpdateMileageCategory(MileageCategory mileageCategory)
        {
            cMileageCat mileageCat = mileageCategory.Cast<cMileageCat>();
            int mileageCatId = _mileagecats.saveVehicleJourneyRate(mileageCat);

            if (mileageCatId == -1)
            {
                throw new ApiException(ApiResources.MileageCategoriesWithLabelAlreadyExists, ApiResources.MileageCategoriesWithLabelAlreadyExistsMessage);   
            }

            return this.Get(mileageCatId);
        }

        /// <summary>
        /// Saves the supplied list of <see cref="DateRange">DateRanges</see>
        /// </summary>
        /// <param name="dateRanges">The list of <see cref="DateRange">DateRange</see>s</param>
        /// <param name="mileageCategoryId">The Id of the MileageCategory the DateRanges belongs to</param>
        /// <returns>The outcome of the event</returns>
        public int SaveMileageCategoryDateRanges(List<DateRange> dateRanges, int mileageCategoryId)
        {
            if (!DoesEmployeeHaveAccess())
            {
                throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.MileageCategoriesDateRangeAddPermissionError);
            }

            DoesMileageCategoryExist(mileageCategoryId);
            ValidateDateRangesForSave(dateRanges, mileageCategoryId);

            foreach (var dateRange in dateRanges)
            {
                dateRange.CreatedById = User.EmployeeID;
                dateRange.CreatedOn = DateTime.UtcNow;
                SaveUpdateMileageCategoryDateRanges(dateRange);
            }

            return 1;
        }

        /// <summary>
        /// Updates the supplied list of <see cref="DateRange">DateRanges</see>
        /// </summary>
        /// <param name="dateRanges">The list of <see cref="DateRange">DateRange</see>s</param>
        /// <param name="mileageCategoryId">The Id of the MileageCategory the DateRanges belongs to</param>
        /// <returns>The outcome of the event</returns>
        public int UpdateMileageCategoryDateRanges(List<DateRange> dateRanges, int mileageCategoryId)
        {
            if (!DoesEmployeeHaveAccess())
            {
                throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.MileageCategoriesDateRangeEditPermissionError);
            }

            MileageCategory mileagecategory = DoesMileageCategoryExist(mileageCategoryId);
            ValidateDateRangesForUpdate(dateRanges, mileagecategory);

            foreach (var dateRange in dateRanges)
            {
                dateRange.ModifiedById = User.EmployeeID;
                dateRange.ModifiedOn = DateTime.UtcNow;
                SaveUpdateMileageCategoryDateRanges(dateRange);
            }

            return 1;
        }

        private void ValidateDateRangesForSave(IEnumerable<DateRange> dateRanges, int mileageCategoryId)
        { 
            foreach (var dateRange in dateRanges)
            {
                if (mileageCategoryId != dateRange.MileageCategoryId)
                {
                    throw new ApiException(ApiResources.MileageCategoriesDateRangeIdMismatch, ApiResources.MileageCategoriesDateRangeIdMismatchMessage);
                }
          
                if (dateRange.MileageDateId > 0)
                {                
                    throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists, ApiResources.ApiErrorRecordAlreadyExistsMessage);   
                }

                ValidateDateRanges(dateRange);
            }
        }

        private void ValidateDateRangesForUpdate(IEnumerable<DateRange> dateRanges, MileageCategory mileageCat)
        {

            foreach (var dateRange in dateRanges)
            {             
                cMileageDaterange range = _mileagecats.getMileageDateRangeById(mileageCat.Cast<cMileageCat>(), dateRange.MileageDateId);

                if (range == null)
                {        
                    throw new ApiException(ApiResources.MileageCategoriesDateRangeNotFound, ApiResources.MileageCategoriesDateRangeNotFoundMessage);
                }

                ValidateDateRanges(dateRange);
            }
        }

        private void ValidateDateRanges(DateRange dateRange)
        {
            var rangeType = (DateRangeType)Enum.Parse(typeof(DateRangeType), ((int)dateRange.DateRangeType).ToString());
            DateTime? value1 = dateRange.DateValue1;
            DateTime? value2 = dateRange.DateValue2;

            switch (rangeType)
            {
                case DateRangeType.AfterOrEqualTo:
                case DateRangeType.Before:

                    if (value1 == null)
                    {
                        throw new ApiException(ApiResources.MileageCategoriesDateValidationFailed, ApiResources.MileageCategoriesDateValue1NullMessage);
                    }

                    break;

                case DateRangeType.Between:

                    if (value1 == null || value2 == null)
                    {
                        throw new ApiException(ApiResources.MileageCategoriesDateValidationFailed, ApiResources.MileageCategoriesDateValue1And2NullMessage);

                    }

                    break;
            }

            string validationMessage = _mileagecats.mileageDateRangeExists(
                dateRange.MileageCategoryId,
                dateRange.MileageDateId,
                ref value1,
                ref value2,
                ref rangeType,
                -1);

            if (validationMessage.Trim().Length > 0)
            {
                throw new ApiException(ApiResources.MileageCategories_InvalidDateRange, validationMessage);
            }
        }

        private void SaveUpdateMileageCategoryDateRanges(DateRange dateRange)
        {
            int mileageDateId = _mileagecats.saveDateRange(dateRange.Cast<cMileageDaterange>(), dateRange.MileageCategoryId);

            if (mileageDateId == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.MileageCategories_DateRangeNotSaved);
            }
        }

        /// <summary>
        /// Saves the supplied list of <see cref="Threshold">Threshold</see>s
        /// </summary>
        /// <param name="thresholds">The list of <see cref="Threshold">Threshold</see>s</param>
        /// <param name="mileageCategoryId">The Id of the MileageCategory the DateRanges belongs to</param>
        /// <param name="mileageDateRangeId">The Id of the DateRangeId the Thresholds belongs to</param>
        /// <returns>The outcome of the save action</returns>
        public int SaveDateRangeThresholds(List<Threshold> thresholds, int mileageCategoryId, int mileageDateRangeId)
        {
            if (!DoesEmployeeHaveAccess())
            {
                throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.MileageCategoriesThresholdAddPermissionError);
            }

            MileageCategory mileageCat = DoesMileageCategoryExist(mileageCategoryId);

            DoesDateRangeExist(mileageDateRangeId, mileageCat);
            ValidateThresholdForSave(thresholds, mileageCategoryId, mileageDateRangeId);

            foreach (var threshold in thresholds)
            {
                threshold.CreatedById = User.EmployeeID;
                threshold.CreatedOn = DateTime.UtcNow;     
                _mileagecats.saveThreshold(threshold.MileageCategoryId, threshold.Cast<cMileageThreshold>());
            }

            return 1;
        }

        /// <summary>
        /// Saves the supplied <see cref="FuelRate">FuelRate</see>s
        /// </summary>
        /// <param name="fuelRate">The Mileage Category Threshold <see cref="FuelRate">FuelRate</see>s</param>
        /// <param name="mileageThresholdId">The Id of the Mileage Category Thresholds the DateRange belongs to</param>
        /// <returns></returns>
        public int SaveFuelRate(FuelRate fuelRate, int mileageThresholdId)
        {
            if (!DoesEmployeeHaveAccess())
            {
                throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.MileageCategoriesThresholdAddPermissionError);
            }

            ValidateFuelRateForSave(fuelRate, mileageThresholdId);

            fuelRate.CreatedById = User.EmployeeID;
            fuelRate.CreatedOn = DateTime.UtcNow;
            fuelRate.Cast<VehicleJourneyRateThresholdRate>().Save(User);

            return 1;
        }

        /// <summary>
        /// Updates the supplied list of <see cref="Threshold">Threshold</see>s
        /// </summary>
        /// <param name="thresholds">The list of <see cref="Threshold">Threshold</see>s</param>
        /// <param name="mileageCategoryId">The Id of the MileageCategory the DateRanges belongs to</param>
        /// <param name="mileageDateRangeId">The Id of the DateRangeId the Thresholds belongs to</param>
        /// <returns>The outcome of the save action</returns>
        public int UpdateDateRangeThresholds(List<Threshold> thresholds, int mileageCategoryId, int mileageDateRangeId)
        {
            if (!DoesEmployeeHaveAccess())
            {
                throw new ApiException(ApiResources.HttpStatusCodeForbidden, ApiResources.MileageCategoriesThresholdEditPermissionError);
            }

            MileageCategory mileageCat = DoesMileageCategoryExist(mileageCategoryId);
            DoesDateRangeExist(mileageDateRangeId, mileageCat);
            ValidateThresholdForUpdate(thresholds, mileageCategoryId, mileageDateRangeId, mileageCat);

            foreach (var threshold in thresholds)
            {
                threshold.ModifiedById = User.EmployeeID;
                threshold.ModifiedOn = DateTime.Now;
                _mileagecats.saveThreshold(threshold.MileageCategoryId, threshold.Cast<cMileageThreshold>());

            }

            return 1;
        }

        private void DoesDateRangeExist(int mileageDateRangeId, MileageCategory mileageCategory)
        {
            cMileageDaterange mileageDateRange = _mileagecats.getMileageDateRangeById(mileageCategory.Cast<cMileageCat>(), mileageDateRangeId);

            if (mileageDateRange == null)
            {
                throw new ApiException(string.Format(ApiResources.ApiErrorANonExistentX, "Mileage Date Range"), string.Format(ApiResources.ApiErrorRecordDoesntExistMessage));
            }
        }

        private void ValidateThresholdForSave(IEnumerable<Threshold> thresholds, int mileageCategoryId, int mileageDateRangeId)
        {

            foreach (var threshold in thresholds)
            {

                if (threshold.MileageThresholdId > 0)
                {
                    throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists, ApiResources.ApiErrorRecordAlreadyExistsMessage);
                }

                if (mileageCategoryId != threshold.MileageCategoryId)
                {
                    throw new ApiException(ApiResources.MileageCategoriesThresholdIdMismatch, ApiResources.MileageCategoriesThresholdIdMismatchMessage);
                }  
                
                if (mileageDateRangeId != threshold.MileageDateId)
                {

                    throw new ApiException(ApiResources.MileageCategoriesDateRangeThresholdIdMismatch, ApiResources.MileageCategoriesDateRangeThresholdIdMismatchMessage);
                }
   
                ValidateThreshold(threshold);
            }

        }

        private void ValidateFuelRateForSave(FuelRate fuelRate, int mileageThresholdId)
        {
            if (fuelRate.MileageThresholdRateId > 0)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists, ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (mileageThresholdId != fuelRate.MileageThresholdId)
            {
                throw new ApiException(ApiResources.MileageCategoriesFuelRateThresholdIdMismatch, ApiResources.MileageCategoriesFuelRateThresholdIdMismatchMessage);
            }
        }

        private void ValidateThresholdForUpdate(IEnumerable<Threshold> thresholds, int mileageCategoryId, int mileageDateRangeId, MileageCategory mileageCategory)
        {
            var daterange = _mileagecats.getMileageDateRangeById(mileageCategory.Cast<cMileageCat>(), mileageDateRangeId);

            foreach (var threshold in thresholds)
            {
                if (mileageCategoryId != threshold.MileageCategoryId)
                {
                    throw new ApiException(ApiResources.MileageCategoriesThresholdIdMismatch, ApiResources.MileageCategoriesThresholdIdMismatchMessage);
                }

                if (daterange.mileagedateid != threshold.MileageDateId)
                {
                    throw new ApiException(ApiResources.MileageCategoriesDateRangeThresholdIdMismatch, ApiResources.MileageCategoriesDateRangeThresholdIdMismatchMessage);
                }

                ValidateThreshold(threshold);
            }
        }

        private void ValidateThreshold(Threshold threshold)
        {
            SpendManagementLibrary.RangeType range = (SpendManagementLibrary.RangeType)threshold.RangeType;
            decimal? rangeValue1 = threshold.RangeValue1;
            decimal? rangeValue2 = threshold.RangeValue2;

            switch (range)
            {
                case RangeType.GreaterThanOrEqualTo:
                case RangeType.LessThan:

                    if (rangeValue1 == null)
                    {
                        throw new ApiException(ApiResources.MileageCategoriesRangeValue1ValidationFailed, ApiResources.MileageCategoriesRangeValue1ValidationFailedMessage);
                    }

                    break;

                case RangeType.Between:

                    if (rangeValue1 == null || rangeValue2 == null)
                    {
                        throw new ApiException(ApiResources.MileageCategoriesRangeValue1ValidationFailed, ApiResources.MileageCategoriesRangeValue1And2NullMessage);
                    }

                    break;
            }

           string message = _mileagecats.mileageThresholdExists(threshold.MileageCategoryId,threshold.MileageDateId,threshold.MileageThresholdId,ref range,ref rangeValue1, ref rangeValue2, -1);
      
            if (message.Trim().Length > 0)
            {
                throw new ApiException(ApiResources.MileageCategories_InvalidThresholdRange, message);
            }
        }

        private void UpdateMileageDateRangeWithThresholds(MileageCategory mileageCategory, int mileageCatId)
        {
            if (mileageCatId == -1)
            {
                throw new ApiException(ApiResources.MileageCategories_InvalidCarSize, ApiResources.MileageCategories_InvalidCarSizeMessage);
            }

            if (mileageCategory.DateRanges.Count > 0)
            {
                mileageCategory.DateRanges.ForEach(
                    date =>
                        {
                            // Save date range
                            DateRange dateRange = date;
                            dateRange.MileageCategoryId = mileageCatId;
                            if (dateRange.MileageDateId > 0)
                            {
                                dateRange.ModifiedById = User.EmployeeID;
                                dateRange.ModifiedOn = DateTime.UtcNow;
                            }

                            dateRange.CreatedById = mileageCategory.CreatedById;
                            dateRange.CreatedOn = mileageCategory.CreatedOn;

                            DateTime? value1 = dateRange.DateValue1;
                            DateTime? value2 = dateRange.DateValue2;
                            DateRangeType rangeType = (DateRangeType)Enum.Parse(typeof(DateRangeType),((int)dateRange.DateRangeType).ToString());

                            //The last parameter isnt used - Passing -1 instead
                            string message1 = _mileagecats.mileageDateRangeExists(
                                dateRange.MileageCategoryId,
                                dateRange.MileageDateId,
                                ref value1,
                                ref value2,
                                ref rangeType,
                                -1);

                            if (message1.Trim().Length > 0)
                            {
                                throw new ApiException(ApiResources.MileageCategories_InvalidDateRange, message1);
                            }

                            int mileageDateId = _mileagecats.saveDateRange(
                                dateRange.Cast<cMileageDaterange>(), mileageCatId);

                            if (mileageDateId == -1)
                            {
                                throw new ApiException(
                                    ApiResources.ApiErrorSaveUnsuccessful, ApiResources.MileageCategories_DateRangeNotSaved);
                            }

                            if (mileageDateId > 0)
                            {
                                cMileageCat originalMileageCategory = _mileagecats.GetMileageCatById(mileageCatId);
                                cMileageDaterange originalDateRange = originalMileageCategory.dateRanges[0];

                                originalDateRange.thresholds.ForEach(
                                    threshold =>
                                    _mileagecats.deleteMileageThreshold(threshold.MileageThresholdId, mileageCatId));

                                dateRange.Thresholds.ForEach(
                                    threshold =>
                                        {
                                            threshold.MileageThresholdId = 0;
                                            threshold.MileageDateId = mileageDateId;
                                            threshold.MileageCategoryId = dateRange.MileageCategoryId;
                                            threshold.CreatedById = mileageCategory.CreatedById;
                                            threshold.CreatedOn = mileageCategory.CreatedOn;
                                     
                                            SpendManagementLibrary.RangeType range = (SpendManagementLibrary.RangeType)threshold.RangeType;
                                            decimal? rangeValue1 = threshold.RangeValue1;
                                            decimal? rangeValue2 = threshold.RangeValue2;
                                            string message =
                                                _mileagecats.mileageThresholdExists(
                                                    threshold.MileageCategoryId,
                                                    threshold.MileageDateId,
                                                    threshold.MileageThresholdId,
                                                    ref range,
                                                    ref rangeValue1,
                                                    ref rangeValue2,
                                                    -1);
                                            if (message.Trim().Length > 0)
                                            {
                                                throw new ApiException(
                                                    ApiResources.MileageCategories_InvalidThresholdRange, message);
                                            }
                                            _mileagecats.saveThreshold(mileageCatId, threshold.Cast<cMileageThreshold>());
                                        });
                            }
                        });
            }
        }

        public override MileageCategory Delete(int mileageCategoryId)
        {
            MileageCategory mileageCategory = Get(mileageCategoryId);

            if (mileageCategory == null)
            {
                throw new ApiException("Invalid mileage category id", "No data available for specified mileage category id");
            }
                
            _mileagecats.deleteMileageCat(mileageCategoryId);
            return this.Get(mileageCategoryId);
        }

        /// <summary>
        /// Only updates the archive status of the record and the expense sub category vat rates/ percentages
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public override MileageCategory Update(MileageCategory mileageCategory)
        {
            base.Update(mileageCategory);

            cMileageCat mileageCat = _mileagecats.GetMileageCatById(mileageCategory.MileageCategoryId);

            if (mileageCategory == null)
            {
                throw new ApiException("Invalid Mileage Category Id", "No record available for specified mileage category id");
            }

            int mileageCatId = _mileagecats.saveVehicleJourneyRate(mileageCategory.Cast<cMileageCat>());
            mileageCategory.CreatedById = mileageCat.createdby;
            mileageCategory.CreatedOn = mileageCat.createdon;

            UpdateMileageDateRangeWithThresholds(mileageCategory, mileageCatId);
            
            return this.Get(mileageCatId);
        }

        private bool DoesEmployeeHaveAccess()
        {
           if((User.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VehicleJourneyRateCategories, true)) || 
               (User.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.VehicleJourneyRateCategories, true)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The get comment.
        /// </summary>
        /// <param name="vehicleId">
        /// The vehicle id.
        /// </param>
        /// <param name="subCatId">
        /// The sub cat id.
        /// </param>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <param name="mileageId">
        /// The mileage id.
        /// </param>
        /// <param name="employeeId">
        /// The employeeId
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public string[] GetComment(int vehicleId, int subCatId, DateTime expenseDate, int mileageId, int employeeId)
        {
            return this._mileagecats.GetComment(
                mileageId.ToString(),
                this.User.AccountID,
                employeeId,
                vehicleId,
                mileageId,
                expenseDate,
                subCatId);
        }

        /// <summary>
        /// Create a <see cref="MileageCategoryBasic">MileageCategoryBasic</see>
        /// </summary>
        /// <param name="vehicleId">
        /// The vehicle id.
        /// </param>
        /// <param name="subCatId">
        /// The sub cat id.
        /// </param>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <param name="category">
        /// The <see cref="cMileageCat">cMileageCat</see>
        /// </param>
        /// <param name="employeeId">The employeeId</param>
        /// <returns>
        /// The <see cref="MileageCategoryBasic">MileageCategoryBasic</see>
        /// </returns>
        private MileageCategoryBasic CreateMileageBasic(
            int vehicleId,
            int subCatId,
            DateTime expenseDate,
            cMileageCat category, 
            int employeeId)
        {

            var mileageCat = new MileageCategoryBasic();
            var comment = this.GetComment(vehicleId, subCatId, expenseDate, category.mileageid, employeeId);

            mileageCat.MileageCategoryId = category.mileageid;
            mileageCat.UnitOfMeasure = (MileageUom)category.mileUom;
            mileageCat.Label = category.carsize;
            mileageCat.TooltipInfo = comment[1];
            mileageCat.HomeToOfficeDeductionRules = comment[2];

            return mileageCat;
        }
    }
}