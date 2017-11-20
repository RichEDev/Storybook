namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Utilities;

    /// <summary>
    /// A P11D Category is a category that the system uses to group expense items, 
    /// for compliance when filling out P11D forms. You assign <see cref="ExpenseSubCategory">Expense Items</see>
    /// to a P11D Category. P11D forms are submitted to HMRC.
    /// <br/>
    /// For more information on P11D, see <a href="http://www.hmrc.gov.uk/payerti/exb/forms.htm">http://www.hmrc.gov.uk/payerti/exb/forms.htm</a>.
    /// </summary>
    public class P11DCategory : BaseExternalType
    {
        /// <summary>
        /// The unique Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name or label for this Expense Category.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A List of <see cref="ExpenseSubCategory">ExpenseSubCategory</see> IDs items that full under this P11DCategory.
        /// Important: Use the ExpenseSubCategories resource to add and update those. You can link them using this resource.
        /// </summary>
        public List<int> ExpenseSubCategoryIds { get; set; }
    }
}