namespace SQLDataAccess.ProjectCodes
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.UserDefinedFields;
    using Common.Logging;

    /// <summary>
    /// The sql project codes with user defined values factory.
    /// </summary>
    public class SqlProjectCodesWithUserDefinedValuesFactory : IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>
    {
        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlProjectCodesWithUserDefinedValuesFactory"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Backing instance of project codes factory.
        /// </summary>
        private readonly SqlProjectCodesFactory _projectCodesFactory;

        /// <summary>
        /// The user defined field value repository.
        /// </summary>
        private readonly UserDefinedFieldValueRepository _userDefinedFieldValueRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProjectCodesWithUserDefinedValuesFactory"/> class.
        /// </summary>
        /// <param name="projectCodesFactory">An instance of <see cref="SqlProjectCodesFactory"/> to decorate with user defined fields</param>
        /// <param name="userDefinedFieldValueRepository">An instance of <see cref="UserDefinedFieldValueRepository" /></param>
        /// <param name="customerDataConnection">An instance of <see cref="CustomerDatabaseConnection"/> to use when retrieving userdefined field data.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="projectCodesFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="userDefinedFieldValueRepository"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null" />.</exception>
        public SqlProjectCodesWithUserDefinedValuesFactory(SqlProjectCodesFactory projectCodesFactory, UserDefinedFieldValueRepository userDefinedFieldValueRepository, ICustomerDataConnection<SqlParameter> customerDataConnection, ILog logger)
        {
            Guard.ThrowIfNull(projectCodesFactory, nameof(projectCodesFactory));
            Guard.ThrowIfNull(userDefinedFieldValueRepository, nameof(userDefinedFieldValueRepository));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(logger, nameof(logger));

            this._projectCodesFactory = projectCodesFactory;
            this._userDefinedFieldValueRepository = userDefinedFieldValueRepository;
            this._customerDataConnection = customerDataConnection;
            this._logger = logger;
        }

        /// <summary>
        /// Gets an instance of <see cref="IProjectCodeWithUserDefinedFields"/> with a matching <c>Id</c> from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{T}"/>
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IProjectCodeWithUserDefinedFields"/> you want to retrieve</param>
        /// <returns>The required <see cref="IProjectCodeWithUserDefinedFields"/> or <see langword="null" /> if it cannot be found</returns>
        public IProjectCodeWithUserDefinedFields this[int id] => this.Get(id);

        /// <summary>
        /// Adds a <see cref="IProjectCode">IProjectCode</see> to the database/repository
        /// </summary>
        /// <param name="entity">
        /// The <see cref="IProjectCode">IProjectCode</see> to save
        /// </param>
        /// <returns>
        /// The saved instance of <see cref="IProjectCode">IProjectCode</see>
        /// </returns>
        public IProjectCodeWithUserDefinedFields Add(IProjectCodeWithUserDefinedFields entity)
        {
            IProjectCodeWithUserDefinedFields result = this._projectCodesFactory.Add(entity) as IProjectCodeWithUserDefinedFields;
            
            // Check the Id to ensure it saved correctly (positive id) and was not a duplicate (negative id)/failed save
            if (result != null && result.Id > 0)
            {
                this._userDefinedFieldValueRepository.Save(entity.Id, entity.UserDefinedFieldValues, ModuleElements.ProjectCodes);
            }

            return result;
        }

        /// <summary>
        /// Inverse the archive state of the <see cref="IProjectCodeWithUserDefinedFields"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProjectCodeWithUserDefinedFields"/> to change the Archive state on.</param>
        /// <returns>A bool indicating the current archive state of the <see cref="IProjectCodeWithUserDefinedFields"/> after execution.</returns>
        public bool Archive(int id)
        {
            return this._projectCodesFactory.Archive(id);
        }

        /// <summary>
        /// Deletes the instance of <see cref="IProjectCodeWithUserDefinedFields"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IProjectCodeWithUserDefinedFields"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            return this._projectCodesFactory.Delete(id);
        }

        /// <summary>
        /// Gets an instance of <see cref="IProjectCodeWithUserDefinedFields"/> from <see cref="IDataConnection{T}"/>
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IProjectCodeWithUserDefinedFields"/> you want to retrieve from <see cref="IDataConnection{T}"/></param>
        /// <returns>The required <see cref="IProjectCodeWithUserDefinedFields"/> or <see langword="null"/> if it does not exist in <see cref="IDataConnection{T}"/></returns>
        public IProjectCodeWithUserDefinedFields Get(int id)
        {
            IProjectCode projectCode = this._projectCodesFactory[id];

            if (projectCode == null)
            {
                return null;
            }

            IProjectCodeWithUserDefinedFields projectCodeWithUserDefined = this.Convert(new List<IProjectCode> { projectCode })[0];

            return projectCodeWithUserDefined;
        }

        /// <summary>
        /// Gets an List of all available <see cref="IProjectCodeWithUserDefinedFields"/> 
        /// </summary>    
        /// <returns>The list of <see cref="IProjectCodeWithUserDefinedFields"/>.</returns>
        public IList<IProjectCodeWithUserDefinedFields> Get()
        {
            IList<IProjectCode> projectCodes = this._projectCodesFactory.Get();

            return this.Convert(projectCodes);
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IProjectCodeWithUserDefinedFields"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IProjectCodeWithUserDefinedFields"/> that match <paramref name="predicate"/>.</returns>
        public IList<IProjectCodeWithUserDefinedFields> Get(Predicate<IProjectCodeWithUserDefinedFields> predicate)
        {
            IList<IProjectCodeWithUserDefinedFields> projectCodes = this.Get();

            if (predicate == null)
            {
                return projectCodes;
            }

            List<IProjectCodeWithUserDefinedFields> matchedProjectCodes = new List<IProjectCodeWithUserDefinedFields>();

            foreach (IProjectCodeWithUserDefinedFields projectCode in projectCodes)
            {
                if (predicate.Invoke(projectCode))
                {
                    matchedProjectCodes.Add(projectCode);
                }
            }

            return matchedProjectCodes;
        }

        /// <summary>
        /// Gets an instance of <see cref="IProjectCodeWithUserDefinedFields"/> if it matches the supplied <paramref name="customGet"/>.
        /// </summary>
        /// <param name="customGet">An instance of <see cref="GetByCustom"/> to retrieve a <see cref="IProjectCodeWithUserDefinedFields"/> that matches the defined rules.</param>
        /// <returns>An instance of <see cref="IProjectCodeWithUserDefinedFields"/> that matches the rules defined in <paramref name="customGet"/>.</returns>
        public IProjectCodeWithUserDefinedFields GetByCustom(GetByCustom customGet)
        {
            IProjectCode projectCode = this._projectCodesFactory.GetByCustom(customGet);

            if (projectCode == null)
            {
                return null;
            }

            return this.Convert(new List<IProjectCode> { projectCode })[0];
        }

        /// <summary>
        /// Converts instances of <see cref="IProjectCode"/> to <see cref="IProjectCodeWithUserDefinedFields"/>.
        /// </summary>
        /// <param name="projectCodes">The collection of <see cref="IProjectCode"/> to convert.</param>
        /// <returns>A collection of <see cref="IProjectCodeWithUserDefinedFields"/> converted from <paramref name="projectCodes"/>.</returns>
        private IList<IProjectCodeWithUserDefinedFields> Convert(IList<IProjectCode> projectCodes)
        {
            IList<IProjectCodeWithUserDefinedFields> convertedProjectCodes = new List<IProjectCodeWithUserDefinedFields>();


            // TODO Make the udf values cachable so we can retrieve those and populate the ProjectCodeWithUserDefinedFields from cache/memory if available
            string sql = @"SELECT userdefinedProjectcodes.* FROM userdefinedProjectcodes";

            if (projectCodes.Count == 1)
            {
                sql += " WHERE userdefinedProjectcodes.projectcodeid = @projectCodeID";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@projectCodeID", SqlDbType.Int) { Value = projectCodes[0].Id });
            }

            Dictionary<int, UserDefinedFieldValueCollection> userDefinedFieldValues = new Dictionary<int, UserDefinedFieldValueCollection>();

            using (DbDataReader reader = this._customerDataConnection.GetReader(sql))
            {
                while (reader.Read())
                {
                    int userDefinedAvailableOrdinal = reader.GetOrdinal("projectCodeId");
                    
                    // Check if there are any userdefined fields                
                    if (!reader.IsDBNull(userDefinedAvailableOrdinal))
                    {
                        int projectCodeId = reader.GetInt32(userDefinedAvailableOrdinal);
                        // All columns after userdefinedAvailable + 1 are userdefined.
                        var i = userDefinedAvailableOrdinal + 1;
                        while (i < reader.FieldCount)
                        {
                            var columnId = string.Join(string.Empty, reader.GetName(i).Where(char.IsDigit));

                            int parsedColumnId;
                            if (int.TryParse(columnId, out parsedColumnId))
                            {
                                if (userDefinedFieldValues.ContainsKey(projectCodeId) == false)
                                {
                                    userDefinedFieldValues.Add(projectCodeId, new UserDefinedFieldValueCollection());    
                                }

                                userDefinedFieldValues[projectCodeId].Add(new KeyValuePair<int, object>(parsedColumnId, reader.GetValue(i)));
                            }

                            i++;
                        }
                    }
                }
            }

            this._customerDataConnection.Parameters.Clear();

            foreach (IProjectCode projectCode in projectCodes)
            {
                ProjectCodeWithUserDefinedFields projectCodeWithUserDefined = new ProjectCodeWithUserDefinedFields(projectCode, userDefinedFieldValues.ContainsKey(projectCode.Id) ? userDefinedFieldValues[projectCode.Id] : new UserDefinedFieldValueCollection());
                convertedProjectCodes.Add(projectCodeWithUserDefined);
            }
            
            return convertedProjectCodes;
        }
    }
}
