namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Utilities;
    using SpendManagementLibrary;
    using Attributes.Validation;

    /// <summary>
    /// An expense category is a method of grouping similar <see cref="ExpenseSubCategory">ExpenseSubCategories</see>.
    /// </summary>
    public class ExpenseCategory : BaseExternalType, IApiFrontForDbObject<cCategory, ExpenseCategory>
    {
        #region Public Properties

        /// <summary>
        /// The Id of the ExpenseCategory.
        /// </summary>
        [ExpenseCategoryValidation("Id")] 
        public int Id { get; set; }

        /// <summary>
        /// The Category Label.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of the Expense Category.
        /// </summary>
        [Required, MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Populates this API instance with the DAL type's properties.
        /// </summary>
        /// <returns>This (the API type)</returns>
        public ExpenseCategory From(cCategory dbType, IActionContext actionContext)
        {
            Id = dbType.categoryid;
            Label = dbType.category;
            Description = dbType.description;
            CreatedOn = dbType.createdon;
            CreatedById = dbType.createdby;
            ModifiedOn = dbType.modifiedon;
            ModifiedById = dbType.modifiedby;
            return this;
        }

        /// <summary>
        /// Converts this API instance to the equivalent DAL type.
        /// </summary>
        /// <returns>The DAL type</returns>
        public cCategory To(IActionContext actionContext)
        {
            return new cCategory(Id, Label, Description, CreatedOn, CreatedById, ModifiedOn ?? DateTime.Now, ModifiedById ?? 0);
        }

        #endregion
    }


    internal static class ExpenseCategoryConversion
    {
        internal static cCategory Cast<TResult>(this ExpenseCategory category) where TResult : cCategory, new()
        {
            if (category == null)
                return null;

            return new cCategory(
                category.Id,
                category.Label,
                category.Description,
                category.CreatedOn,
                category.CreatedById,
                category.ModifiedOn ?? DateTime.UtcNow,
                category.ModifiedById ?? -1);
        }

        internal static ExpenseCategory Cast<TResult>(this cCategory category, int accountId) where TResult : ExpenseCategory, new()
        {
            if (category == null)
                return null;
            return new TResult
            {
                AccountId = accountId,
                Id = category.categoryid,
                CreatedById = category.createdby,
                CreatedOn = category.createdon,
                Description = category.description,
                Label = category.category,
                ModifiedById = category.modifiedby,
                ModifiedOn = category.modifiedon
            };
        }
    }
}