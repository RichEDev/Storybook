using SpendManagementApi.Models.Common;

namespace SpendManagementApi.Models.Requests
{
    /// <summary>
    /// Facilitates the finding of ItemRoles, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindItemRolesRequest : FindRequest
    {
        /// <summary>
        /// Role Id
        /// </summary>
        public int? ItemRoleId { get; set; }

        /// <summary>
        /// Role Name
        /// </summary>
        public string ItemRoleName { get; set; }

        /// <summary>
        /// Expense Sub Category Id
        /// </summary>
        public int? ExpenseSubCategoryId { get; set; }
    }
}