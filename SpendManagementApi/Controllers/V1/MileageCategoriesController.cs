namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Attributes;
    using Interfaces;
    using Models.Common;
    using Models.Requests;
    using Models.Responses;
    using Models.Types;
    using Repositories;
    using Utilities;

    /// <summary>
    /// Manages operations on <see cref="MileageCategory">Mileage Categories</see>.
    /// </summary>
    [RoutePrefix("MileageCategories")]
    [Version(1)]
    public class MileageCategoriesV1Controller : BaseApiController<MileageCategory>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="MileageCategory">MileageCategories</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="MileageCategory">MileageCategories</see> in the system.
        /// </summary>
        /// <returns>A GetMileageCategoriesResponse containing all the <see cref="MileageCategory">MileageCategories</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.AccessRoles, AccessRoleType.View)]
        public GetMileageCategoriesResponse GetAll()
        {
            return this.GetAll<GetMileageCategoriesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="MileageCategory">MileageCategory</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="MileageCategory">MileageCategory</see> to get.</param>
        /// <returns>A MileageCategoryResponse containing the <see cref="MileageCategory">MileageCategory</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.View)]
        public MileageCategoryResponse Get([FromUri] int id)
        {
            return this.Get<MileageCategoryResponse>(id);
        }

        /// <summary>
        /// Gets a list of <see cref="GetMileageCategoriesResponse">GetMileageCategoriesResponse</see> for the supplied vehicleId/subcatId
        /// If subcat has enforced mileage category then return this, not the vehicle's mileage categories
        /// </summary>
        /// <param name="vehicleId">
        /// The Id of the vehicle
        /// </param>
        /// <param name="subCatId">
        /// The Id of the subcat for the expense 
        /// </param>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <returns>
        /// A <see cref="MileageCategoryBasicResponse">MileageCategoryBasicResponse</see>
        /// </returns>
        [HttpGet]
        [Route("GetMileageCategoriesForVehicleOnExpense")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MileageCategoryBasicResponse GetMileageCategoriesForVehicleOnExpense([FromUri] int vehicleId, int subCatId, DateTime expenseDate)
        {         
            var response = this.InitialiseResponse<MileageCategoryBasicResponse>();
            response.List = ((MileageCategoryRepository)this.Repository).GetMileageCategoriesForVehicleOnExpense(vehicleId, subCatId, expenseDate);
            return response;
        }

        /// <summary>
        /// Gets a list of <see cref="GetMileageCategoriesResponse">GetMileageCategoriesResponse</see> for the supplied criteria, when calling as an approver
        /// If subcat has enforced mileage category then return this, not the vehicle's mileage categories
        /// </summary>
        /// <param name="vehicleId">
        /// The Id of the vehicle
        /// </param>
        /// <param name="subCatId">
        /// The Id of the subcat for the expense 
        /// </param>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <param name="employeeId">
        /// The employee Id the claim is associated with.
        /// </param>
        /// <returns>
        /// A <see cref="MileageCategoryBasicResponse">MileageCategoryBasicResponse</see>
        /// </returns>
        [HttpGet]
        [Route("GetMileageCategoriesForVehicleOnExpenseAsApprover"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MileageCategoryBasicResponse GetMileageCategoriesForVehicleOnExpenseAsApprover([FromUri] int vehicleId, int subCatId, DateTime expenseDate, int employeeId)
        {
            var response = this.InitialiseResponse<MileageCategoryBasicResponse>();
            response.List = ((MileageCategoryRepository)this.Repository).GetMileageCategoriesForVehicleOnExpense(vehicleId, subCatId, expenseDate, employeeId);
            return response;
        }

        /// <summary>
        /// Saves a <see cref="MileageCategory">MileageCategory</see>.
        /// </summary>
        /// <param name="request">The <see cref="MileageCategory">MileageCategory</see></param>
        /// <returns>A MileageCategoryResponse containing the saved <see cref="MileageCategory">MileageCategory</see>.</returns>
        [HttpPost]
        [Route("SaveMileageCategory")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Add)]
        public MileageCategoryResponse SaveMileageCategory([FromBody] MileageCategoryRequest request)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesNullRequest, ApiResources.MileageCategoriesNullRequestMessage);
            }
            var response = this.InitialiseResponse<MileageCategoryResponse>();
            response.Item = ((MileageCategoryRepository)this.Repository).SaveMileageCategory(request);
            return response;
        }

        /// <summary>
        /// Updates a <see cref="MileageCategory">MileageCategory</see>.
        /// </summary>
        /// <param name="request">The <see cref="MileageCategory">MileageCategory</see></param>
        /// <returns>A MileageCategoryResponse containing the updated <see cref="MileageCategory">MileageCategory</see>.</returns>
        [HttpPut]
        [Route("UpdateMileageCategory")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Edit)]
        public MileageCategoryResponse UpdateMileageCategory([FromBody] MileageCategoryRequest request)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesNullRequest, ApiResources.MileageCategoriesNullRequestMessage);
            }
          
            var response = this.InitialiseResponse<MileageCategoryResponse>();
             response.Item = ((MileageCategoryRepository)this.Repository).UpdateMileageCategory(request);
            return response;
        }


        /// <summary>
        /// Saves a list of <see cref="DateRange">DateRange</see>s for the supplied MileageCategoryId.
        /// </summary>
        /// <param name="request">The <see cref="MileageDateRangeRequest">MileageDateRangeRequest</see></param>
        /// <param name="mileageCategoryId">The Id of the <see cref="MileageCategory">MileageCategory</see> the DateRanges belong to.</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse.</see> The outcome of the action, where 1 incidicates success</returns>
        [HttpPost]
        [Route("SaveMileageCategoryDateRanges")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Add)]
        public NumericResponse SaveMileageCategoryDateRanges([FromBody] MileageDateRangeRequest request, [FromUri] int mileageCategoryId)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesDateRangeNullRequest, ApiResources.MileageCategoriesDateRangeNullRequestMessage);
            }

            var response = this.InitialiseResponse<NumericResponse>();
            
            response.Item = ((MileageCategoryRepository)this.Repository).SaveMileageCategoryDateRanges(request.DateRanges, mileageCategoryId);
            return response;
        }

        /// <summary>
        /// Updates a list of <see cref="DateRange">DateRange</see>s for the supplied MileageCategoryId.
        /// </summary>
        /// <param name="request">The <see cref="MileageDateRangeRequest">MileageDateRangeRequest</see></param>
        /// <param name="mileageCategoryId">The Id of the <see cref="MileageCategory">MileageCategory</see> the DateRanges belong to.</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse.</see> The outcome of the action, where 1 incidicates success</returns>
        [HttpPut]
        [Route("UpdateMileageCategoryDateRanges")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Edit)]
        public NumericResponse UpdateMileageCategoryDateRanges(MileageDateRangeRequest request, [FromUri] int mileageCategoryId)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesDateRangeNullRequest, ApiResources.MileageCategoriesDateRangeNullRequestMessage);
            }

            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((MileageCategoryRepository)this.Repository).UpdateMileageCategoryDateRanges(request.DateRanges, mileageCategoryId);
            return response;
        }

        /// <summary>
        /// Saves a list of <see cref="Threshold">Threshold</see>s for the supplied DateRangeId and MileageCategoryId.
        /// </summary>
        /// <param name="request">The <see cref="MileageThresholdRequest">MileageThresholdRequest</see></param>
        /// <param name="mileageCategoryId">The Id of the parent <see cref="MileageCategory">MileageCategory</see></param>
        /// <param name="dateRangeId">The Id of the <see cref="DateRange">DateRange</see> the Thresholds belong to</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse.</see> The outcome of the action, where 1 incidicates success</returns>
        [HttpPost]
        [Route("SaveDateRangeThresholds")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Add)]
        public NumericResponse SaveDateRangeThresholds(MileageThresholdRequest request, [FromUri] int mileageCategoryId, int dateRangeId)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesDateRangeThresholdNullRequest, ApiResources.MileageCategoriesDateRangeThresholdNullRequestMessage);
            }

            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((MileageCategoryRepository)this.Repository).SaveDateRangeThresholds(request.Thresholds, mileageCategoryId,dateRangeId);
            return response;
        }

        /// <summary>
        /// Saves a list of <see cref="FuelRate">FuelRAte</see>s for the supplied mileageThresholdId.
        /// </summary>
        /// <param name="request">The <see cref="ThresholdFuelRateRequest">ThresholdFuelRateRequest</see></param>
        /// <param name="mileageThresholdId">The Id of the <see cref="Threshold">Threshold</see> the FuelRates belong to</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse.</see> The outcome of the action, where 1 incidicates success</returns>
        [HttpPost]
        [Route("SaveThresholdFuelRate")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Add)]
        public NumericResponse SaveFuelRate(ThresholdFuelRateRequest request, [FromUri] int mileageThresholdId)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesThresholdFuelRateNullRequest, ApiResources.MileageCategoriesThresholdFuelRateNullRequestMessage);
            }

            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((MileageCategoryRepository)this.Repository).SaveFuelRate(request.FuelRate, mileageThresholdId);
            return response;
        }

        /// <summary>
        /// Updates a list of <see cref="Threshold">Threshold</see>s for the supplied DateRangeId and MileageCategoryId.
        /// </summary>
        /// <param name="request">The <see cref="MileageThresholdRequest">MileageThresholdRequest</see></param>
        /// <param name="mileageCategoryId">The Id of the parent <see cref="MileageCategory">MileageCategory</see></param>
        /// <param name="dateRangeId">The Id of the <see cref="DateRange">DateRange</see> the Thresholds belong to</param>
        /// <returns>A <see cref="NumericResponse">NumericResponse.</see> The outcome of the action, where 1 incidicates success</returns>
        [HttpPut]
        [Route("UpdateDateRangeThresholds")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Edit)]
        public NumericResponse UpdateDateRangeThresholds(MileageThresholdRequest request, [FromUri] int mileageCategoryId, int dateRangeId)
        {
            if (request == null)
            {
                throw new ApiException(ApiResources.MileageCategoriesDateRangeThresholdNullRequest, ApiResources.MileageCategoriesDateRangeThresholdNullRequestMessage);
            }

            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((MileageCategoryRepository)this.Repository).UpdateDateRangeThresholds(request.Thresholds, mileageCategoryId, dateRangeId);
            return response;
        }

        /// <summary>
        /// Deletes a <see cref="MileageCategory">MileageCategory</see> with specified id
        /// </summary>
        /// <param name="id">The id of the <see cref="MileageCategory">MileageCategory</see> to delete.</param>
        /// <returns>A MileageCategoryResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.Delete)]
        public MileageCategoryResponse Delete(int id)
        {
            return this.Delete<MileageCategoryResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="MileageCategory">MileageCategories</see> specified criteria. 
        /// Available querystring parameters : 
        /// SearchOperator, MileageCategoryId, MileageCategoryName, Comment, FinancialYearId
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>FindMileageCategoriesResponse containing <see cref="MileageCategory">MileageCategories</see> matching specified criteria.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.VehicleJourneyRateCategories, AccessRoleType.View)]
        public FindMileageCategoriesResponse Find([FromUri] FindMileageCategoriesRequest criteria)
        {
            var findMileageCategoriesResponse = this.InitialiseResponse<FindMileageCategoriesResponse>();
            var conditions = new List<Expression<Func<MileageCategory, bool>>>();
            if (criteria.MileageCategoryId.HasValue)
            {
                conditions.Add(cat => cat.MileageCategoryId == criteria.MileageCategoryId);
            }

            if (!string.IsNullOrEmpty(criteria.VehicleJourneyRate))
            {
                conditions.Add(cat => cat.Label.ToLower().Contains(criteria.VehicleJourneyRate.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.Comment))
            {
                conditions.Add(cat => cat.Comment.ToLower().Contains(criteria.Comment.Trim().ToLower()));
            }

            if (criteria.FinancialYearId.HasValue)
            {
                conditions.Add(cat => cat.FinancialYearId.Value == criteria.FinancialYearId);
            }

            findMileageCategoriesResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return findMileageCategoriesResponse;
        }
    }
    
}