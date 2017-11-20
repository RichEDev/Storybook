using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpendManagementApi.Models.Types.Expedite;
using SpendManagementApi.Utilities;

namespace SpendManagementApi.Models.Requests.Expedite
{
    /// <summary>
    /// Allows an ExpediteOperator to post all the results of validating a single expense item in one go.
    /// </summary>
    public class ExpenseItemValidationResults
    {
        /// <summary>
        /// The Id of the Expense Item (claim line) that this result pertains to. 
        /// Note that changing this whilst editing a result will move this Result to another ExpenseItem.
        /// </summary>
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// The total of the expense item's invoice.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// The result that you would like to add.
        /// </summary>
        public List<MatchingResult> Results { get; set; }
    }

    /// <summary>
    /// Represents the result of a receipt match against a criterion.
    /// Will be converted to an ExpenseValidationResult.
    /// </summary>
    public class MatchingResult
    {
        /// <summary>
        /// The rule that this is the result for. Note that changing this whilst editing a result
        /// will move this Result to another Criterion.
        /// </summary>
        public int CriterionId { get; set; }

        /// <summary>
        /// The rule that this is the result for. Note that changing this whilst editing a result
        /// will move this Result to another Criterion.
        /// </summary>
        public ExpenseValidationMatchingResult Reason { get; set; }
        
        /// <summary>
        /// Any comments that the Expedite operator wishes to offer as 
        /// extra reasons for why the validation status is the way it is.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Comments { get; set; }

        /// <summary>
        /// Any other data, in XML format.
        /// </summary>
        public string Data { get; set; }
    }
}