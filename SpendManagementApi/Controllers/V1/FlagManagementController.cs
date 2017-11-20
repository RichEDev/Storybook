namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using Repositories;
    using Models.Requests;

    /// <summary>
    /// Manages operations on <see cref="Flag">Flag</see>.
    /// </summary>
    [RoutePrefix("FlagManagement")]
    [Version(1)]
    public class FlagManagementV1Controller : BaseApiController<Flag>
    {

        /// <summary>
        /// Gets all of the available end points from the <see cref="Flag">Flag</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return Links();
        }

        /// <summary>
        /// Creates a flag
        /// </summary>
        /// <param name="flagRequest">
        /// The flag Request.
        /// </param>
        /// <returns>
        /// <see cref="FlagResponse"> FlagResponse</see> of the created flag
        /// </returns>
        [HttpPost, Route("CreateFlag")]
        [AuthAudit(SpendManagementElement.FlagsAndLimits, AccessRoleType.Add)]
        public FlagResponse CreateFlag(Flag flagRequest)
        {
            var response = this.InitialiseResponse<FlagResponse>();
            response.Item = ((FlagManagementRepository)this.Repository).CreateExpenseFlag(flagRequest);
            return response;
        }

        /// <summary>
        /// Associates item roles with a flag
        /// </summary>
        /// <param name="request">
        /// The flag <see cref="AssociateItemRolesWithFlagRequest">AssociateWithItemRolesFlagRequest</see>
        /// </param>
        /// <returns>
        /// <see cref="NumericResponse"> NumericResponse</see> of the outcome of the action
        /// </returns>
        [HttpPost, Route("AssociateItemRolesWithFlag")]
        [AuthAudit(SpendManagementElement.FlagsAndLimits, AccessRoleType.Add)]
        public NumericResponse AssociateItemRolesWithFlag(AssociateItemRolesWithFlagRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((FlagManagementRepository)this.Repository).AssociateItemRolesWithFlag(request.FlagId, request.ItemRoleIds);
            return response;
        }

        /// <summary>
        /// Associates expense items with a flag
        /// </summary>
        /// <param name="request">
        /// The flag <see cref="AssociateExpenseItemsWithFlagRequest">AssociateExpenseItemsWithFlagRequest</see>
        /// </param>
        /// <returns>
        /// <see cref="NumericResponse"> NumericResponse</see> of the outcome of the action
        /// </returns>
        [HttpPost, Route("AssociateExpenseItemsWithFlag")]
        [AuthAudit(SpendManagementElement.FlagsAndLimits, AccessRoleType.Add)]
        public NumericResponse AssociateExpenseItemsWithFlag(AssociateExpenseItemsWithFlagRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((FlagManagementRepository)this.Repository).AssociateExpenseItemsWithFlag(request.FlagId, request.ExpenseItemIds);
            return response;
        }


        /// <summary>
        /// Associates fields with a flag
        /// </summary>
        /// <param name="request">
        /// The flag <see cref="AssociateFieldsWithFlagRequest">AssociateFieldsWithFlagRequest</see>
        /// </param>
        /// <returns>
        /// <see cref="NumericResponse"> NumericResponse</see> of the outcome of the action
        /// </returns>
        [HttpPost, Route("AssociateFieldsWithFlag")]
        [AuthAudit(SpendManagementElement.FlagsAndLimits, AccessRoleType.Add)]
        public NumericResponse AssociateFieldsWithFlag(AssociateFieldsWithFlagRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((FlagManagementRepository)this.Repository).AssociateFieldsWithFlag(request.FlagId, request.FieldIds);
            return response;
        }

        /// <summary>
        /// Deletes a flag
        /// </summary>
        /// <param name="id">The id of the flag to delete</param>
        /// <returns><see cref="NumericResponse">NumericResponse</see> of the outcome of the action</returns>
        [HttpDelete, Route("DeleteFlag/{id:int}")]
        [AuthAudit(SpendManagementElement.FlagsAndLimits, AccessRoleType.Delete)]
        public NumericResponse DeleteFlag([FromUri] int id)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((FlagManagementRepository)this.Repository).DeleteExpenseFlag(id);
            return response;
        }
    }
}
