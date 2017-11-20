namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// Facilitates the management of P11D Categories. See <see cref="P11DCategory">P11DCategory</see>.
    /// </summary>
    [RoutePrefix("P11DCategories")]
    [Version(1)]
    public class P11DCategoriesV1Controller : BaseApiController<P11DCategory>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="P11DCategory">P11DCategories</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="P11DCategory">P11DCategories</see> within the system.
        /// </summary>
        /// <returns>A GetP11DCategoriesResponse containing all the <see cref="P11DCategory">P11DCategories</see>.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.P11D, AccessRoleType.View)]
        public GetP11DCategoriesResponse GetAll()
        {
            return this.GetAll<GetP11DCategoriesResponse>();
        }

        /// <summary>
        /// Gets a <see cref="P11DCategory">P11DCategory</see> with the supplied id.
        /// </summary>
        /// <param name="id">The id of the <see cref="P11DCategory">P11DCategory</see>.</param>
        /// <returns>A P11DCategoryResponse containing the matching <see cref="P11DCategory">P11DCategory</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.P11D, AccessRoleType.View)]
        public P11DCategoryResponse Get(int id)
        {
            return this.Get<P11DCategoryResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="P11DCategory">P11DCategories</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>Label, SubCatIds<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetP11DCategoriesResponse containing <see cref="P11DCategory">P11DCategories</see> matching the specified criteria.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.P11D, AccessRoleType.View)]
        public GetP11DCategoriesResponse Find([FromUri] FindP11DCategoryRequest criteria)
        {
            var response = this.InitialiseResponse<GetP11DCategoriesResponse>();
            var conditions = new List<Expression<Func<P11DCategory, bool>>>();

            if (!string.IsNullOrWhiteSpace(criteria.Label))
            {
                conditions.Add(r => r.Label.ToLower().Contains(criteria.Label.Trim().ToLower()));
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="P11DCategory">P11DCategory</see>.
        /// </summary>
        /// <param name="request">The <see cref="P11DCategory">P11DCategory</see> to add.<br/>
        /// When adding a new <see cref="P11DCategory">P11DCategory</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>A P11DCategoryResponse containing the added <see cref="P11DCategory">P11DCategory</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.P11D, AccessRoleType.Add)]
        public P11DCategoryResponse Post([FromBody] P11DCategory request)
        {
            return this.Post<P11DCategoryResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="P11DCategory">P11DCategory</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="P11DCategory">P11DCategory</see> to edit.</param>
        /// <param name="request">The <see cref="P11DCategory">P11DCategory</see> to edit.</param>
        /// <returns>A P11DCategoryResponse containing the edited <see cref="P11DCategory">P11DCategory</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.P11D, AccessRoleType.Edit)]
        public P11DCategoryResponse Put([FromUri] int id, [FromBody] P11DCategory request)
        {
            request.Id = id;
            return this.Put<P11DCategoryResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="P11DCategory">P11DCategory</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="P11DCategory">P11DCategory</see> to be deleted.</param>
        /// <returns>A P11DCategoryResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.P11D, AccessRoleType.Delete)]
        public P11DCategoryResponse Delete(int id)
        {
            return this.Delete<P11DCategoryResponse>(id);
        }


        #endregion Api Methods
    }

}
