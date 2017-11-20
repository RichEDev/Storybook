using System.Collections.Generic;
namespace SpendManagementApi.Controllers.V1
{
    using System;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Models.Requests;

    /// <summary>
    /// Manages operations on <see cref="ESRAssignments">ESR</see>.
    /// </summary>
    [RoutePrefix("ESRAssignment")]
    [Version(1)]
    public class ESRAssignmentsV1Controller : BaseApiController<ESRAssignments>
    {
        #region Api Methods
        
        /// <summary>
        /// Gets all of the available end points from the <see cref="ESRAssignments">ESRAssignments</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="ESRAssignments">ESRAssignments</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.ESRElements, AccessRoleType.View)]
        public GetESRResponse GetAll()
        {
            return GetAll<GetESRResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="ESRAssignments">ESRAssignments</see>, by its Id.
        /// </summary>
        /// <param name="id">Id of the ESRAssignment to get. (The internal id for Software Europe)</param>
        /// <returns>An ESRResponse object, which will contain an <see cref="ESRAssignments">ESRAssignments</see> if one was found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ESRElements, AccessRoleType.View)]
        public ESRResponse Get([FromUri] int id)
        {
            return Get<ESRResponse>(id);
        }
        
        /// <summary>
        /// Gets a single <see cref="ESRAssignments">ESRAssignments</see>, by its assignmentNumber.
        /// </summary>
        /// <param name="assignmentNumber">assignmentNumber of the ESRAssignment to get.</param>
        /// <returns>An ESRResponse object, which will contain an <see cref="ESRAssignments">ESRAssignments</see> if one was found.</returns>
        [HttpGet, Route("ByAssignmentNumber")]
        [AuthAudit(SpendManagementElement.ESRElements, AccessRoleType.View)]
        public ESRResponse GetAssignmentByAssignmentNumber(string assignmentNumber)
        {
            var response = this.InitialiseResponse<ESRResponse>();
            response.Item = ((ESRRepository)this.Repository).GetAssignmentByAssignmentNumber(assignmentNumber);
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="ESRAssignments">ESRAssignments</see>, by its assignmentNumber.
        /// </summary>
        /// <param name="assignmentId">The assignment id of the ESR assignment to get.</param>
        /// <returns>An ESRResponse object, which will contain an <see cref="ESRAssignments">ESRAssignments</see> if one was found.</returns>
        [HttpGet, Route("ByAssignmentId")]
        [AuthAudit(SpendManagementElement.ESRElements, AccessRoleType.View)]
        public ESRResponse GetAssignmentByAssignmentId(int assignmentId)
        {
            var response = this.InitialiseResponse<ESRResponse>();
            response.Item = ((ESRRepository)this.Repository).GetAssignmentByAssignmentId(assignmentId);
            return response;
        }


        /// <summary>
        /// Gets a list of <see cref="ESRAssignmentsBasicResponse">ESRAssignmentsBasicResponse</see>, for the current user
        /// </summary>     
        /// <returns>
        /// A list of <see cref="ESRAssignmentsBasicResponse">ESRAssignmentsBasicResponse</see>
        /// </returns>
        [HttpGet, Route("GetAssignmentsforEmployee")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ESRAssignmentsBasicResponse GetAssignmentsforEmployee()
        {
            var response = this.InitialiseResponse<ESRAssignmentsBasicResponse>();
            response.EsrAssignments = ((ESRRepository)this.Repository).GetActiveAssignmentsForEmployee();
            return response;
        }

        /// <summary>
        /// Gets a list of <see cref="ESRAssignmentsBasicResponse">ESRAssignmentsBasicResponse</see>, for the specified expense date, and employee
        /// </summary>
        /// <param name="request">
        /// The <see cref="GetClaimantsEsrAssignmentsRequest">GetClaimantsEsrAssignmentsRequest</see>
        /// </param>
        /// <returns>
        /// A list of <see cref="ESRAssignmentsBasicResponse">ESRAssignmentsBasicResponse</see>
        /// </returns>
        [HttpPut, Route("GetAssignmentsforSpecifiedEmployee"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ESRAssignmentsBasicResponse GetAssignmentsforSpecifiedEmployee(GetClaimantsEsrAssignmentsRequest request)
        {
            var response = this.InitialiseResponse<ESRAssignmentsBasicResponse>();
            response.EsrAssignments = ((ESRRepository)this.Repository).GetActiveAssignmentsForEmployeeByDate(request.ExpenseDate, request.EmployeeId, request.ExpenseId);
            return response;
        }

        /// <summary>
        /// Gets a list of <see cref="ESRAssignmentsBasicResponse">ESRAssignmentsBasicResponse</see>, for the current user and expense date.
        /// </summary>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <returns>
        /// A list of <see cref="ESRAssignmentsBasicResponse">ESRAssignmentsBasicResponse</see>
        /// </returns>
        [HttpGet, Route("GetActiveAssignmentsforExpenseDate")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ESRAssignmentsBasicResponse GetActiveAssignmentsforExpenseDate(DateTime expenseDate)
        {
            var response = this.InitialiseResponse<ESRAssignmentsBasicResponse>();
            response.EsrAssignments = ((ESRRepository)this.Repository).GetActiveAssignmentsForCurrentUserByDate(expenseDate);
            return response;
        }

        /// <summary>
        /// Saves a <see cref="ESRAssignments">ESRAssignments</see>
        /// </summary>
        /// <param name="request">
        /// The <see cref="EsrAssignmentRequest">EsrAssignmentRequest</see>
        /// </param>
        /// <returns>
        /// The <see cref="ESRResponse">ESRResponse</see>
        /// </returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Add)]
        public ESRResponse Post([FromBody] EsrAssignmentRequest request)
        {
            return Post<ESRResponse>(request.EsrAssignment);
        }

        /// <summary>
        /// Deletes a <see cref="ESRAssignments">ESRAssignments</see> by its Id
        /// </summary>
        /// <param name="id">The id of the ESRAssignment to be deleted</param>
        /// <returns> A ESRResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Add)]
        public ESRResponse Delete(int id)
        {
            return this.Delete<ESRResponse>(id);
        }

        #endregion Api Methods
    }
}