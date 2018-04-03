using SpendManagementApi.Interfaces;

namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Spend_Management;
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Represents the details for a signed off Claim.
    /// </summary>
    public class ClaimSignOffDetails : IRequiresValidation, IEquatable<ClaimSignOffDetails>
    {
        /// <summary>
        /// The Sign off group id.
        /// </summary>
        public int? SignOffGroupId { get; set; }

        /// <summary>
        /// The sign off group for credit card.
        /// </summary>
        public int? CreditCardSignOffGroupId { get; set; }

        /// <summary>
        /// The sign off group for purchase card.
        /// </summary>
        public int? PurchaseCardSignOffGroupId { get; set; }

        /// <summary>
        /// The sign off group for advances.
        /// </summary>
        public int? AdvancesSignOffGroupId { get; set; }

        /// <summary>
        /// The list of associated Item Roles for this Employee.
        /// <strong>Do not try to modify this user's List by changing this list.</strong>
        /// Instead use the <see cref="ItemRole">List</see> resource.
        /// </summary>
        public List<int> ItemRoles { get; internal set; }


        internal static ClaimSignOffDetails Merge(ClaimSignOffDetails dataToUpdate, ClaimSignOffDetails existingData)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new ClaimSignOffDetails
                                   {
                                       AdvancesSignOffGroupId = existingData.AdvancesSignOffGroupId,
                                       CreditCardSignOffGroupId = existingData.CreditCardSignOffGroupId,
                                       PurchaseCardSignOffGroupId = existingData.PurchaseCardSignOffGroupId,
                                       SignOffGroupId = existingData.SignOffGroupId,
                                       ItemRoles = existingData.ItemRoles
                                   };
            }

            return dataToUpdate;
        }

        public bool Equals(ClaimSignOffDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return this.AdvancesSignOffGroupId.Equals(other.AdvancesSignOffGroupId)
                   && this.CreditCardSignOffGroupId.Equals(other.CreditCardSignOffGroupId)
                   && this.PurchaseCardSignOffGroupId.Equals(other.PurchaseCardSignOffGroupId)
                   && this.SignOffGroupId.Equals(other.SignOffGroupId)
                   && this.ItemRoles.SequenceEqual(other.ItemRoles);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClaimSignOffDetails);
        }

        public void Validate(IActionContext actionContext)
        {
        }
    }

    internal static class ClaimSignOffDetailsConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee, EmployeeItemRoles itemRoles)
            where TResult : ClaimSignOffDetails, new()
        {
            List<int> employeeItemRoles = new List<int>();
            foreach (EmployeeItemRole itemRole in itemRoles.ItemRoles)
            {
                employeeItemRoles.Add(itemRole.ItemRoleId);
            }
            var result =  new TResult
            {
                AdvancesSignOffGroupId = employee.AdvancesSignOffGroup == 0 ? (int?) null : employee.AdvancesSignOffGroup,
                CreditCardSignOffGroupId = employee.CreditCardSignOffGroup == 0 ? (int?)null : employee.CreditCardSignOffGroup,
                PurchaseCardSignOffGroupId = employee.PurchaseCardSignOffGroup == 0 ? (int?)null : employee.PurchaseCardSignOffGroup,
                SignOffGroupId = employee.SignOffGroupID == 0 ? (int?)null : employee.SignOffGroupID,
                ItemRoles = employeeItemRoles
            };
            return result;
        }

        internal static void Cast<TResult>(
            this ClaimSignOffDetails claimSignOffDetails, SpendManagementLibrary.Employees.Employee employee, ICurrentUser currentUser)
        {
            employee.SignOffGroupID = claimSignOffDetails.SignOffGroupId ?? 0;
            employee.AdvancesSignOffGroup = claimSignOffDetails.AdvancesSignOffGroupId ?? 0;
            employee.CreditCardSignOffGroup = claimSignOffDetails.CreditCardSignOffGroupId ?? 0;
            employee.PurchaseCardSignOffGroup = claimSignOffDetails.PurchaseCardSignOffGroupId ?? 0;
        }
    }
}