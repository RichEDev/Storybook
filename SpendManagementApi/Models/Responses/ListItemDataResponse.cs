using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// A response that returns a List of  <see cref="ListItemData">ListItem</see>
    /// </summary>
    public class ListItemDataResponse : ApiResponse
    {
        /// <summary>
        /// Creates a List of <see cref="ListItemData">ListItem</see>
        /// </summary>
        public List<ListItemData> List { get; set; }
    }
}