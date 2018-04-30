namespace SQLDataAccess.ImportExport
{
    using System;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.ImportExport;
    using BusinessLogic.Interfaces;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.Tables;
    using BusinessLogic.UserDefinedFields;

    /// <summary>
    /// A factory class to create instances of <see cref="IImportFile{T}"/> where 'T' is derived from the Table ID
    /// </summary>
    public class ImportFileFactory
    {
        private readonly FieldRepository _fieldRepository;

        private readonly UserDefinedFieldRepository _userDefinedFieldRepository;

        private readonly TableRepository _tableRepository;

        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        private readonly IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> _projectCodesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportFileFactory"/> class.
        /// </summary>
        /// <param name="fieldRepository">An instance of <see cref="FieldRepository"/></param>
        /// <param name="userDefinedFieldRepository">An instance of <see cref="UserDefinedFieldRepository"/></param>
        /// <param name="tableRepository">An instance of <see cref="TableRepository"/></param>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{T}"/>Where T is a <seealso cref="IProjectCodeWithUserDefinedFields"/></param>
        /// <param name="projectCodesRepository">An instance of <see cref="RepositoryBase{T,TK}"/> Where T is a <seealso cref="IProjectCodeWithUserDefinedFields"/></param>
        public ImportFileFactory(FieldRepository fieldRepository, UserDefinedFieldRepository userDefinedFieldRepository, TableRepository tableRepository, ICustomerDataConnection<SqlParameter> customerDataConnection, IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> projectCodesRepository)
        {
            Guard.ThrowIfNull(fieldRepository, "fieldrepository");
            Guard.ThrowIfNull(userDefinedFieldRepository, "userDefinedFieldRepository");
            Guard.ThrowIfNull(tableRepository, "tableRepository");
            Guard.ThrowIfNull(customerDataConnection, "customerDataConnection");
            Guard.ThrowIfNull(projectCodesRepository, "projectCodesRepository");
            this._fieldRepository = fieldRepository;
            this._userDefinedFieldRepository = userDefinedFieldRepository;
            this._tableRepository = tableRepository;
            this._customerDataConnection = customerDataConnection;
            this._projectCodesRepository = projectCodesRepository;
        }

        /// <summary>
        /// Create a New instance of <see cref="IImportFile{T}"/> where T is derived from the <paramref name="tableId"/>
        /// </summary>
        /// <param name="tableId">The <see cref="Guid"/>ID of the table to import</param>
        /// <returns>A valid instance of <see cref="IImportFile{T}"/> or null if not currently implemented.</returns>
        public IImportFile<IIdentifier<int>> New(Guid tableId)
        {
            if (tableId == BusinessLogic.Constants.Tables.ProjectCodes.TableId)
                return new ImportProjectCodes<IIdentifier<int>>(
                    this._fieldRepository,
                    this._userDefinedFieldRepository,
                    this._tableRepository,
                    this._customerDataConnection,
                    this._projectCodesRepository);

            return null;
        }

    }
}
