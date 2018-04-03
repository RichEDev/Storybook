namespace SpendManagementApi.Models.Types
{
    using System;
    using SpendManagementLibrary;
    using Interfaces;
    using Common;

    /// <summary>
    /// Links <see cref="ExpenseSubCategory">ExpenseSubCategories</see> and <see cref="ItemRole">List</see>.
    /// </summary>
    public class SubCatItemRole : DeletableBaseExternalType, IRequiresValidation, IEquatable<SubCatItemRole>
    {
        /// <summary>
        /// The unique Id of the SubCatItemRole.
        /// </summary>
        public int ItemRoleSubCatId { get; set; }

        /// <summary>
        /// The Id of the related ItemRole.
        /// </summary>
        internal int ItemRoleId { get; set; }

        /// <summary>
        /// The Id of the related SubCategory.
        /// </summary>
        public int SubCatId { get; set; }

        /// <summary>
        /// Whether to add to the template.
        /// </summary>
        public bool AddToTemplate { get; set; }

        /// <summary>
        /// The maximum claim allowed with a receipt.
        /// </summary>
        public decimal MaximumAllowedWithReceipt { get; set; }

        /// <summary>
        /// The maximum claim allowed without a receipt.
        /// </summary>
        public decimal MaximumAllowedWithoutReceipt { get; set; }

        public void Validate(IActionContext actionContext)
        {
            if (actionContext.ItemRoles.GetItemRoleById(this.ItemRoleId) == null)
            {
                throw new ApiException("Invalid Item Role", "Please provide a valid item role id");
            }

            if (actionContext.SubCategories.GetSubcatById(this.SubCatId) == null)
            {
                throw new ApiException("Invalid Subcat", "Please provide a valid sub category id");
            }
        }

        public bool Equals(SubCatItemRole other)
        {
            if (other == null)
            {
                return false;
            }
            return this.AddToTemplate.Equals(other.AddToTemplate) && this.MaximumAllowedWithoutReceipt.Equals(other.MaximumAllowedWithoutReceipt)
                   && this.MaximumAllowedWithReceipt.Equals(other.MaximumAllowedWithReceipt) && this.SubCatId.Equals(other.SubCatId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SubCatItemRole);
        }
    }

    internal static class SubCatItemRoleExtension
    {
        internal static TResult Cast<TResult>(this RoleSubcat roleSubcat)
            where TResult : SubCatItemRole, new()
        {
            if (roleSubcat == null)
                return null;
            return new TResult
                {
                    ItemRoleSubCatId = roleSubcat.RolesubcatId,
                    AddToTemplate = roleSubcat.Isadditem,
                    ItemRoleId = roleSubcat.RoleId,
                    MaximumAllowedWithoutReceipt = roleSubcat.MaximumLimitWithoutReceipt,
                    MaximumAllowedWithReceipt = roleSubcat.MaximumLimitWithReceipt,
                    SubCatId = roleSubcat.SubcatId
                };
        }

        internal static RoleSubcat Cast(this SubCatItemRole subCatItemRole, IActionContext actionContext)
        {
            if (subCatItemRole == null)
                return null;
            return new RoleSubcat(
                subCatItemRole.ItemRoleSubCatId,
                subCatItemRole.ItemRoleId,
                subCatItemRole.SubCatId,
                subCatItemRole.MaximumAllowedWithoutReceipt,
                subCatItemRole.MaximumAllowedWithReceipt,
                subCatItemRole.AddToTemplate);
        }
    }
}