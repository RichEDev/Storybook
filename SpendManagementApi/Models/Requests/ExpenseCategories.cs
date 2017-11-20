namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;
    using Common;

    /// <summary>
    /// Facilitates the finding of ExpenseCategories, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindExpenseCategoriesRequest : FindRequest
    {
        /// <summary>
        /// Search for an ExpenseCategory that may contain these letters.
        /// </summary>
        [Required]
        public string Label { get; set; }
    }
}
    