namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types.ClaimableItem;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// Manages operations on <see cref="ClaimableItem">ClaimableItems</see>.
    /// </summary>
    [RoutePrefix("ClaimableItems")]
    [Version(1)]
    public class ClaimableItemsV1Controller : BaseApiController<ClaimableItem>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="ClaimableItem">ClaimableItems</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links();
        }


        /// <summary>
        /// Gets all <see cref="ClaimableItem">ClaimableItem</see> in the system.
        /// </summary>
        /// <returns>A ClaimableItemsResponse, containing the list of <see cref="ClaimableItem">ClaimableItem</see> if found.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ClaimableItemsResponse GetAll()
        {
            return this.GetAll<ClaimableItemsResponse>();
        }

        /// <summary>
        /// Add the claimable items for the user, item will appear when user adds a new expense.
        /// </summary>
        /// <param name="claimableItem">
        /// The claimable Item.
        /// </param>
        /// <returns>
        /// A list of <see cref="ClaimableItem">ClaimableItem</see> containing Claimable Items for the user 
        /// </returns>
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        [Route("AddClaimableItems")]
        public ClaimableItemsResponse AddClaimableItems([FromBody] ClaimableItemsRequest claimableItem)
        {
            if (claimableItem.SubCatIds == null || claimableItem.SubCatIds.Count <= 0)
            {
                throw new ApiException(ApiResources.ApiErrorInvalidClaimableItemError, ApiResources.ApiErrorInvalidClaimableItemMessage);
            }

            var response = this.InitialiseResponse<ClaimableItemsResponse>();
            var subCatIds = claimableItem.SubCatIds.ToArray();
            response.List = ((ClaimableItemsRepository)this.Repository).AddClaimableItems(subCatIds).ToList();
            return response;
        }
    }
}