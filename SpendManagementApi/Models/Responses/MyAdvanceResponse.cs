namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The My Advance response
    /// </summary>
    public class MyAdvanceResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a list of <see cref="MyAdvance"/>.
        /// </summary>
        public List<MyAdvance> List { get; set; }
    }
}