namespace SpendManagementApi.Models.Responses.Expedite
{
    using System.Collections.Generic;
    using Common;
    using Types.Expedite;

    /// <summary>
    /// A response containing a list of <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see>s.
    /// </summary>
    public class GetExpenseValidationCriteriaResponse : GetApiResponse<ExpenseValidationCriterion>
    {
        /// <summary>
        /// Creates a new GetExpenseValidationCriterionResponse.
        /// </summary>
        public GetExpenseValidationCriteriaResponse()
        {
            List = new List<ExpenseValidationCriterion>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="ExpenseValidationCriterion">ExpenseValidationCriterion</see>.
    /// </summary>
    public class ExpenseValidationCriterionResponse : ApiResponse<ExpenseValidationCriterion>
    {
    }

}