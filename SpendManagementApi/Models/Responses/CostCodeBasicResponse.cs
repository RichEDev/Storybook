namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The cost code basic response.
    /// </summary>
    public class CostCodeBasicResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of <see cref="CostCodeBasic">CostCodeBasic</see>
        /// </summary>
        public List<CostCodeBasic> List { get; set; }
    }
}