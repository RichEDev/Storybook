namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic.Fields;
    using BusinessLogic.UserDefinedFields;

    /// <summary>
    /// The "Fields" Mocked objects used in unit tests
    /// </summary>
    public interface IFields
    {
        
        /// <summary>
        /// Gets or sets the Mock'd <see cref="UserDefinedFieldRepository"/>
        /// </summary>
        UserDefinedFieldRepository UserDefinedFieldRepository { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="BusinessLogic.UserDefinedFields.UserDefinedFieldRepository"/>
        /// </summary>
        UserDefinedFieldValueRepository UserDefinedFieldValueRepository { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="FieldRepository"/>
        /// </summary>
        FieldRepository FieldRepository { get; set; }



    }
}