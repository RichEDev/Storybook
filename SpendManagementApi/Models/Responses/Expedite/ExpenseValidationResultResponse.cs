namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="ExpenseValidationResult">ExpenseValidationResult</see>s.
    /// </summary>
    public class GetExpenseValidationResultsResponse : GetApiResponse<ExpenseValidationResult>
    {
        /// <summary>
        /// Creates a new GetExpenseValidationResultResponse.
        /// </summary>
        public GetExpenseValidationResultsResponse()
        {
            List = new List<ExpenseValidationResult>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="ExpenseValidationResult">ExpenseValidationResult</see>.
    /// </summary>
    public class ExpenseValidationResultResponse : ApiResponse<ExpenseValidationResult>
    {
        /// <summary>
        /// flag to indicate the success or failure of update operation
        /// </summary>
        public  bool IsUpdated { get; set; }
        /// <summary>
        /// Message set if any erro occured during the operation
        /// </summary>
        public string ErrorMessage { get; set; }
    }

}