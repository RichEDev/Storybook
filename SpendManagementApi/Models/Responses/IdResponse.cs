using SpendManagementLibrary.Expedite;

namespace SpendManagementApi.Models.Responses
{
    using System;
    using System.Collections.Generic;
    using Common;
    
    /// <summary>
    /// A response that contains a single Id, 
    /// as well as the common API information.
    /// </summary>
    public class IdResponse : ApiResponse<int>
    {
    }

    /// <summary>
    /// A response that contains a list of Ids,
    /// as well as the common API information.
    /// </summary>
    public class GetIdsReponse : ApiResponse
    {
        /// <summary>
        /// The list of Ids.
        /// </summary>
        public List<ValidatableExpenseInfo> List { get; set; }

        /// <summary>
        /// Creates a new GetIdsResponse, initialising the list.
        /// </summary>
        public GetIdsReponse()
        {
            List = new List<ValidatableExpenseInfo>();
        }
    }
}