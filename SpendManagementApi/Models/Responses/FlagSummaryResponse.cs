namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="FlagSummary">FlagSummary</see>/>s
    /// </summary>
    public class FlagSummaryResponse : GetApiResponse<FlagSummary>
    {
        /// <summary>
        /// Creates a new FlagSummaryResponse
        /// </summary>
        public FlagSummaryResponse()
        {
            List = new List<FlagSummary>();
        }
    }
}