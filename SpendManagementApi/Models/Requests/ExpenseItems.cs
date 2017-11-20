namespace SpendManagementApi.Models.Requests
{
    using Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Facilitates the finding of ExpenseSubCategories, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindExpenseSubCategoriesRequest : FindRequest
    {
        /// <summary>
        /// Sub Category Id
        /// </summary>
        public int? SubCatId { get; set; }

        /// <summary>
        /// Expense CategoryId
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Sub Category Name
        /// </summary>
        public string SubCat { get; set; }

        /// <summary>
        /// Account Code
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// P11d Category
        /// </summary>
        public string PdCat { get; set; }

        /// <summary>
        /// Re-imbursable
        /// </summary>
        public bool? Reimbursable { get; set; }

        /// <summary>
        /// Alternate Account Code
        /// </summary>
        public string AlternateAccountCode { get; set; }

        /// <summary>
        /// Abbreviation
        /// </summary>
        public string ShortSubCategory { get; set; }

        /// <summary>
        /// Vat Applicable
        /// </summary>
        public bool? VatApplicable { get; set; }

    }

    /// <summary>
    /// Facilitates the retrieval of subcat names from their Ids
    /// </summary>
    public class GetExpenseSubCategoryNamesByIdsRequest : ApiRequest
    {
        /// <summary>
        /// A list of Subcat Ids.
        /// </summary>
        ///  
        [Required]
        public List<int> SubCatIds { get; set; }
    }
}