namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="Receipt">Receipt</see>s.
    /// </summary>
    public class GetReceiptsResponse : GetApiResponse<Receipt>
    {
        /// <summary>
        /// Creates a new GetReceiptResponse.
        /// </summary>
        public GetReceiptsResponse()
        {
            List = new List<Receipt>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Receipt">Receipt</see>.
    /// </summary>
    public class ReceiptResponse : ApiResponse<Receipt>
    {
    }

}