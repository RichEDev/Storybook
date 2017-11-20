
namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;
    using SpendManagementLibrary;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a response containing a list of <see cref="cAccount"/> accounts.
    /// </summary>
    public class GetAccountVatCalculationEnabledResponses : ApiResponse
    {
        ///<summary>
        /// Gets or sets the list of accounts.
        /// </summary>
        public List<int> List { get; set; }
    }
    /// <summary>
    /// Represents a response containing Payment process status update(download or execute status)
    /// </summary>
    public class FuelReceiptToVATCalculationProcessResponse : ApiResponse
    {
        ///<summary>
        /// Gets or sets processed status for a request
        /// </summary>
        public int isProcessed { get; set; }
    }

}