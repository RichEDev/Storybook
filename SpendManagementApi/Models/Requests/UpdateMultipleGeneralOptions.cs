namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;
    using System.Collections.Generic;

    /// <summary>
    /// Contains a list of global or account specific option in the system.
    /// </summary>
    public class UpdateMultipleGeneralOptions : ApiRequest
    {
        /// <summary>
        /// A list <see cref="GeneralOption">GeneralOption</see>
        /// </summary>
        public List<GeneralOption> GeneralOptions { get; set; }
    }
}