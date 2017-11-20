namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;
    using Repositories;
    using Attributes;
    using Interfaces;
    using Models.Common;
    using Models.Requests;
    using Models.Responses;
    using Models.Types;

    /// <summary>
    /// Manages operations on <see cref="ExpenseSubCategory">ExpenseSubCategories</see>.
    /// </summary>
    [RoutePrefix("ExpenseSubCategories")]
    [Version(1)]
    public class ExpenseSubCategoriesV1Controller : BaseApiController<ExpenseSubCategory>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="ExpenseSubCategory">ExpenseSubCategory</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="ExpenseSubCategory">ExpenseSubCategories</see> available.
        /// </summary>
        /// <returns>A GetExpenseSubCategoriesResponse containing a list of <see cref="ExpenseSubCategory">ExpenseSubCategories</see></returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.View)]
        public GetExpenseSubCategoriesResponse GetAll()
        {
            return this.GetAll<GetExpenseSubCategoriesResponse>();
        }

        /// <summary>
        /// Gets an <see cref="ExpenseSubCategory">ExpenseSubCategory</see> matching the specified id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ExpenseSubCategory">ExpenseSubCategory</see></param>
        /// <returns>An ExpenseSubCategoryResponse containing an <see cref="ExpenseSubCategory">ExpenseSubCategory</see></returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.View)]
        public ExpenseSubCategoryResponse Get(int id)
        {
            return this.Get<ExpenseSubCategoryResponse>(id);
        }

        /// <summary>
        /// Gets a list of the user's <see cref="ExpenseSubCategoryItemRoleBasic">ExpenseSubCategoryItemRoleBasic</see>.
        /// </summary>
        /// <returns>A list of <see cref="ExpenseSubCategoryItemRoleBasic">ExpenseSubCategoryItemRoleBasic</see>.</returns>
        [HttpGet, Route("GetMySubCatsByItemRoles")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetMySubCatsByItemRolesResponse GetMySubCatsByItemRoles()
        {
            var response = InitialiseResponse<GetMySubCatsByItemRolesResponse>();
            response.List = ((ExpenseSubCategoryRepository)Repository).GetByItemRoles(response);
            return response;
        }

        /// <summary>
        /// Gets all mileage <see cref="ExpenseSubCategory">ExpenseSubCategories</see> that belong to the user's item roles.
        /// </summary>
        /// <returns>A GetExpenseSubCategoriesResponse containing a list of <see cref="ExpenseSubCategory">ExpenseSubCategories</see></returns>
        [HttpGet, Route("GetMyMileageSubCats")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetExpenseSubCategoriesResponse GetMyMileageSubCats()
        {
            var response = InitialiseResponse<GetExpenseSubCategoriesResponse>();
            response.List = ((ExpenseSubCategoryRepository)Repository).GetMileageSubCategories().ToList();
            return response;
        }

        /// <summary>
        /// Gets all mileage <see cref="ExpenseSubCategoryBasic">ExpenseSubCategoryBasic</see> that belong to the employees's item roles.
        /// </summary>
        /// <returns>A ExpenseSubCategoriesMileageBasicResponse containing a list of <see cref="ExpenseSubCategoryBasic">ExpenseSubCategoryBasic</see></returns>
        [HttpGet, Route("GetEmployeeMileageSubCats")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ExpenseSubCategoriesMileageBasicResponse GetEmployeeMileageSubCats()
        {
            var response = InitialiseResponse<ExpenseSubCategoriesMileageBasicResponse>();
            response.List = ((ExpenseSubCategoryRepository)Repository).EmployeeExpenseSubCategoriesForMileage().ToList();
            return response;
        }

        /// <summary>
        /// Gets a  list of <see cref="ExpenseSubCategoryNames">ExpenseSubCategoryNames</see> for a list of subcat Ids
        /// </summary>
        /// <param name="ids">A string of comma seperated subcat ids</param>
        /// <returns>A list of <see cref="ExpenseSubCategoryNames"/>ExpenseSubCategoryNames</returns>.
        [HttpGet, Route("GetBySubcatNames/{ids}")]

        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.View)]
        public GetSubCatNamesByIdsResponse GetBySubcatNames([FromUri] string ids)
        {
           var response = InitialiseResponse<GetSubCatNamesByIdsResponse>();
           response.List = ((ExpenseSubCategoryRepository)Repository).GetBySubCatNamesByIds(ids, response);
           return response;
        }

        /// <summary>
        /// Gets the <see cref="ExpenseSubCategoryBasic">ExpenseSubCategoryBasic</see> for the subcat Id
        /// </summary>
        /// <param name="id">The subcat Id</param>
        /// <returns>The <see cref="ExpenseSubCategoryBasicResponse"></see> ExpenseSubCategoryBasicResponse</returns>
        [HttpGet, Route("GetExpenseSubcategoryBasicById/{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.View)]
        public ExpenseSubCategoryBasicResponse GetExpenseSubcategoryBasicById([FromUri] int id)
        {
            var response = InitialiseResponse<ExpenseSubCategoryBasicResponse>();
            response.Item = ((ExpenseSubCategoryRepository)Repository).GetSubCatBasic(id);
            return response;
        }

        /// <summary>
        /// Adds a new <see cref="ExpenseSubCategory">ExpenseSubCategory</see> with VAT rates.
        /// </summary>
        /// <param name="request">Add given <see cref="ExpenseSubCategory">ExpenseSubCategory</see> with VAT rates.</param>
        /// <returns>An ExpenseSubCategoryResponse containing the added <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.Add)]
        public ExpenseSubCategoryResponse Post([FromBody]ExpenseSubCategory request)
        {
            return this.Post<ExpenseSubCategoryResponse>(request);
        }

        /// <summary>
        /// Updates the <see cref="ExpenseSubCategory">ExpenseSubCategory</see>, adds, updates, deletes VAT rates.
        /// To delete an associated VAT rate, set the ForDelete flag to true.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.</param>
        /// <param name="request">The <see cref="ExpenseSubCategory">ExpenseSubCategory</see> to update with VAT rates needing add/ update/ delete</param>
        /// <returns>An ExpenseSubCategoryResponse containing the edited <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.Edit)]
        public ExpenseSubCategoryResponse Put([FromUri] int id, [FromBody]ExpenseSubCategory request)
        {
            request.SubCatId = id;
            return this.Put<ExpenseSubCategoryResponse>(request);
        }

        /// <summary>
        /// Deletes the<see cref="ExpenseSubCategory">ExpenseSubCategory</see> with given id.
        /// </summary>
        /// <param name="id">Id of the <see cref="ExpenseSubCategory">ExpenseSubCategory</see> to delete.</param>
        /// <returns>An ExpenseSubCategoryResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.Delete)]
        public ExpenseSubCategoryResponse Delete(int id)
        {
            return this.Delete<ExpenseSubCategoryResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="ExpenseSubCategory">ExpenseSubCategories</see> matching specified criteria. 
        /// </summary>
        /// <param name="criteria">
        /// Find parameters - SearchOperator,AccountCode,AlternateAccountCode,
        ///                     Category,PdCat,Reimbursable,ShortSubCategory,SubCatId,VatApplicable . 
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query</param>
        /// <returns><see cref="ExpenseSubCategory">ExpenseSubCategories</see> matching specified criteria</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.ExpenseItems, AccessRoleType.View)]
        public GetExpenseSubCategoriesResponse Find([FromUri]FindExpenseSubCategoriesRequest criteria)
        {
            var findExpenseSubCategoriesResponse = this.InitialiseResponse<GetExpenseSubCategoriesResponse>();
            var conditions = new List<Expression<Func<ExpenseSubCategory, bool>>>();

            if (criteria.CategoryId.HasValue)
            {
                conditions.Add(subcat => subcat.ParentCategoryId == criteria.CategoryId);
            }

            if (!string.IsNullOrEmpty(criteria.AccountCode))
            {
                conditions.Add(subcat => subcat.AccountCode.ToLower().Contains(criteria.AccountCode.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.AlternateAccountCode))
            {
                conditions.Add(subcat => subcat.AlternateAccountCode.ToLower().Contains(criteria.AlternateAccountCode.Trim().ToLower()));
            }

            if (criteria.Reimbursable.HasValue)
            {
                conditions.Add(subcat => subcat.Reimbursable == criteria.Reimbursable);
            }

            if (!string.IsNullOrEmpty(criteria.ShortSubCategory))
            {
                conditions.Add(subcat => subcat.ShortSubCategory.ToLower().Contains(criteria.ShortSubCategory.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.SubCat))
            {
                conditions.Add(subcat => subcat.SubCat.ToLower().Contains(criteria.SubCat.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.ShortSubCategory))
            {
                conditions.Add(subcat => subcat.ShortSubCategory.ToLower().Contains(criteria.ShortSubCategory.Trim().ToLower()));
            }

            if (criteria.SubCatId.HasValue)
            {
                conditions.Add(subcat => subcat.SubCatId == criteria.SubCatId);
            }

            if (criteria.VatApplicable.HasValue)
            {
                conditions.Add(subcat => subcat.VatApplicable == criteria.VatApplicable);
            }

            findExpenseSubCategoriesResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return findExpenseSubCategoriesResponse;
        }

        /// <summary>
        /// Gets a List of ExpenseItemSubCats that belong to a Category ID
        /// </summary>
        /// <param name="categoryId">The categoryId</param>
        /// <param name="isCorpCard">Whether it's for a corporate card reconciliation</param>
        /// <param name="isMobileJourney">Whether its for a mobile journey reconciliation</param>
        /// <returns>A <see cref="ListItemDataResponse">ListItemDataResponse</see></returns>
        [Route("GetExpenseSubCategoryByCategory")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ListItemDataResponse GetExpenseSubCategoriesByCategory([FromUri] int categoryId, [FromUri] bool isCorpCard, [FromUri] bool isMobileJourney)
        {
            var response = this.InitialiseResponse<ListItemDataResponse>();
            response.List = ((ExpenseSubCategoryRepository)this.Repository).GetExpenseSubCatsForCategory(categoryId, isCorpCard, isMobileJourney);
            return response;
        }

        #endregion Api Methods
    }
}