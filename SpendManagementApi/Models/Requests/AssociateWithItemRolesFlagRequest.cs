namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;

    /// <summary>
    /// The associate with item roles flag request.
    /// </summary>
    public class AssociateItemRolesWithFlagRequest
    {
        /// <summary>
        /// Gets or sets the flag id.
        /// </summary>
        public int FlagId { get; set; }

        /// <summary>
        /// Gets or sets the item role ids.
        /// </summary>
        public List<int> ItemRoleIds { get;set; }
    }
}