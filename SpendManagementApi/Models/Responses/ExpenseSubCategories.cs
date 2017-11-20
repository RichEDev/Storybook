namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// Reponse with list of subcategories
    /// </summary>
    public class GetExpenseSubCategoriesResponse : GetApiResponse<ExpenseSubCategory>
    {
        /// <summary>
        /// Creates a new GetExpenseSubCategoriesResponse.
        /// </summary>
        public GetExpenseSubCategoriesResponse()
        {
            List = new List<ExpenseSubCategory>();  
        }
    }

    /// <summary>
    /// Returns the added/ updated subcat
    /// </summary>
    public class ExpenseSubCategoryResponse : ApiResponse<ExpenseSubCategory>
    {
        
    }

    /// <summary>
    /// A response containing the basic subcat details
    /// </summary>
    public class ExpenseSubCategoryBasicResponse : ApiResponse<ExpenseSubCategoryBasic>
    {

    }

    /// <summary>
    /// A response containing a list of <see cref="ExpenseSubCategoryItemRoleBasic"> ExpenseSubCategoryItemRoleBasic</see>.
    /// </summary>
    public class GetMySubCatsByItemRolesResponse : GetApiResponse<ExpenseSubCategoryItemRoleBasic>
    {
        /// <summary>
        /// Creates a new ist of <see cref="ExpenseSubCategoryItemRoleBasic"> ExpenseSubCategoryItemRoleBasic</see>.
        /// </summary>
        public GetMySubCatsByItemRolesResponse()
        {
            List = new List<ExpenseSubCategoryItemRoleBasic>();  
        }
    }

    /// <summary>
    /// A response containing a list of <see cref="ExpenseSubCategoryNames"> ExpenseSubCategoryNames</see>.
    /// </summary>
    public class GetSubCatNamesByIdsResponse : GetApiResponse<ExpenseSubCategoryNames>
    {
        /// <summary>
        /// Creates a new ist of <see cref="ExpenseSubCategoryNames"> ExpenseSubCategoryNames</see>.
        /// </summary>
        public GetSubCatNamesByIdsResponse()
        {
           List = new List<ExpenseSubCategoryNames>();
        }
    }


}