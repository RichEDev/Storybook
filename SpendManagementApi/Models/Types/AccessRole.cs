using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SpendManagementApi.Attributes.Validation;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Utilities;
using SpendManagementLibrary;

namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Represents a permission that can be applied to an Employee to allow them access to a specfific part of the system.
    /// </summary>
    [MetadataType(typeof(IAccessRole))]
    public class AccessRole : BaseExternalType, IAccessRole, IApiFrontForDbObject<cAccessRole, AccessRole>
    {
        #region Properties

        /// <summary>
        /// RoleID of the access role.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Label of the access role.
        /// </summary>
        [Required, MaxLength(100, ErrorMessage = ApiResources.ErrorMaxLength + @"100")]
        public string Label { get; set; }

        /// <summary>
        /// Description for the access role.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// List of individual <see cref="ElementAccessDetail">ElementAccessDetail</see> that correspond to the SpandManagement system.
        /// </summary>
        public List<ElementAccessDetail> ElementAccess { get; set; }

        /// <summary>
        /// The minimum amount a claim is allowed to be before it cannot be submitted, or null if no minimum is set.
        /// </summary>
        public decimal? ExpenseClaimMinimumAmount { get; set; }

        /// <summary>
        /// The maximum amount a claim is allowed to be before it cannot be submitted, or null if no maximum is set.
        /// </summary>
        public decimal? ExpenseClaimMaximumAmount { get; set; }

        /// <summary>
        /// Gets the access level the user has within reports.
        /// </summary>
        [Required, ValidEnumValue]
        public AccessRoleLevel AccessLevel { get; set; }

        /// <summary>
        /// Returns a list of AccessRoleIDs, which this role can access to report on.
        /// </summary>
        public List<int> AccessRoleLinks { get; set; }

        /// <summary>
        /// Can this user modify their cost code allocations.
        /// </summary>
        [Required]
        public bool CanEditCostCode { get; set; }
        
        /// <summary>
        /// Can this user modify their department allocations.
        /// </summary>
        [Required]
        public bool CanEditDepartment { get; set; }
        
        /// <summary>
        /// Can this user modify their project code allocations.
        /// </summary>
        [Required]
        public bool CanEditProjectCode { get; set; }

        /// <summary>
        /// Does the Employee require a Bank Account to claim Expenses?
        /// </summary>
        [Required]
        public bool MustHaveBankAccount { get; set; }

        /// <summary>
        /// List of individual custom entity access indexed by entity id.
        /// </summary>
        public List<CustomEntityGroupAccess> CustomEntityAccess { get; set; }
        
        /// <summary>
        /// Allow this role access to the website.
        /// </summary>
        [Required]
        public bool AllowWebsiteAccess { get; set; }

        /// <summary>
        /// Allow this role access to the Mobile.
        /// </summary>
        [Required]
        public bool AllowMobileAccess { get; set; }

        /// <summary>
        /// Allow this role access to the Api.
        /// </summary>
        [Required]
        public bool AllowApiAccess { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new AccessRole.
        /// </summary>
        public AccessRole()
        {
            AccessRoleLinks = new List<int>();
            ElementAccess = new List<ElementAccessDetail>();
            CustomEntityAccess = new List<CustomEntityGroupAccess>();
        }

        /// <summary>
        /// Populates this Item from a DAL type.
        /// </summary>
        /// <param name="dbType">The database access layer type.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>This, a fully populated object</returns>
        public AccessRole From(cAccessRole dbType, IActionContext actionContext)
        {
            Id = dbType.RoleID;
            Label = dbType.RoleName;
            Description = dbType.Description;
            ElementAccess = dbType.ElementAccess
                            .Select(item => new ElementAccessDetail().From(item.Value, actionContext))
                            .ToList();
            AccessLevel = dbType.AccessLevel;
            CreatedById = dbType.CreatedBy;
            CreatedOn = dbType.CreatedOn;
            ModifiedById = dbType.ModifiedBy;
            ModifiedOn = dbType.ModifiedOn;
            ExpenseClaimMinimumAmount = dbType.ExpenseClaimMinimumAmount;
            ExpenseClaimMaximumAmount = dbType.ExpenseClaimMaximumAmount;
            CanEditCostCode = dbType.CanEditCostCode;
            CanEditDepartment = dbType.CanEditDepartment;
            CanEditProjectCode = dbType.CanEditProjectCode;
            MustHaveBankAccount = dbType.MustHaveBankAccount;
            AccessRoleLinks = dbType.AccessRoleLinks;
            CustomEntityAccess = dbType.CustomEntityAccess
                                .Select(item => new CustomEntityGroupAccess().From(item.Value, actionContext))
                                .ToList();
            return this;
        }

        /// <summary>
        /// Converts this Item to it's DAL type.
        /// </summary>
        /// <returns></returns>
        public cAccessRole To(IActionContext actionContext)
        {
            var elementAccess = new Dictionary<SpendManagementElement, cElementAccess>();
            foreach (var elementAccessDetail in ElementAccess)
            {
                elementAccess.Add(((SpendManagementElement)elementAccessDetail.Id), elementAccessDetail.To(actionContext));
            }

            var customAccess = new Dictionary<int, cCustomEntityAccess>();
            foreach (var customEntityAccess in CustomEntityAccess)
            {
                customAccess.Add(customEntityAccess.Id, customEntityAccess.To(actionContext));
            }

            return new cAccessRole(Id, Label, Description, elementAccess, AccessLevel, CreatedById, CreatedOn,
                                   ModifiedById, ModifiedOn, CanEditCostCode, CanEditDepartment, CanEditProjectCode, MustHaveBankAccount,
                                   ExpenseClaimMinimumAmount, ExpenseClaimMaximumAmount, AccessRoleLinks, customAccess, AllowWebsiteAccess, AllowMobileAccess, AllowApiAccess);
        }

        #endregion
    }

   
}