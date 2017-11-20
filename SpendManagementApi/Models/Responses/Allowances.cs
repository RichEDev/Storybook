namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;
    
    /// <summary>
    /// A response containing a list of <see cref="Allowance">Item</see>s.
    /// </summary>
    public class GetAllowancesResponse : GetApiResponse<Allowance>
    {
        /// <summary>
        /// Creates a new GetAllowancesResponse.
        /// </summary>
        public GetAllowancesResponse()
        {
            List = new List<Allowance>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Allowance">Allowance</see>.
    /// </summary>
    public class AllowanceResponse : ApiResponse<Allowance>
    {

    }
}
