namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Attributes.Validation;
    using Interfaces;
    using Common;
    using Employees;
    using Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// A CostCode is a unit of financial information against which you record expenditure.<br/>
    /// An <see cref="Employee">Employee</see> or an <see cref="ExpenseSubCategory">ExpenseSubCategory</see>.
    /// </summary>
    public class CostCode : ArchivableBaseExternalType, IApiFrontForDbObject<cCostCode, CostCode>, IRequiresValidation
    {
        /// <summary>
        /// The Id of this object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name / label for this CostCode object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this CostCode object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// The Id of the Owner of this cost code (if there is one).
        /// </summary>
        [RequiredIf("OwnerType")]
        public int? OwnerId { get; set; }

        /// <summary>
        /// The <see cref="SpendManagementElement">SpendManagementElement</see> Type of the Owner (if there is one.)
        /// Should be one of these three: <br/>SpendManagementElement.Employee, <br/>SpendManagementElement.BudgetHolder, or <br/>SpendManagementElement.Team.
        /// </summary>
        [RequiredIf("OwnerId")]
        [IsSpendManagementValue(true, SpendManagementElement.Employees, SpendManagementElement.BudgetHolders, SpendManagementElement.Teams)]
        public SpendManagementElement? OwnerType { get; set; }

        /// <summary>
        /// A sorted dictionary of any User Defined Fields.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefined { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <returns>This, the API type.</returns>
        public CostCode From(cCostCode dbType, IActionContext actionContext)
        {
            this.Id = dbType.CostcodeId;
            this.Label = dbType.Costcode;
            this.Description = dbType.Description;
            this.OwnerId = dbType.OwnerId();
            this.OwnerType = dbType.OwnerElementType();
            // if the udf list is null or empty, let's just set it to null.
            this.UserDefined = dbType.UserdefinedFields == null || dbType.UserdefinedFields.Count == 0 ? null :  dbType.UserdefinedFields.ToUserDefinedFieldValueList();
            this.CreatedById = dbType.CreatedBy;
            this.CreatedOn = dbType.CreatedOn;
            this.ModifiedById = dbType.ModifiedBy;
            this.ModifiedOn = dbType.ModifiedOn;
            this.Archived = dbType.Archived;

            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public cCostCode To(IActionContext actionContext)
        {
            var isEmployee = OwnerType == SpendManagementElement.Employees;
            var isBudgetHolder = OwnerType == SpendManagementElement.BudgetHolders;
            var isTeam = OwnerType == SpendManagementElement.Teams;

            return new cCostCode(Id, Label, Description, Archived, CreatedOn, CreatedById, ModifiedOn, ModifiedById, this.UserDefined.ToSortedList(), isEmployee ? OwnerId : null, isTeam ? OwnerId : null, isBudgetHolder ? OwnerId : null);
        }

        /// <summary>
        /// Validates the object for any database / relational errors.
        /// </summary>
        /// <param name="actionContext">An actionContext that contains varous DB access.</param>
        public void Validate(IActionContext actionContext)
        {
            // if owner isn't set, all is good
            if (OwnerId == null) return;

            var shouldThrow = false;

            // check the owner
            switch (OwnerType)
            {
                case SpendManagementElement.Employees:
                    shouldThrow = actionContext.Employees.GetEmployeeById((int)OwnerId) == null;
                    break;

                case SpendManagementElement.BudgetHolders:
                    shouldThrow = actionContext.BudgetHolders.getBudgetHolderById((int)OwnerId) == null;
                    break;

                case SpendManagementElement.Teams:
                    shouldThrow = actionContext.Teams.GetTeamById((int)OwnerId) == null;
                    break;
            }

            if (shouldThrow)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            this.UserDefined = UdfValidator.Validate(this.UserDefined, actionContext, "userdefined_costcodes");

            // if the udf list is null or empty, let's just set it to null.
            this.UserDefined = this.UserDefined == null || this.UserDefined.Count == 0 ? null : this.UserDefined;
        }
    }
}