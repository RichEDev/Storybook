namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types.ClaimableItem;

    /// <summary>
    /// A response containing a list of <see cref="ClaimableItem">ClaimableItem</see>s.
    /// </summary>
    public class ClaimableItemsResponse : GetApiResponse<ClaimableItem>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ClaimableItemsResponse"/> class. 
        /// </summary>
        public ClaimableItemsResponse()
        {
            this.List = new List<ClaimableItem>();
        }
    }
}