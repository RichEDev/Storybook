namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="Receipt">Receipts</see>.
    /// </summary>
    public class ReceiptResponse : GetApiResponse<Receipt>
    {
        /// <summary>
        /// Creates a new ReceiptResponse.
        /// </summary>
        public ReceiptResponse()
        {
          List = new List<Receipt>();
        }
    }
}