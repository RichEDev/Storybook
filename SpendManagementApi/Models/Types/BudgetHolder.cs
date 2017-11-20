namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Common;
    using Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// A Budget Holder is the encapsulation of an Employee => Budget assignment.
    /// A budget holder can be linked to a particular stage in a group.
    /// </summary>
    public class BudgetHolder : BaseExternalType, IApiFrontForDbObject<cBudgetHolder, BudgetHolder>, IRequiresValidation
    {
        /// <summary>
        /// The unique Id for this BudgetHolder object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name / label for this BudgetHolder object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this BudgetHolder object.
        /// </summary>
        [Required, MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// The Id of the Employee who is responsible for this budget.
        /// </summary>
        [Required]
        public new int? EmployeeId { get; set; }

        /// <summary>
        /// Converts from a DAL to API object.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns></returns>
        public BudgetHolder From(cBudgetHolder dbType, IActionContext actionContext)
        {
            Id = dbType.budgetholderid;
            Label = dbType.budgetholder;
            Description = dbType.description;
            EmployeeId = dbType.employeeid;
            CreatedById = dbType.createdby ?? -1;
            if (dbType.createdon != null) CreatedOn = (DateTime)dbType.createdon;
            ModifiedById = dbType.modifiedby ?? -1;
            ModifiedOn = dbType.modifiedon;
            return this;
        }

        /// <summary>
        /// Converts this API object to a DAL object.
        /// </summary>
        /// <returns></returns>
        public cBudgetHolder To(IActionContext actionContext)
        {
            // ReSharper disable once PossibleInvalidOperationException
            return new cBudgetHolder(Id, Label, Description, EmployeeId.Value, CreatedById, CreatedOn, ModifiedById, ModifiedOn);
        }

        /// <summary>
        /// Performs relational / db validation on this object.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public void Validate(IActionContext actionContext)
        {
            // check that the employee exists.
            // ReSharper disable once PossibleInvalidOperationException
            if (actionContext.Employees.GetEmployeeById(EmployeeId.Value) == null)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorWrongEmployeeId);
            }
        }
    }
}
