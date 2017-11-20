namespace SpendManagementApi.Models.Requests
{
    using Common;

    /// <summary>
    /// Facilitates the finding of BudgetHolders, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindBudgetHoldersRequest : FindRequest
    {
        /// <summary>
        /// Search by label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Search by description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Search by Employee Id.
        /// </summary>
        public int? EmployeeId { get; set; }
    }
}