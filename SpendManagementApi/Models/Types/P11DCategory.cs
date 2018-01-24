namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Utilities;
    using BusinessLogic.P11DCategories;
    using Interfaces;

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

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <returns>This, the API type.</returns>
        public P11DCategory From(IP11DCategory original, IActionContext actionContext)
        {
            this.Id = original.Id;
            this.Label = original.Name;
            
            this.CreatedById = 0;
            this.CreatedOn = default(DateTime);
            this.ModifiedById = 0;
            this.ModifiedOn = default(DateTime);

            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public IP11DCategory To(IActionContext actionContext)
        {
            return new BusinessLogic.P11DCategories.P11DCategory(this.Id, this.Label);
        }
    }
}