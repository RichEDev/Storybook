namespace SpendManagementApi.Models.Requests
{
    using Common;
    using Types;

    /// <summary>
    /// Facilitates the finding of AccessRoles, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindAccessRolesRequest : FindRequest
    {
        /// <summary>
        /// Find by name (checks whether the name contains what's here, not an exact match.)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Setting this to true or false will filter the results by 
        /// <see cref="AccessRole.CanEditCostCode">CanEditCostCode</see>
        /// </summary>
        public bool? CanEditCostCode { get; set; }

        /// <summary>
        /// Setting this to true or false will filter the results by 
        /// <see cref="AccessRole.CanEditDepartment">CanEditDepartment</see>
        /// </summary>
        public bool? CanEditDepartment { get; set; }

        /// <summary>
        /// Setting this to true or false will filter the results by 
        /// <see cref="AccessRole.CanEditProjectCode">CanEditProjectCode</see>
        /// </summary>
        public bool? CanEditProjectCode { get; set; }
    }
}