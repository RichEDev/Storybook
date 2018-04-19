namespace PublicAPI.Controllers
{
    using System;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Identity;

    using PublicAPI.Security.Filters;
    using PublicAPI.DTO;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IGeneralOptions"/> using the <see cref="GeneralOptionsRequestDTO"/>.
    /// </summary>
    [JwtAuthentication]
    public class GeneralOptionsController : ApiController
    {
        private readonly Lazy<IDataFactory<IGeneralOptions, int>> _generalOptionsFactory = new Lazy<IDataFactory<IGeneralOptions, int>>(() => WebApiApplication.container.GetInstance<IDataFactory<IGeneralOptions, int>>());

        private readonly Lazy<IIdentityProvider> _identity = new Lazy<IIdentityProvider>(() => WebApiApplication.container.GetInstance<IIdentityProvider>());

        private readonly Lazy<IDataFactory<IEmployee, int>> _employeeFactory = new Lazy<IDataFactory<IEmployee, int>>(() => WebApiApplication.container.GetInstance<IDataFactory<IEmployee, int>>());

        /// <summary>
        /// Controller action to generate an instance of <see cref="GeneralOptionsDTO"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/GeneralOptions">https://api.hostname/GeneralOptions</a>
        ///  Body: <see cref="GeneralOptionsRequestDTO"/>
        /// </remarks>
        /// <param name="value">The <see cref="GeneralOptionsRequestDTO"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="GeneralOptionsDTO"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post([FromBody] GeneralOptionsRequestDTO value)
        {
            var identity = this._identity.Value.GetUserIdentity();

            var employee = this._employeeFactory.Value[identity.EmployeeId];

            var generalOptions =
                value.Generate(this._generalOptionsFactory.Value[employee.SubAccountId]);
            GeneralOptionsDTO generalOptionsDto = MapObjects.Map<GeneralOptionsDTO>(generalOptions);

            return this.Json(generalOptionsDto);
        }

        /// <summary>
        /// Controller action to generate an instance of <see cref="GeneralOptionsDTO"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/GeneralOptions">https://api.hostname/GeneralOptions</a>
        ///  Body: <see cref="GeneralOptionsRequestDTO"/>
        /// </remarks>
        /// <param name="value">The <see cref="GeneralOptionsRequestDTO"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="GeneralOptionsDTO"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put([FromBody] GeneralOptionsRequestDTO value)
        {
            var identity = this._identity.Value.GetUserIdentity();

            var employee = this._employeeFactory.Value[identity.EmployeeId];

            var generalOptions =
                value.Generate(this._generalOptionsFactory.Value[employee.SubAccountId]);
            GeneralOptionsDTO generalOptionsDto = MapObjects.Map<GeneralOptionsDTO>(generalOptions);

            return this.Json(generalOptionsDto);
        }

        /// <summary>
        /// Controller action to get all available instances of <see cref="GeneralOptionsRequestDTO"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/GeneralOptions">https://api.hostname/GeneralOptions</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="GeneralOptionsDTO"/>.</returns>
        public IHttpActionResult Get()
        {
            var identity = this._identity.Value.GetUserIdentity();

            var employee = this._employeeFactory.Value[identity.EmployeeId];

            var generalOptions =
                this._generalOptionsFactory.Value[employee.SubAccountId].WithAll();
            GeneralOptionsDTO generalOptionsDto = MapObjects.Map<GeneralOptionsDTO>(generalOptions);

            return this.Json(generalOptionsDto);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="GeneralOptionsDTO"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/GeneralOptions/{id}">https://api.hostname/GeneralOptions/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="GeneralOptionsDTO"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            var generalOptions =
                this._generalOptionsFactory.Value[id].WithAll();
            GeneralOptionsDTO generalOptionsDto = MapObjects.Map<GeneralOptionsDTO>(generalOptions);

            return this.Json(generalOptionsDto);
        }
    }
}