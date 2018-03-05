namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// A response that contains a list of zero to many <see cref="WalletReceipt"></see>
    /// </summary>
    public class GetProcessedReceiptResponse : GetApiResponse<ProcessedReceipt>
    {
    }

    /// <summary>
    /// A response that contains a <see cref="WalletReceipt"></see>
    /// </summary>
    public class ProcessedReceiptResponse : ApiResponse<ProcessedReceipt>
    {
    }
}