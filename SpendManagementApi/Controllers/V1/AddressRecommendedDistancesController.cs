namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="AddressRecommendedDistance">AddressRecommendedDistances</see>.
    /// </summary>
    [RoutePrefix("AddressRecommendedDistances")]
    [Version(1)]
    public class AddressRecommendedDistancesV1Controller : BaseApiController<AddressRecommendedDistance>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="AddressRecommendedDistance">AddressRecommendedDistances</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions]
        [Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Gets all <see cref="AddressRecommendedDistance">AddressRecommendedDistances</see> for the given Address Id.
        /// </summary>
        /// <param name="id">The Id of the Address to fetch the distances for.</param>
        /// <returns>A GetAddressRecommendedDistancesResponse containing any matching <see cref="AddressRecommendedDistance">AddressRecommendedDistances</see></returns>
        [HttpGet, Route("ByAddress/{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.View)]
        public GetAddressRecommendedDistancesResponse ByAddress(int id)
        {
            var response = this.InitialiseResponse<GetAddressRecommendedDistancesResponse>();
            response.List = ((AddressRecommendedDistanceRepository) this.Repository).GetByAddress(id).ToList();
            return response;
        }

        /// <summary>
        /// Gets a single <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>An AddresssResponse object, which will contain an <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> if one was found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.View)]
        public AddressRecommendedDistanceResponse Get([FromUri] int id)
        {
            return this.Get<AddressRecommendedDistanceResponse>(id);
        }
        
        /// <summary>
        /// Adds an <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>.
        /// </summary>
        /// <param name="request">The <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> to add. <br/>
        /// When adding a new <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.<br/>
        /// AddressAId: Must be the ID of a valid Address.<br/>
        /// AddressBId: Must be the ID of a valid Address.
        /// </param>
        /// <returns>An AddressRecommendedDistanceResponse.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Add)]
        public AddressRecommendedDistanceResponse Post([FromBody] AddressRecommendedDistance request)
        {
            return this.Post<AddressRecommendedDistanceResponse>(request);
        }

        /// <summary>
        /// Edits an <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>.<br/>
        /// When editing an existing <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to the Id of a valid <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>.<br/>
        /// RecommendedDistance: This can change.<br/>
        /// <strong>Note: You cannot edit the two address Ids using this method. In order to do this, create another recommended distance and delete the original.</strong>
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>The edited <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> in an AddressRecommendedDistanceResponse.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressRecommendedDistanceResponse Put([FromUri] int id, [FromBody] AddressRecommendedDistance request)
        {
            request.Id = id;
            return this.Put<AddressRecommendedDistanceResponse>(request);
        }

        /// <summary>
        /// Deletes an <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="AddressRecommendedDistance">AddressRecommendedDistance</see> to be deleted.</param>
        /// <returns>An AddressRecommendedDistanceResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Delete)]
        public AddressRecommendedDistanceResponse Delete(int id)
        {
            return this.Delete<AddressRecommendedDistanceResponse>(id);
        }

        /// <summary>
        /// Gets a recommended or custom distance for origin and destination address Ids
        /// </summary>
        /// <param name="originAddressId">
        /// The origin address Id.
        /// </param>
        /// <param name="destinationAddressId">
        /// The destination address Id
        /// </param>
        /// <param name="vehicleId">
        /// The Id of the vehicle the distance is calculated for
        /// </param>
        /// <returns>
        /// The <see cref="RecommendedOrCustomDistanceResponse">RecommendedOrCustomDistanceResponse</see>
        /// </returns>
        [HttpGet, Route("GetRecommendedOrCustomDistance")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public RecommendedOrCustomDistanceResponse GetRecommendedOrCustomDistance(int originAddressId, int destinationAddressId, int vehicleId)
        {
            var response = this.InitialiseResponse<RecommendedOrCustomDistanceResponse>();
            response.Distance = ((AddressRecommendedDistanceRepository)this.Repository).GetRecommendedOrCustomDistance(originAddressId, destinationAddressId, vehicleId);
            return response;
        }

        #endregion Api Methods
    }
}
