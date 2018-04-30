namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic.Fields;
    using BusinessLogic.UserDefinedFields;

    using Common.Logging;

    using NSubstitute;

    /// <summary>
    /// The "Fields" Mocked objects used in unit tests
    /// </summary>
    public class Fields : IFields
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fields"/> class.
        /// </summary>
        /// <param name="logger">
        /// An instance of <see cref="ILog"/>.
        /// </param>
        public Fields(ILog logger)
        {
            this.UserDefinedFieldRepository = Substitute.For<UserDefinedFieldRepository>(logger);
            this.UserDefinedFieldValueRepository = Substitute.For<UserDefinedFieldValueRepository>();
            this.FieldRepository = Substitute.For<FieldRepository>(logger);
        }

        /// <summary>
        /// Gets or sets the Mock'd <see cref="IFields.UserDefinedFieldRepository"/>
        /// </summary>
        public UserDefinedFieldRepository UserDefinedFieldRepository { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="BusinessLogic.UserDefinedFields.UserDefinedFieldRepository"/>
        /// </summary>
        public UserDefinedFieldValueRepository UserDefinedFieldValueRepository { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IFields.FieldRepository"/>
        /// </summary>
        public FieldRepository FieldRepository { get; set; }

    }
}