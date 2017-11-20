using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// cElementAccess details for a user about a specific element
    /// </summary>
    [Serializable()]
    public class cElementAccess
    {
         private int nElementID;
        private bool bCanView;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;
         
        /// <summary>
        /// Constructor for storing a roles specific element access details
        /// </summary>
        /// <param name="canView">If this user can view records belonging to this element</param>
        /// <param name="canAdd">If this user can add records belonging to this element</param>
        /// <param name="canEdit">If this user can edit records belonging to this element</param>
        /// <param name="canDelete">If this user can delete records belonging to this element</param>
        public cElementAccess(int elementID, bool canView, bool canAdd, bool canEdit, bool canDelete)
        {
            nElementID = elementID;
            bCanView = canView;
            bCanAdd = canAdd;
            bCanEdit = canEdit;
            bCanDelete = canDelete;
        }

        /// <summary>
        /// Returns the elementID
        /// </summary>
        public int ElementID { get { return nElementID; } }

        /// <summary>
        /// Boolean stating if a user can view or not, if CanEdit, CanAdd or CanDelete are true CanView will return true
        /// </summary>
        public bool CanView
        {
            get
            {
                if (bCanAdd || bCanEdit || bCanDelete)
                {
                    return true;
                }
                return bCanView;
            }

            set { bCanView = value; }
        }

        /// <summary>
        /// Boolean stating if a user can add or not
        /// </summary>
        public bool CanAdd
        {
            get { return bCanAdd; }
            set { bCanAdd = value; }
        }

        /// <summary>
        /// Boolean stating if a user can edit or not
        /// </summary>
        public bool CanEdit
        {
            get { return bCanEdit; }
            set { bCanEdit = value; }
        }

        /// <summary>
        /// Boolean stating if a user can delete or not
        /// </summary>
        public bool CanDelete
        {
            get { return bCanDelete; }
            set { bCanDelete = value; }
        }
    }
   


    /// <summary>
    /// An individual access role
    /// </summary>
    /// 
    [Serializable()]
    public class cAccessRole
    {
        private int nRoleID;
        private string sRoleName;
        private string sDescription;
        private Dictionary<SpendManagementElement, cElementAccess> lstElementAccess;
        private AccessRoleLevel eAccessLevel;
        int nCreatedBy;
        DateTime dtCreatedOn;
        int? nModifiedBy;
        DateTime? dtModifiedOn;
        decimal? dExpenseClaimMinimumAmount;
        decimal? dExpenseClaimMaximumAmount;
        bool bEmployeesCanAmendDesignatedCostCode;
        bool bEmployeesCanAmendDesignatedDepartment;
        bool bEmployeesCanAmendDesignatedProjectCode;
        bool _mustHaveBankAccount;
        List<int> lstAccessableAccessRoles;
        private Dictionary<int, cCustomEntityAccess> lstCustomEntityAccess;

        /// <summary>
        /// The reportable fields for the accessrole.
        /// </summary>
        private List<Guid> reportableFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="cAccessRole"/> class.
        /// </summary>
        /// <param name="roleId">
        /// The role id of an access role.
        /// </param>
        /// <param name="roleName">
        /// The role name of an access role.
        /// </param>
        /// <param name="description">
        /// The description of an access role.
        /// </param>
        /// <param name="elements">
        /// The elements of an access role.
        /// </param>
        /// <param name="accessLevel">
        /// The access level of an access role.
        /// </param>
        /// <param name="createdBy">
        /// The access role created by user.
        /// </param>
        /// <param name="createdOn">
        /// The created on date of an access role.
        /// </param>
        /// <param name="modifiedBy">
        /// The modified by user for an access role.
        /// </param>
        /// <param name="modifiedOn">
        /// The modified on date of an current access role.
        /// </param>
        /// <param name="canAmendDesignatedCostCode">
        /// The can amend designated cost code.
        /// </param>
        /// <param name="canAmendDesignatedDepartment">
        /// The can amend designated department.
        /// </param>
        /// <param name="canAmendDesignatedProjectCode">
        /// The can amend designated project code.
        /// </param>
        /// <param name="mustHaveBankAccount">
        /// The must have bank account for an access role.
        /// </param>
        /// <param name="MinimumExpenseClaimAmount">
        /// The minimum expense claim amount assigned for an access role.
        /// </param>
        /// <param name="maximumExpenseClaimAmount">
        /// The maximum expense claim amount assigned for an access role.
        /// </param>
        /// <param name="accessableAccessRoles">
        /// The accessable access roles for the current access role.
        /// </param>
        /// <param name="customEntityAccess">
        /// The custom entity access level for an access role.
        /// </param>
        /// <param name="allowWebsiteAccess">
        /// The allow website access.
        /// </param>
        /// <param name="allowMobileAccess">
        /// The allow mobile access.
        /// </param>
        /// <param name="allowApiAccess">
        /// The allow api access.
        /// </param>
        /// <param name="exclusionType">
        /// The exclusion type of an access role for reporting fields.
        /// </param>
        /// <param name="reportableFieldsForCurrentRole">
        /// The list of fields which are restricted/allowed for the accessrole to report on
        /// </param>
        public cAccessRole(int roleId, string roleName, string description, Dictionary<SpendManagementElement, cElementAccess> elements, AccessRoleLevel accessLevel, int createdBy, DateTime createdOn, int? modifiedBy, DateTime? modifiedOn, bool canAmendDesignatedCostCode, bool canAmendDesignatedDepartment, bool canAmendDesignatedProjectCode, bool mustHaveBankAccount, decimal? MinimumExpenseClaimAmount, decimal? maximumExpenseClaimAmount, List<int> accessableAccessRoles, Dictionary<int, cCustomEntityAccess> customEntityAccess, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess, int exclusionType = 0, List<Guid> reportableFieldsForCurrentRole = null)
        {
            AllowWebsiteAccess = allowWebsiteAccess;
            AllowMobileAccess = allowMobileAccess;
            AllowApiAccess = allowApiAccess;
            nRoleID = roleId;
            sRoleName = roleName;
            sDescription = description;
            lstElementAccess = elements;
            eAccessLevel = accessLevel;
            nCreatedBy = createdBy;
            dtCreatedOn = createdOn;
            nModifiedBy = modifiedBy;
            dtModifiedOn = modifiedOn;
            dExpenseClaimMaximumAmount = maximumExpenseClaimAmount;
            dExpenseClaimMinimumAmount = MinimumExpenseClaimAmount;
            bEmployeesCanAmendDesignatedCostCode = canAmendDesignatedCostCode;
            bEmployeesCanAmendDesignatedDepartment = canAmendDesignatedDepartment;
            bEmployeesCanAmendDesignatedProjectCode = canAmendDesignatedProjectCode;
            _mustHaveBankAccount = mustHaveBankAccount;
            lstAccessableAccessRoles = accessableAccessRoles;
            lstCustomEntityAccess = customEntityAccess;
            this.ExclusionType = exclusionType;
            this.reportableFields = reportableFieldsForCurrentRole;
        }

        #region Properties

        /// <summary>
        /// The reportable fields enabled for the current access role.
        /// </summary>
        public List<Guid> reportableFieldsEnabled => this.reportableFields;

        /// <summary>
        /// The minimum amount a claim is allowed to be before it cannot be submitted, or null if no minimum is set
        /// </summary>
        public decimal? ExpenseClaimMinimumAmount { get { return dExpenseClaimMinimumAmount; } }

        /// <summary>
        /// The maximum amount a claim is allowed to be before it cannot be submitted, or null if no maximum is set
        /// </summary>
        public decimal? ExpenseClaimMaximumAmount { get { return dExpenseClaimMaximumAmount; } }

        /// <summary>
        /// RoleID of the access role
        /// </summary>
        public int RoleID { get { return nRoleID; } }

        /// <summary>
        /// Name of the access role
        /// </summary>
        public string RoleName { get { return sRoleName; } }

        /// <summary>
        /// Description for the access role
        /// </summary>
        public string Description { get { return sDescription; } }

        /// <summary>
        /// List of individual element access
        /// </summary>
        public Dictionary<SpendManagementElement, cElementAccess> ElementAccess { get { return lstElementAccess; } }

        /// <summary>
        /// Gets the access level the user has within reports
        /// </summary>
        public AccessRoleLevel AccessLevel { get { return eAccessLevel; } }

        /// <summary>
        /// Returns a list of AccessRoleID's of which this role can access to report on.
        /// </summary>
        public List<int> AccessRoleLinks { get { return lstAccessableAccessRoles; } }

        /// <summary>
        /// DateTime when this AccessRole was created
        /// </summary>
        public DateTime CreatedOn { get { return dtCreatedOn; } }

        /// <summary>
        /// EmployeeID for who created this AccessRole
        /// </summary>
        public int CreatedBy { get { return nCreatedBy; } }

        /// <summary>
        /// DateTime when this AccessRole was modified, nullable
        /// </summary>
        public DateTime? ModifiedOn { get { return dtModifiedOn; } }

        /// <summary>
        /// EmployeeID for who modified this AccessRole, nullable
        /// </summary>
        public int? ModifiedBy { get { return nModifiedBy; } }

        /// <summary>
        /// Can this user modify their cost code allocations
        /// </summary>
        public bool CanEditCostCode { get { return bEmployeesCanAmendDesignatedCostCode; } }
        /// <summary>
        /// Can this user modify their department allocations
        /// </summary>
        public bool CanEditDepartment { get { return bEmployeesCanAmendDesignatedDepartment; } }
        /// <summary>
        /// Can this user modify their project code allocations
        /// </summary>
        public bool CanEditProjectCode { get { return bEmployeesCanAmendDesignatedProjectCode; } }

        /// <summary>
        /// Does the Employee require at least one Bank Account before they can claim and Expense Item?
        /// </summary>
        public bool MustHaveBankAccount { get { return  _mustHaveBankAccount; } }
        
        /// <summary>
        /// List of individual custom entity access indexed by entity id
        /// </summary>
        public Dictionary<int, cCustomEntityAccess> CustomEntityAccess { get { return lstCustomEntityAccess; } }

        public bool AllowWebsiteAccess { get; set; }
        public bool AllowMobileAccess { get; set; }
        public bool AllowApiAccess { get; set; }

        /// <summary>
        /// Gets the exclusion type for an accessrole.
        /// </summary>
        public int ExclusionType { get; }

        #endregion
    }


}
