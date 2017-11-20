namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The advances response.
    /// </summary>
    public class AdvancesResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="Advance"/>.
        /// </summary>
        public List<Advance> List { get; set; }
    }


    /// <summary>
    /// Returns the added/ updated <see cref="Advance">Advance</see>
    /// </summary>
    public class AdvanceResponse : ApiResponse<Advance>
    {
    }
}