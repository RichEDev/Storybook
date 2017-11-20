namespace SpendManagementApi.Models.Requests
{
    using Common;
    using Types;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The request for updating an expense item.
    /// </summary>
    public class UpdateExpenseItemRequest : ApiRequest
    {
        /// <summary>
        /// The old claim Id. Used when moving an expense item to a new claim
        /// </summary>
        public int OldClaimId;

        /// <summary>
        ///  Is an offline expense Item?
        /// </summary>
        public bool OfflineItem;

        /// <summary>
        /// The <see cref="ExpenseItem">ExpenseItem</see>
        /// </summary>
        [Required]
        public ExpenseItem ExpenseItem;

        /// <summary>
        /// The reason for amendment if edited by an approver
        /// </summary>
        public string ReasonForAmendment;
    }
}