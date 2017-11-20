using SpendManagementApi.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary.Mobile;

    /// <summary>
    /// The claim expense items response.
    /// </summary>
    public class ClaimExpenseItemsResponse : GetApiResponse<ClaimExpenseItem>
    {
        /// <summary>
        /// Gets or sets the display fields.
        /// </summary>
        public List<DisplayField> DisplayFields { get; set; }

         /// <summary>
        /// Creates a new GetClaimExpenseItemsResponse.
        /// </summary>
        public ClaimExpenseItemsResponse()
        {
            this.List = new List<ClaimExpenseItem>();
            this.DisplayFields = new List<DisplayField>();
        }
    }


    /// <summary>
    /// Returns the added/ updated expense item
    /// </summary>
    public class ClaimExpenseItemResponse : ApiResponse<ClaimExpenseItem>
    {
    }
}