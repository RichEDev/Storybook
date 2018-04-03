namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementLibrary;
    using Interfaces;
    using Common;
    using Utilities;

    /// <summary>
    /// An ItemRole is a collection of <see cref="ExpenseSubCategory">ExpenseSubCategories</see>, grouped togther in a neat group to be able to be assigned to a user.
    /// It is the equivalent in terms of structure to how an <see cref="AccessRole">AccessRole</see> object contains multiple sets of permisssion, which are assigned in bulk to a User.
    /// </summary>
    public class ItemRole : BaseExternalType, IRequiresValidation, IEquatable<ItemRole>
    {
        /// <summary>
        /// The unique Id of the item role.
        /// </summary>
        public int ItemRoleId { get; set; }

        /// <summary>
        /// The name of the item role.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// The description of the item role.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The associated sub categories for the item role.
        /// </summary>
        public List<SubCatItemRole> SubCatItemRoles { get; set; }

        public void Validate(IActionContext actionContext)
        {
            if (this.ItemRoleId == 0 && actionContext.ItemRoles.GetItemRoleByName(this.RoleName) != null)
            {
                throw new ApiException(ApiResources.ItemRoles_InvalidRoleName, ApiResources.ItemRoles_InvalidRoleNameMessage);
            }

            if (String.IsNullOrEmpty(this.Description))
            {
                throw new ApiException(ApiResources.ItemRoles_InvalidRoleDescription, ApiResources.ItemRoles_InvalidRoleDecriptionMessage);
            }

            if (this.SubCatItemRoles == null ||
                (!this.SubCatItemRoles.Any()))
            {
                throw new ApiException(ApiResources.ItemRoles_MissingRoleSubCats, ApiResources.ItemRoles_MissingRoleSubCatsMessage);
            }

            if (this.SubCatItemRoles.Count(subcat => actionContext.SubCategories.GetSubcatById(subcat.SubCatId) == null) > 0)
            {
                throw new ApiException(ApiResources.ItemRoles_InvalidRoleSubCats, ApiResources.ItemRoles_InvalidRoleSubCatsMessage);
            }
        }

        public bool Equals(ItemRole other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Description.Equals(other.Description) && this.RoleName.Equals(other.RoleName)
                   && this.SubCatItemRoles.SequenceEqual(other.SubCatItemRoles);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ItemRole);
        }
    }

    internal static class ItemRoleConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.ItemRole itemRole) where TResult : ItemRole, new()
        {
            if (itemRole == null)
                return null;
            return new TResult
                {
                    CreatedById = itemRole.CreatedBy,
                    CreatedOn = itemRole.CreatedOn,
                    Description = itemRole.Description,
                    ItemRoleId = itemRole.ItemRoleId,
                    ModifiedById = itemRole.ModifiedBy,
                    ModifiedOn = itemRole.ModifiedOn,
                    RoleName = itemRole.Rolename,
                    SubCatItemRoles = itemRole.Items.Values.Select(subcat => subcat.Cast<SubCatItemRole>()).ToList()
                };
        }

        internal static SpendManagementLibrary.ItemRole Cast<TResult>(this ItemRole itemRole, IActionContext actionContext) 
            where TResult : SpendManagementLibrary.ItemRole
        {
            if (itemRole == null)
                return null;
            return new SpendManagementLibrary.ItemRole(
                itemRole.ItemRoleId, 
                itemRole.RoleName, 
                itemRole.Description, 
                itemRole.SubCatItemRoles.Select(roleSubCat => roleSubCat.Cast(actionContext)).ToDictionary(roleSubCat => roleSubCat.SubcatId),
                itemRole.CreatedOn, 
                itemRole.CreatedById, 
                (itemRole.ModifiedOn != null)? itemRole.ModifiedOn.Value : new DateTime(1900,01,01), 
                itemRole.ModifiedById != null ? itemRole.ModifiedById.Value : -1);
        }
    }

}