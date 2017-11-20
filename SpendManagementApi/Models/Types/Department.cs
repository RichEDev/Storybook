namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Employees;
    using Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// A department is a collection of <see cref="Employee">Employees</see>.
    /// </summary>
    public class Department : ArchivableBaseExternalType, IApiFrontForDbObject<cDepartment, Department>
    {
        /// <summary>
        /// The Id of this department.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The label or name of this department.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }
        
        /// <summary>
        /// The description of this department.
        /// </summary>
        [Required, MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// A list of user-defined objects.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefined { get; set; }
        
        /// <summary>
        /// Create a new Department with an empty UserDefined list.
        /// </summary>
        public Department()
        {
            UserDefined = new List<UserDefinedFieldValue>();
        }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns></returns>
        public Department From(cDepartment dbType, IActionContext actionContext)
        {
            Id = dbType.DepartmentId;
            Label = dbType.Department;
            Description = dbType.Description;
            Archived = dbType.Archived;
            this.UserDefined = dbType.UserdefinedFields.ToUserDefinedFieldValueList();
            CreatedById = dbType.CreatedBy;
            CreatedOn = dbType.CreatedOn;
            ModifiedById = dbType.ModifiedBy ?? -1;
            ModifiedOn = dbType.ModifiedOn;
            Archived = dbType.Archived;

            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns></returns>
        public cDepartment To(IActionContext actionContext)
        {
            return new cDepartment(Id, Label, Description, Archived, CreatedOn, CreatedById, ModifiedOn, ModifiedById, UserDefined.ToSortedList());
        }
    }
}