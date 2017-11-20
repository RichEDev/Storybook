namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.UserDefinedFields;

    using Interfaces;
    using Utilities;

    /// <summary>
    /// A ProjectCode is a unit of business information against which you record expenditure, in a similar way to <see cref="CostCode">CostCodes</see>.
    /// </summary>
    public class ProjectCode : ArchivableBaseExternalType, IApiFrontForDbObject<IProjectCodeWithUserDefinedFields, ProjectCode>
    {
        /// <summary>
        /// The Id for the Project Code.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The label or name for the Project Code.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// The description for the Project Code.
        /// </summary>
        [Required, MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// Whether this project code is rechargeable.
        /// </summary>
        public bool Rechargeable { get; set; }

        /// <summary>
        /// A list of user-defined objects.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefined { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <returns>This, the API type.</returns>
        public ProjectCode From(IProjectCodeWithUserDefinedFields original, IActionContext actionContext)
        {
            this.Id = original.Id;
            this.Label = original.Name;
            this.Description = original.Description;
            this.Rechargeable = original.Rechargeable;
            this.UserDefined = new List<UserDefinedFieldValue>();

            foreach (KeyValuePair<int, object> keyValuePair in original.UserDefinedFieldValues)
            {
                this.UserDefined.Add(new UserDefinedFieldValue(keyValuePair.Key, keyValuePair.Value));
            }

            this.Archived = original.Archived;
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
        public IProjectCodeWithUserDefinedFields To(IActionContext actionContext)
        {
            var udfSortedList = new SortedList<int, object>();

            foreach (UserDefinedFieldValue userDefinedFieldValue in UserDefined)
            {
                udfSortedList.Add(userDefinedFieldValue.Id, userDefinedFieldValue.Value);
            }

            return new ProjectCodeWithUserDefinedFields(new BusinessLogic.ProjectCodes.ProjectCode(Id, Label, Description, Archived, Rechargeable), new UserDefinedFieldValueCollection(udfSortedList));
        }
    }
}