namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// <see cref="ExpenseCategory">ExpenseCategories</see> are the user definable categories under which you can submit an expense claim.
    /// </summary>
    [RoutePrefix("ExpenseCategories")]
    [Version(1)]
    public class ExpenseCategoriesV1Controller : BaseApiController<ExpenseCategory>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="ExpenseCategory">ExpenseCategories</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions]
        [Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets the entire list of <see cref="ExpenseCategory">ExpenseCategories</see>.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.ExpenseCategories, AccessRoleType.View)]
        public GetExpenseCategoriesResponse GetAll()
        {
            return this.GetAll<GetExpenseCategoriesResponse>();
        }

        /// <summary>
        /// Gets a particular Expense Category by its Id.
        /// </summary>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseCategories, AccessRoleType.View)]
        public ExpenseCategoryResponse Get([FromUri] int id)
        {
            return this.Get<ExpenseCategoryResponse>(id);
        }

        /// <summary>
        /// Finds all <see cref="ExpenseCategory">ExpenseCategories</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: Name<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>Item roles matching specified criteria</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.ExpenseCategories, AccessRoleType.View)]
        public GetExpenseCategoriesResponse Find([FromUri] FindExpenseCategoriesRequest criteria)
        {
            var response = this.InitialiseResponse<GetExpenseCategoriesResponse>();
            var conditions = new List<Expression<Func<ExpenseCategory, bool>>>();

            if (!string.IsNullOrWhiteSpace(criteria.Label))
            {
                conditions.Add(r => r.Label.ToLower().Contains(criteria.Label.Trim().ToLower()));
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds an Expense Category.
        /// </summary>
        /// <param name="request">The Expense Category to add. <br/>
        /// When adding a new Expense Category through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Name: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>An ExpenseCategoryResponse containing the added <see cref="ExpenseCategory">ExpenseCategory</see>.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.ExpenseCategories, AccessRoleType.Add)]
        public ExpenseCategoryResponse Post([FromBody] ExpenseCategory request)
        {
            return this.Post<ExpenseCategoryResponse>(request);
        }

        /// <summary>
        /// Edits an <see cref="ExpenseCategory">ExpenseCategory</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="ExpenseCategory">ExpenseCategory</see> to edit.</param>
        /// <param name="request">The <see cref="ExpenseCategory">ExpenseCategory</see> to edit.</param>
        /// <returns>An ExpenseCategoryResponse containing the edited <see cref="ExpenseCategory">ExpenseCategory</see></returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseCategories, AccessRoleType.Edit)]
        public ExpenseCategoryResponse Put([FromUri] int id, [FromBody] ExpenseCategory request)
        {
            request.Id = id;
            return this.Put<ExpenseCategoryResponse>(request);
        }

        /// <summary>
        /// Deletes an <see cref="ExpenseCategory">ExpenseCategory</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="ExpenseCategory">ExpenseCategory</see> to be deleted</param>
        /// <returns>An ExpenseCategoryResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.ExpenseCategories, AccessRoleType.Delete)]
        public ExpenseCategoryResponse Delete(int id)
        {
            return this.Delete<ExpenseCategoryResponse>(id);
        }

        /// <summary>
        /// Gets the Expense Categories for an employee
        /// </summary>
        /// <returns>A <see cref="ListItemDataResponse">ListItemDataResponse</see></returns>
        [Route("GetEmployeeExpenseCategories")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public ListItemDataResponse GetEmployeeExpenseCategories()
        {
            var response = this.InitialiseResponse<ListItemDataResponse>();
            response.List = ((ExpenseCategoryRepository)this.Repository).GetEmployeeExpenseCategories();
    
            return response;
        }

        /// <summary>
        /// Gets the Expense Categories and Sub Categories for an employee
        /// </summary>
        /// <returns>A <see cref="EmployeeCategoriesAndSubCategoriesResponse">EmployeeCategoriesAndSubCategoriesResponse</see></returns>
        [Route("GetEmployeeCategoriesAndSubCategories")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public EmployeeCategoriesAndSubCategoriesResponse GetEmployeeCategoriesAndSubCategories()
        {
            var response = this.InitialiseResponse<EmployeeCategoriesAndSubCategoriesResponse>();
            response = ((ExpenseCategoryRepository)this.Repository).GetEmployeeCategoriesAndSubCategories(response);

            return response;
        }

        /// <summary>
        /// Gets the Expense Categories and Sub Categories for a claimant, when called as the claimant's approver
        /// </summary>
        /// <param name="request">
        /// The <see cref="GetClaimantsRequest"> GetClaimantsRequest</see>
        /// </param>
        /// <returns>
        /// A <see cref="EmployeeCategoriesAndSubCategoriesResponse">EmployeeCategoriesAndSubCategoriesResponse</see>
        /// </returns>
        [HttpPut, Route("GetEmployeeCategoriesAndSubCategoriesByEmployee"), ApiExplorerSettings(IgnoreApi = true)]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public EmployeeCategoriesAndSubCategoriesResponse GetEmployeeCategoriesAndSubCategoriesByEmployee(GetClaimantsRequest request)
        {
            var response = this.InitialiseResponse<EmployeeCategoriesAndSubCategoriesResponse>();
            response = ((ExpenseCategoryRepository)this.Repository).GetCategoriesAndSubCategoriesForEmployee(response,request.ExpenseId, request.EmployeeId);

            return response;
        }

        #endregion Api Methods
    }
}
