namespace BusinessLogic.ProjectCodes
{
    using System;

    using UserDefinedFields;

    /// <summary>
    /// The project code with user defined fields class extends <see cref="ProjectCode"/> by adding the <c>UserDefinedFieldValues</c> property.
    /// </summary>
    [Serializable]
    public class ProjectCodeWithUserDefinedFields : IProjectCodeWithUserDefinedFields
    {
        /// <summary>
        /// The <see cref="IProjectCode"/> this <see cref="ProjectCodeWithUserDefinedFields"/> covers.
        /// </summary>
        private readonly IProjectCode _projectCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCodeWithUserDefinedFields"/> class. 
        /// </summary>
        /// <param name="projectCode">
        /// The base <see cref="IProjectCode"/>
        /// </param>
        /// <param name="userDefinedFieldValues">
        /// The <see cref="UserDefinedFieldValueCollection"/> for this <see cref="IProjectCode"/>
        /// </param>
        public ProjectCodeWithUserDefinedFields(IProjectCode projectCode, UserDefinedFieldValueCollection userDefinedFieldValues)
        {
            Guard.ThrowIfNull(projectCode, nameof(projectCode));
            Guard.ThrowIfNull(userDefinedFieldValues, nameof(userDefinedFieldValues));

            this._projectCode = projectCode;
            this.UserDefinedFieldValues = userDefinedFieldValues;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCodeWithUserDefinedFields"/> class. 
        /// </summary>
        /// <remarks>Used for creaing default instances.</remarks>
        public ProjectCodeWithUserDefinedFields()
        {
            this._projectCode = new ProjectCode();
            this.UserDefinedFieldValues = new UserDefinedFieldValueCollection();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProjectCodeWithUserDefinedFields"/> is archived.
        /// </summary>
        public bool Archived
        {
            get { return this._projectCode.Archived; }
            set { this._projectCode.Archived = value; }
        }

        /// <summary>
        /// Gets or sets the description for this <see cref="ProjectCodeWithUserDefinedFields"/>.
        /// </summary>
        public string Description
        {
            get { return this._projectCode.Description; }
            set { this._projectCode.Description = value; }
        }

        /// <summary>
        /// Gets or sets the identifier for <see cref="ProjectCodeWithUserDefinedFields"/>
        /// </summary>
        public int Id
        {
            get { return this._projectCode.Id; }
            set { this._projectCode.Id = value; }
        }

        /// <summary>
        /// Gets or sets the reference/code for this <see cref="ProjectCodeWithUserDefinedFields"/>.
        /// </summary>
        public string Name
        {
            get { return this._projectCode.Name; }
            set { this._projectCode.Name = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="ProjectCodeWithUserDefinedFields"/> is rechargeable.
        /// </summary>
        public bool Rechargeable => this._projectCode.Rechargeable;

        /// <summary>
        /// Gets or sets a <see cref="UserDefinedFieldValueCollection"/> associated with this <see cref="IProjectCode"/>.
        /// </summary>
        public UserDefinedFieldValueCollection UserDefinedFieldValues { get; set; }

        /// <summary>
        /// Get the value to display in the list.
        /// </summary>
        /// <param name="useDescription">If true, use the "Description" field of the object, else use the "Reference"</param>
        /// <returns>The text description or reference of the object.</returns>
        public string ToString(bool useDescription)
        {
            return useDescription ? this.Description : this.Name;
        }
    }
}
