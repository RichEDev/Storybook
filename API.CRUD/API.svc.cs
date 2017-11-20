namespace ApiCrud
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using System.ServiceModel;
    using ApiCrud.DataAccess;
    using ApiCrud.Interfaces;
    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using Utilities.DistributedCaching;

    /// <summary>
    /// The API service .
    /// </summary>
    public class ApiService : IApiCrud
    {
        /// <summary>
        /// The cache.
        /// </summary>
        private static Cache cache;

        /// <summary>
        /// The service url.
        /// </summary>
        private readonly string serviceUrl;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log logger;

        /// <summary>
        /// The account id.
        /// </summary>
        private int accountId;

        /// <summary>
        /// The entity id.
        /// </summary>
        private long entityId;

        /// <summary>
        /// Initialises static members of the <see cref="ApiService"/> class.
        /// </summary>
        static ApiService()
        {
            cache = new Cache();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiService"/> class.
        /// Gets the URI used to active the service and stores it for use in formatting return URI.
        /// </summary>
        public ApiService()
        {
            if (this.serviceUrl == string.Empty)
            {
                if (OperationContext.Current != null)
                {
                    var via = OperationContext.Current.IncomingMessageProperties.Via;
                    this.serviceUrl = string.Format("{0}://{1}{2}", via.Scheme, via.Host, via.AbsolutePath);
                }
                else
                {
                    this.serviceUrl = "/unittest/";
                }
            }

            this.logger = new Log();
            this.logger.WriteExtra(string.Empty, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Starting {1}", "Api Crud Service", DateTime.Now), "ApiCrud");
            if (cache == null)
            {
                cache = new Cache();
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiService"/> class.
        /// with an IAPIDBCONNECTION object passed as a parameter
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public ApiService(ApiLibrary.Interfaces.IApiDbConnection connection)
        {
            if (this.serviceUrl == string.Empty)
            {
                if (OperationContext.Current != null)
                {
                    var via = OperationContext.Current.IncomingMessageProperties.Via;
                    this.serviceUrl = string.Format("{0}://{1}{2}", via.Scheme, via.Host, via.AbsolutePath);
                }
                else
                {
                    this.serviceUrl = "/unittest/";
                }
            }

            this.logger = new Log(connection);
            this.logger.WriteExtra(string.Empty, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Starting {1}", "Service Starting", DateTime.Now), "ApiCrud");
        }

        /// <summary>
        /// Gets or sets the API DB connection.
        /// </summary>
        public IApiDbConnection ApiDbConnection
        {
            get;
            set;
        }

        #region Employees

        /// <summary>
        /// Create employees from list.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employees">
        /// The employees.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Employee> CreateEmployees(string metabase, string accountid, List<Employee> employees)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Employee>();

            if (this.ConvertParams(accountid))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEmployees", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employee = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employee.Create(employees);

                this.CloseConnection();
                this.ClearCache(result);
            }

            return result;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        public Employee ReadEmployee(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new Employee();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployee", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employees = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employees.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read employee by user name.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee ReadEmployeeByUsername(string metabase, string accountid, string userName)
        {
            this.CheckMetaBase(metabase);
            var result = new Employee();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeByUsername", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employees = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employees.ReadSpecial(userName).FirstOrDefault();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<Employee> ReadAllEmployees(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Employee>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "RealAllEmployees", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employees = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employees.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employees">
        /// The employees.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<Employee> UpdateEmployees(string metabase, string accountid, List<Employee> employees)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Employee>();
            if (this.ConvertParams(accountid))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEmployees", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employee = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employee.Create(employees);

                this.CloseConnection();
                this.ClearCache(result);
            }

            return result;
        }

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee DeleteEmployees(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new Employee();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEmployee", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employees = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employees.Delete(this.entityId);

                this.CloseConnection();
                this.ClearCache(new List<Employee> { result });
            }

            return result;
        }

        /// <summary>
        /// The read employee by person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Employee> ReadEmployeeByPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Employee>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeByPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var employees = DataAccessFactory.CreateDataAccess<Employee>(this.serviceUrl, this.ApiDbConnection);
                result = employees.ReadByEsrId(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region ESR Person

        /// <summary>
        /// The create ESR persons method.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPersons">
        /// The ESR persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPerson> CreateEsrPersons(string metabase, string accountid, List<EsrPerson> esrPersons)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPerson>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrPersons", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPerson = DataAccessFactory.CreateDataAccess<EsrPerson>(this.serviceUrl, this.ApiDbConnection);
                result = esrPerson.Create(esrPersons);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPerson ReadEsrPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrPerson();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPerson = DataAccessFactory.CreateDataAccess<EsrPerson>(this.serviceUrl, this.ApiDbConnection);
                result = esrPerson.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPerson> ReadAllEsrPerson(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPerson>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "RealAllEsrPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPerson = DataAccessFactory.CreateDataAccess<EsrPerson>(this.serviceUrl, this.ApiDbConnection);
                result = esrPerson.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPersons">
        /// The ESR persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPerson> UpdateEsrPersons(string metabase, string accountid, List<EsrPerson> esrPersons)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPerson>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPersons", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPerson = DataAccessFactory.CreateDataAccess<EsrPerson>(this.serviceUrl, this.ApiDbConnection);
                result = esrPerson.Update(esrPersons);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The ESR Person id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPerson DeleteEsrPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrPerson();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPerson = DataAccessFactory.CreateDataAccess<EsrPerson>(this.serviceUrl, this.ApiDbConnection);
                result = esrPerson.Delete(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region ESR Assignment

        /// <summary>
        /// Create ESR assignments.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignments">
        /// The ESR assignments.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> CreateEsrAssignments(string metabase, string accountid, List<EsrAssignment> esrAssignments)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAssignment>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrAssignments", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                result = esrAssignment.Create(esrAssignments);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR assignment.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id of the record to read.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment ReadEsrAssignment(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrAssignment", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                var result = esrAssignment.Read(this.entityId);

                this.CloseConnection();

                return result;
            }

            return new EsrAssignment
            {
                ActionResult = new ApiResult
                {
                    Result = ApiActionResult.Failure
                }
            };
        }

        /// <summary>
        /// The read ESR assignment by employee.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public List<EsrAssignment> ReadEsrAssignmentByPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrAssignmentByPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                var result = esrAssignment.ReadByEsrId(this.entityId);

                this.CloseConnection();

                return result;
            }

            return new List<EsrAssignment>
                       {
                           new EsrAssignment
                               {
                                   ActionResult =
                                       new ApiResult
                                           {
                                               Result =
                                                   ApiActionResult.Failure
                                           }
                               }
                       };
        }

        /// <summary>
        /// The read all ESR assignment records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> ReadAllEsrAssignment(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrAssignment", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                var result = esrAssignment.ReadAll();

                this.CloseConnection();

                return result;
            }

            return new List<EsrAssignment>();
        }

        /// <summary>
        /// The read ESR assignment by assignment.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrAssignment> ReadEsrAssignmentByAssignment(string metaBase, string accountId, string reference)
        {
            this.CheckMetaBase(metaBase);
            if (this.ConvertParams(accountId, string.Empty))
            {
                this.logger.WriteDebug(metaBase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrAssignmentByAssignment", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metaBase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                var result = esrAssignment.ReadSpecial(reference);

                this.CloseConnection();

                return result;
            }

            return new List<EsrAssignment>();
        }

        /// <summary>
        /// The update ESR assignments.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignments">
        /// The ESR assignments.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> UpdateEsrAssignments(string metabase, string accountid, List<EsrAssignment> esrAssignments)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAssignment>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrAssignments", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                result = esrAssignment.Update(esrAssignments);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR assignment.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The ESR assignment id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment DeleteEsrAssignment(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrAssignment", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrAssignment = DataAccessFactory.CreateDataAccess<EsrAssignment>(this.serviceUrl, this.ApiDbConnection);
                var result = esrAssignment.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return new EsrAssignment
            {
                ActionResult = new ApiResult
                {
                    Result = ApiActionResult.Failure
                }
            };
        }

        #endregion

        #region ESR Vehicle

        /// <summary>
        /// The create ESR vehicles.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrVehicles">
        /// The ESR vehicles.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrVehicle> CreateEsrVehicles(string metabase, string accountid, List<EsrVehicle> esrVehicles)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrVehicle>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrVehicles", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrVehicle = DataAccessFactory.CreateDataAccess<EsrVehicle>(this.serviceUrl, this.ApiDbConnection);
                result = esrVehicle.Create(esrVehicles);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR vehicle.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle ReadEsrVehicle(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrVehicle();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrVehicle", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrVehicle = DataAccessFactory.CreateDataAccess<EsrVehicle>(this.serviceUrl, this.ApiDbConnection);
                result = esrVehicle.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR vehicle.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrVehicle> ReadAllEsrVehicle(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrVehicle>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrVehicle", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrVehicle = DataAccessFactory.CreateDataAccess<EsrVehicle>(this.serviceUrl, this.ApiDbConnection);
                result = esrVehicle.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR vehicle by person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrVehicle> ReadEsrVehicleByPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrVehicle>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrVehicleByPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrVehicle = DataAccessFactory.CreateDataAccess<EsrVehicle>(this.serviceUrl, this.ApiDbConnection);
                result = esrVehicle.ReadByEsrId(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR vehicles.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrVehicles">
        /// The ESR vehicles.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrVehicle> UpdateEsrVehicles(string metabase, string accountid, List<EsrVehicle> esrVehicles)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrVehicle>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrVehicles", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrVehicle = DataAccessFactory.CreateDataAccess<EsrVehicle>(this.serviceUrl, this.ApiDbConnection);
                result = esrVehicle.Update(esrVehicles);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR vehicle.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle DeleteEsrVehicle(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrVehicle", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrVehicle = DataAccessFactory.CreateDataAccess<EsrVehicle>(this.serviceUrl, this.ApiDbConnection);
                var result = esrVehicle.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return new EsrVehicle();
        }

        #endregion

        #region ESR Phone

        /// <summary>
        /// The create ESR phones.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPhones">
        /// The ESR phones.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPhone> CreateEsrPhones(string metabase, string accountid, List<EsrPhone> esrPhones)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPhone>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrPhones", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrPhone>(this.serviceUrl, this.ApiDbConnection);
                result = esrPhone.Create(esrPhones);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR phone.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone ReadEsrPhone(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrPhone();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrPhone", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrPhone>(this.serviceUrl, this.ApiDbConnection);
                result = esrPhone.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR phone.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPhone> ReadAllEsrPhone(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPhone>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrPhone", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrPhone>(this.serviceUrl, this.ApiDbConnection);
                result = esrPhone.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR phone by person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPhone> ReadEsrPhoneByPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPhone>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrPhoneByPerson", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrPhone>(this.serviceUrl, this.ApiDbConnection);
                result = esrPhone.ReadByEsrId(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR phones.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPhones">
        /// The ESR phones.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPhone> UpdateEsrPhones(string metabase, string accountid, List<EsrPhone> esrPhones)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPhone>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPhones", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrPhone>(this.serviceUrl, this.ApiDbConnection);
                result = esrPhone.Update(esrPhones);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR phone.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone DeleteEsrPhone(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrPhone", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrPhone>(this.serviceUrl, this.ApiDbConnection);
                var result = esrPhone.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }
        #endregion

        #region ESR Address

        /// <summary>
        /// The create ESR address.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAddress">
        /// The ESR address.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> CreateEsrAddresss(string metabase, string accountid, List<EsrAddress> esrAddress)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(this.serviceUrl, this.ApiDbConnection);
                result = addressDataAccess.Create(esrAddress);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR address.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress ReadEsrAddress(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrAddress();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(this.serviceUrl, this.ApiDbConnection);
                result = addressDataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR address by person.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> ReadEsrAddressByPerson(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAddress>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(this.serviceUrl, this.ApiDbConnection);
                result = addressDataAccess.ReadByEsrId(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR address.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> ReadAllEsrAddress(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(this.serviceUrl, this.ApiDbConnection);
                result = addressDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR address.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAddress">
        /// The ESR address.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> UpdateEsrAddresss(string metabase, string accountid, List<EsrAddress> esrAddress)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(this.serviceUrl, this.ApiDbConnection);
                result = addressDataAccess.Update(esrAddress);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR address.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress DeleteEsrAddress(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var esrPhone = DataAccessFactory.CreateDataAccess<EsrAddress>(this.serviceUrl, this.ApiDbConnection);
                var result = esrPhone.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }
        #endregion

        #region EsrAssignmentCostings

        /// <summary>
        /// The create ESR assignment COSTINGS.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignmentCostings">
        /// The ESR assignment COSTINGS.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignmentCostings> CreateEsrAssignmentCostingss(string metabase, string accountid, List<EsrAssignmentCostings> esrAssignmentCostings)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAssignmentCostings>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrAssignmentCostings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var assignmentCostingsDataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(this.serviceUrl, this.ApiDbConnection);
                result = assignmentCostingsDataAccess.Create(esrAssignmentCostings);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR ASSIGNMENT COSTINGS.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentCostings"/>.
        /// </returns>
        public EsrAssignmentCostings ReadEsrAssignmentCostings(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrAssignmentCostings();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", string.Format("ReadEsrAssignmentCostings - {0}", id), DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var assignmentCostingsDataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(this.serviceUrl, this.ApiDbConnection);
                result = assignmentCostingsDataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        public List<EsrAssignmentCostings> ReadEsrAssignmentCostingsByPerson(string metabase, string accountid, string personId)
        {
            this.CheckMetaBase(metabase);
            if (!this.ConvertParams(accountid, personId))
            {
                return new List<EsrAssignmentCostings> { new EsrAssignmentCostings { ActionResult = new ApiResult { Result = ApiActionResult.Failure } } };
            }

            this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrAssignmentCostingsByPerson", DateTime.Now), "ApiCrud");

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
            }

            try
            {
                var esrAssignmentCostings = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(this.serviceUrl, this.ApiDbConnection);
                return esrAssignmentCostings.ReadByEsrId(this.entityId);
            }
            finally
            {
                this.CloseConnection();
            }
        }

        /// <summary>
        /// The read all ESR ASSIGNMENT COSTINGS.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignmentCostings> ReadAllEsrAssignmentCostings(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAssignmentCostings>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrAssignmentCostings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var assignmentCostingsDataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(this.serviceUrl, this.ApiDbConnection);
                result = assignmentCostingsDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR ASSIGNMENT COSTINGS.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignmentCostings">
        /// The ESR ASSIGNMENT COSTINGS.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignmentCostings> UpdateEsrAssignmentCostingss(string metabase, string accountid, List<EsrAssignmentCostings> esrAssignmentCostings)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAssignmentCostings>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrAssignmentCostings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var assignmentCostingsDataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(this.serviceUrl, this.ApiDbConnection);
                result = assignmentCostingsDataAccess.Update(esrAssignmentCostings);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR ASSIGNMENT COSTINGS.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentCostings"/>.
        /// </returns>
        public EsrAssignmentCostings DeleteEsrAssignmentCostings(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrAssignmentCostings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var assignmentCostingsDataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(this.serviceUrl, this.ApiDbConnection);
                var result = assignmentCostingsDataAccess.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }

        #endregion

        #region EsrAssignmentLocations
        public List<EsrAssignmentLocation> CreateAssignmentLocations(string metabase, string accountid, List<EsrAssignmentLocation> esrAssignmentLocations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrAssignmentLocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrAssignmentLocations", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentLocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(esrAssignmentLocations);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region EsrElementFields

        /// <summary>
        /// The create ESR element fields.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElementFields">
        /// The ESR Element Fields.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElementFields> CreateEsrElementFieldss(string metabase, string accountid, List<EsrElementFields> esrElementFields)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElementFields>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrElementFields", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementFields>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Create(esrElementFields);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR element fields.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementFields"/>.
        /// </returns>
        public EsrElementFields ReadEsrElementFields(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrElementFields();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrElementFields", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementFields>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR element fields.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElementFields> ReadAllEsrElementFields(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElementFields>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrElementFields", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementFields>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR element fields.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElementFields">
        /// The ESR Element Fields.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElementFields> UpdateEsrElementFieldss(string metabase, string accountid, List<EsrElementFields> esrElementFields)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElementFields>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrElementFieldss", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementFields>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Update(esrElementFields);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR element fields.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementFields"/>.
        /// </returns>
        public EsrElementFields DeleteEsrElementFields(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrElementFields", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementFields>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }

        #endregion

        #region EsrElements

        /// <summary>
        /// The create ESR elements.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElements">
        /// The ESR elements.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElement> CreateEsrElements(string metabase, string accountid, List<EsrElement> esrElements)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElement>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrElements", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Create(esrElements);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR element.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElement"/>.
        /// </returns>
        public EsrElement ReadEsrElement(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrElement();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrElement", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR element.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElement> ReadAllEsrElement(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElement>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrElement", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR elements.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElements">
        /// The ESR elements.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElement> UpdateEsrElements(string metabase, string accountid, List<EsrElement> esrElements)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElement>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrElement", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Update(esrElements);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR element.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElement"/>.
        /// </returns>
        public EsrElement DeleteEsrElement(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrElement", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }

        #endregion

        #region EsrTrusts

        /// <summary>
        /// The create ESR trusts.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrTrusts">
        /// The ESR trusts.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List{T}</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrTrust> CreateEsrTrusts(string metabase, string accountid, List<EsrTrust> esrTrusts)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrTrust>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrTrusts", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrTrust>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(esrTrusts);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR trust.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust ReadEsrTrust(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrTrust();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrTrust", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrTrust>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR trust.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List{T}</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrTrust> ReadAllEsrTrust(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrTrust>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrTrust", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrTrust>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR trusts.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrTrusts">
        /// The ESR trusts.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<EsrTrust> UpdateEsrTrusts(string metabase, string accountid, List<EsrTrust> esrTrusts)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrTrust>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrTrusts", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrTrust>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(esrTrusts);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR trust.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust DeleteEsrTrust(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrTrust", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrTrust>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }

        /// <summary>
        /// The find ESR trust in a given meta base by id.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="esrTrust">
        /// The ESR Trust.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// Account that contains the NHS VPD
        /// </returns>
        public EsrTrust FindEsrTrust(string metaBase, string esrTrust)
        {
            this.CheckMetaBase(metaBase);
            int vpd;
            int.TryParse(esrTrust, out vpd);
            this.logger.WriteDebug(metaBase, vpd, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "FindEsrTrust", DateTime.Now), "ApiCrud");
            using (var apiData = this.ApiDbConnection ?? new ApiDbConnection(metaBase))
            {
                var accountList = apiData.GetAccounts();

                if (accountList != null)
                {
                    foreach (Account account in accountList)
                    {
                        var result = this.ReadAllEsrTrust(
                            metaBase, account.AccountId.ToString(CultureInfo.InvariantCulture));
                        if (result.Any(trust => trust.trustVPD == esrTrust))
                        {
                            EsrTrust foundTrust = result.Find(trust => trust.trustVPD == esrTrust);
                            foundTrust.AccountId = account.AccountId;
                            return foundTrust;
                        }
                    }
                }
            }

            return new EsrTrust();
        }

        #endregion

        #region EsrLocations

        /// <summary>
        /// The create ESR locations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrLocations">
        /// The ESR locations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrLocation> CreateEsrLocations(string metabase, string accountid, List<EsrLocation> esrLocations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrLocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrLocations", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(esrLocations);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR location.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation ReadEsrLocation(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrLocation();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrLocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR location.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrLocation> ReadAllEsrLocation(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrLocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrLocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR locations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrLocations">
        /// The ESR locations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrLocation> UpdateEsrLocations(string metabase, string accountid, List<EsrLocation> esrLocations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrLocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrLocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(esrLocations);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete ESR location.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation DeleteEsrLocation(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrLocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return null;
        }
        #endregion

        #region EsrOrganisation

        /// <summary>
        /// The create ESR organisations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrOrganisations">
        /// The ESR organisations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> CreateEsrOrganisations(string metabase, string accountid, List<EsrOrganisation> esrOrganisations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrOrganisation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrOrganisations", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(esrOrganisations);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read ESR organisation.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation ReadEsrOrganisation(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrOrganisation();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrOrganisation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all ESR organisation.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> ReadAllEsrOrganisation(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrOrganisation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrOrganisation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update ESR organisations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrOrganisations">
        /// The ESR organisations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> UpdateEsrOrganisations(string metabase, string accountid, List<EsrOrganisation> esrOrganisations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrOrganisation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrOrganisation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(esrOrganisations);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete ESR organisation.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation DeleteEsrOrganisation(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrOrganisation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return null;
        }
        #endregion

        #region EsrPosition

        /// <summary>
        /// The create ESR positions.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPositions">
        /// The ESR positions.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> CreateEsrPositions(string metabase, string accountid, List<EsrPosition> esrPositions)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPosition>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrPositions", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(esrPositions);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read ESR position.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition ReadEsrPosition(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrPosition();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrPosition", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all ESR position.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> ReadAllEsrPosition(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPosition>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEsrPosition", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update ESR positions.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPositions">
        /// The ESR positions.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> UpdateEsrPositions(string metabase, string accountid, List<EsrPosition> esrPositions)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrPosition>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPositions", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(esrPositions);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete ESR position.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition DeleteEsrPosition(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrPositions", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return null;
        }
        #endregion

        #region EsrElementSubCat

        /// <summary>
        /// The create ESR ElementSubCats.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElementSubCats">
        /// The ESR ElementSubCats.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElementSubCat> CreateEsrElementSubCats(string metabase, string accountid, List<EsrElementSubCat> esrElementSubCats)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElementSubCat>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEsrElementSubCats", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Create(esrElementSubCats);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read ESR ElementSubCat.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementSubCat"/>.
        /// </returns>
        public EsrElementSubCat ReadEsrElementSubCat(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EsrElementSubCat();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEsrElementSubCat", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all ESR ElementSubCat.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElementSubCat> ReadAllEsrElementSubCat(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElementSubCat>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllElementSubCat", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update ESR ElementSubCats.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElementSubCats">
        /// The ESR ElementSubCats.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrElementSubCat> UpdateEsrElementSubCats(string metabase, string accountid, List<EsrElementSubCat> esrElementSubCats)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EsrElementSubCat>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrElementSubCats", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(this.serviceUrl, this.ApiDbConnection);
                result = elementFieldsDataAccess.Update(esrElementSubCats);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete ESR ElementSubCat.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementSubCat"/>.
        /// </returns>
        public EsrElementSubCat DeleteEsrElementSubCat(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEsrElementSubCat", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var elementFieldsDataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(this.serviceUrl, this.ApiDbConnection);
                var result = elementFieldsDataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return null;
        }
        #endregion

        #region Car

        /// <summary>
        /// The create cars.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="cars">
        /// The cars.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Car> CreateCars(string metabase, string accountid, List<Car> cars)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Car>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateCars", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Car>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(cars);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read car.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Car"/>.
        /// </returns>
        public Car ReadCar(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new Car();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadCar", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Car>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all car.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Car> ReadAllCar(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Car>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllCar", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Car>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read car by ESR id.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Car> ReadCarByEsrId(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Car>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadCarByEsrId", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Car>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadByEsrId(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update cars.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="cars">
        /// The cars.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Car> UpdateCars(string metabase, string accountid, List<Car> cars)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Car>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateCars", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Car>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(cars);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete car.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Car"/>.
        /// </returns>
        public Car DeleteCar(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteCar", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Car>(this.serviceUrl, this.ApiDbConnection);
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return null;
        }

        #endregion

        #region Misc

        /// <summary>
        /// The read all fields for imports.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<Field> ReadAllFields(string metabase, string accountID)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Field>();
            if (this.ConvertParams(accountID, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllFields", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var fieldsDataAccess = DataAccessFactory.CreateDataAccess<Field>(this.serviceUrl, this.ApiDbConnection);
                var interimResult = fieldsDataAccess.ReadAll();
                foreach (Field field in interimResult)
                {
                    switch (field.tablename)
                    {
                        case "ESRLocations":
                            field.ElementType = TemplateMapping.ImportElementType.Location;
                            break;
                        case "ESRAssignmentCostings":
                            field.ElementType = TemplateMapping.ImportElementType.Costing;
                            break;
                        case "employees":
                            field.ElementType = TemplateMapping.ImportElementType.Employee;
                            break;
                        case "ESRPersons":
                            field.ElementType = TemplateMapping.ImportElementType.Employee;
                            break;
                        case "ESRVehicles":
                            field.ElementType = TemplateMapping.ImportElementType.Vehicle;
                            break;
                        case "esr_assignments":
                            field.ElementType = TemplateMapping.ImportElementType.Assignment;
                            break;
                        case "esrTrusts":
                            field.ElementType = TemplateMapping.ImportElementType.None;
                            break;
                        case "ESRPhones":
                            field.ElementType = TemplateMapping.ImportElementType.Phone;
                            break;
                        case "ESRAddresses":
                            field.ElementType = TemplateMapping.ImportElementType.Address;
                            break;
                        case "ESRPositions":
                            field.ElementType = TemplateMapping.ImportElementType.Position;
                            break;
                        case "ESRElementSubcats":
                            field.ElementType = TemplateMapping.ImportElementType.None;
                            break;
                        case "ESROrganisations":
                            field.ElementType = TemplateMapping.ImportElementType.Organisation;
                            break;
                    }

                    if (field.ElementType != TemplateMapping.ImportElementType.None)
                    {
                        result.Add(field);
                    }
                }

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all User Defined Fields.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<UserDefinedField> ReadAllUdf(string metabase, string accountId)
        {
            this.CheckMetaBase(metabase);
            var result = new List<UserDefinedField>();
            if (this.ConvertParams(accountId, string.Empty))
            {
                this.logger.WriteDebug(
                    metabase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "ReadAllUdf", DateTime.Now),
                    "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<UserDefinedField>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The create UDF value.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<UserDefinedField> CreateUdfValue(string metabase, string accountId, List<UserDefinedField> entity)
        {
            this.CheckMetaBase(metabase);
            var result = new List<UserDefinedField>();
            if (this.ConvertParams(accountId, string.Empty))
            {
                this.logger.WriteDebug(
                    metabase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "CreateUdfValue", DateTime.Now),
                    "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<UserDefinedField>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(entity);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all account properties.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<AccountProperty> ReadAllAccountProperties(string metabase, string accountId)
        {
            this.CheckMetaBase(metabase);
            var result = new List<AccountProperty>();
            if (this.ConvertParams(accountId, string.Empty))
            {
                this.logger.WriteDebug(
                    metabase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "ReadAllAccountProperties", DateTime.Now),
                    "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<AccountProperty>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read lookup value.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="tableid">
        /// The table id.
        /// </param>
        /// <param name="fieldid">
        /// The field id.
        /// </param>
        /// <param name="keyvalue">
        /// The key value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public Lookup ReadLookupValue(string metaBase, string accountId, string tableid, string fieldid, string keyvalue)
        {
            this.CheckMetaBase(metaBase);
            var result = new Lookup();
            if (this.ConvertParams(accountId, string.Empty))
            {
                this.logger.WriteDebug(
                    metaBase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "ReadLookupValue", DateTime.Now),
                    "ApiCrud");
                using (var dataAccess = this.ApiDbConnection ?? new ApiDbConnection(metaBase, this.accountId))
                {
                    if (dataAccess.ConnectionStringValid)
                    {
                        dataAccess.Sqlexecute.Parameters.AddWithValue("@tableid", tableid);
                        dataAccess.Sqlexecute.Parameters.AddWithValue("@fieldid", fieldid);
                        dataAccess.Sqlexecute.Parameters.AddWithValue("@lookupValue", keyvalue);
                        using (IDataReader reader = dataAccess.GetStoredProcReader("APILookupValue"))
                        {
                            if (string.IsNullOrEmpty(dataAccess.ErrorMessage))
                            {
                                var schemaTable = reader.GetSchemaTable();
                                if (schemaTable != null)
                                {
                                    result.FirstColumnName = schemaTable.Rows[0].ItemArray[0] as string;
                                    if (schemaTable.Rows.Count > 1)
                                    {
                                        result.SecondColumnName = schemaTable.Rows[1].ItemArray[0] as string;
                                    }
                                }

                                if (reader.Read())
                                {
                                    var firstColumn = reader.GetValue(0);
                                    if (firstColumn is int)
                                    {
                                        result.FirstColumnValue = firstColumn as int?;
                                    }
                                    else
                                    {
                                        result.FirstColumnValue = firstColumn as long?;
                                    }
                                    
                                    if (reader.FieldCount == 2)
                                    {
                                        var secondColumn = reader.GetValue(1);
                                        if (secondColumn is string)
                                        {
                                            result.SecondColumnValue = null;
                                        }
                                        else if (secondColumn is int)
                                        {
                                            result.SecondColumnValue = secondColumn as int?;
                                        }
                                        else
                                        {
                                            result.SecondColumnValue = secondColumn as long?;    
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Templates

        public Template ReadTemplate(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new Template();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadTemplates", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Template>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region Templates

        public Template ReadTemplate(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new Template();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadTemplates", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Template>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region Template Mappings

        /// <summary>
        /// The create template mappings.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="templateMappings">
        /// The template mappings.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<TemplateMapping> CreateTemplateMappings(string metabase, string accountid, List<TemplateMapping> templateMappings)
        {
            this.CheckMetaBase(metabase);
            var result = new List<TemplateMapping>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateTemplateMappings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<TemplateMapping>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(templateMappings);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read template mapping.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping ReadTemplateMapping(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new TemplateMapping();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadTemplateMappings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<TemplateMapping>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all template mapping.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<TemplateMapping> ReadAllTemplateMapping(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<TemplateMapping>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllTemplateMappings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<TemplateMapping>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update template mappings.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="templateMappings">
        /// The template mappings.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<TemplateMapping> UpdateTemplateMappings(string metabase, string accountid, List<TemplateMapping> templateMappings)
        {
            this.CheckMetaBase(metabase);
            var result = new List<TemplateMapping>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateTemplateMappings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<TemplateMapping>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(templateMappings);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete template mapping.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping DeleteTemplateMapping(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteTemplateMappings", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var templateDataAccess = DataAccessFactory.CreateDataAccess<TemplateMapping>(this.serviceUrl, this.ApiDbConnection);
                var result = templateDataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return null;
        }

        #endregion

        #region Addresses

        /// <summary>
        /// Create addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="addresses">
        /// The Addresses.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Address> CreateAddresses(string metabase, string accountid, List<Address> addresses)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Address>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Address>(this.serviceUrl, this.ApiDbConnection);
                var currentAddresses = new List<Address>();
                foreach (Address address in addresses)
                {
                    Address singleAddress = null;
                    if (address.EsrLocationID != null)
                    {
                        singleAddress = dataAccess.ReadByEsrId((int)address.EsrLocationID).FirstOrDefault();
                    }

                    if (address.EsrAddressID != null)
                    {
                        singleAddress = dataAccess.ReadByEsrId((int)address.EsrAddressID).FirstOrDefault();
                    }

                    if (singleAddress != null)
                    {
                        address.AddressID = singleAddress.AddressID;
                    }

                    currentAddresses.Add(address);
                }

                result = dataAccess.Create(currentAddresses);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read Address.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Address"/>.
        /// </returns>
        public Address ReadAddress(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new Address();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Address>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Address> ReadAllAddresses(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Address>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Address>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="addresses">
        /// The Addresses.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Address> UpdateAddresses(string metabase, string accountid, List<Address> addresses)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Address>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Address>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(addresses);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Address"/>.
        /// </returns>
        public Address DeleteAddresses(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Address>(this.serviceUrl, this.ApiDbConnection);
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return new Address();
        }

        /// <summary>
        /// The read Address by ESR.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Address> ReadAddressByEsr(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<Address>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAddressByEsr", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<Address>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadByEsrId(this.entityId);

                this.CloseConnection();
            }

            return result;
        }
        #endregion

        #region EmployeeHomeAddresses

        /// <summary>
        /// The create employee home Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeHomeAddresses">
        /// The employee home Addresses.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeHomeAddress> CreateEmployeeHomeAddresses(string metabase, string accountid, List<EmployeeHomeAddress> employeeHomeAddresses)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeHomeAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEmployeeHomeAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(employeeHomeAddresses);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read employee home Address.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeHomeAddress"/>.
        /// </returns>
        public EmployeeHomeAddress ReadEmployeeHomeAddress(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EmployeeHomeAddress();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeHomeAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all employee home Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeHomeAddress> ReadAllEmployeeHomeAddresses(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeHomeAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEmployeeHomeAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update employee home Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeHomeAddresses">
        /// The employee home Addresses.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeHomeAddress> UpdateEmployeeHomeAddresses(string metabase, string accountid, List<EmployeeHomeAddress> employeeHomeAddresses)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeHomeAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEmployeeHomeAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(employeeHomeAddresses);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete employee home Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeHomeAddress"/>.
        /// </returns>
        public EmployeeHomeAddress DeleteEmployeeHomeAddresses(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEmployeeHomeAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(this.serviceUrl, this.ApiDbConnection);
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return new EmployeeHomeAddress();
        }

        #endregion

        #region EmployeeWorkAddresses

        /// <summary>
        /// The create employee work Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeWorkAddresses">
        /// The employee work Addresses.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeWorkAddress> CreateEmployeeWorkAddresses(string metabase, string accountid, List<EmployeeWorkAddress> employeeWorkAddresses)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeWorkAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEmployeeWorkAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeWorkAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(employeeWorkAddresses);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read employee work Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeWorkAddress"/>.
        /// </returns>
        public EmployeeWorkAddress ReadEmployeeWorkAddress(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EmployeeWorkAddress();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeWorkAddress", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeWorkAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all employee work Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeWorkAddress> ReadAllEmployeeWorkAddresses(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeWorkAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEmployeWorkAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeWorkAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        public List<EmployeeWorkAddress> ReadEmployeeWorkAddressesByAssignment(string metaBase, string accountid, string assignmentnumber)
        {
            this.CheckMetaBase(metaBase);
            var result = new List<EmployeeWorkAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metaBase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEmployeWorkAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metaBase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeWorkAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadSpecial(assignmentnumber);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update employee work Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeWorkAddresses">
        /// The employee work Addresses.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeWorkAddress> UpdateEmployeeWorkAddresses(string metabase, string accountid, List<EmployeeWorkAddress> employeeWorkAddresses)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeWorkAddress>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEmployeeWorkAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeWorkAddress>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(employeeWorkAddresses);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete employee work Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeWorkAddress"/>.
        /// </returns>
        public EmployeeWorkAddress DeleteEmployeeWorkAddresses(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEmployeeWorkAddresses", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeWorkAddress>(this.serviceUrl, this.ApiDbConnection);
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();

                return result;
            }

            return new EmployeeWorkAddress();
        }

        #endregion

        #region CostCodes

        /// <summary>
        /// The create cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="costCodes">
        /// The cost codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CostCode> CreateCostCodes(string metabase, string accountid, List<CostCode> costCodes)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(costCodes);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read cost code.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CostCode"/>.
        /// </returns>
        public CostCode ReadCostCode(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new CostCode();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadCostCode", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CostCode> ReadAllCostCodes(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The update cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="costCodes">
        /// The cost codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CostCode> UpdateCostCodes(string metabase, string accountid, List<CostCode> costCodes)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(costCodes);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CostCode"/>.
        /// </returns>
        public CostCode DeleteCostCodes(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CostCode>(this.serviceUrl, this.ApiDbConnection);
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return new CostCode();
        }

        #endregion

        #region EmployeeCostCodes

        /// <summary>
        /// The create employee cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeCostCodes">
        /// The employee Cost Codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeCostCode> CreateEmployeeCostCodes(string metabase, string accountid, List<EmployeeCostCode> employeeCostCodes)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeCostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEmployeeCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeCostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(employeeCostCodes);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read employee cost code.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        public EmployeeCostCode ReadEmployeeCostCode(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new EmployeeCostCode();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeCostCode", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeCostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read all employee cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeCostCode> ReadAllEmployeeCostCodes(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeCostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEmployeeCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeCostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The read employee cost codes by employee.
        /// </summary>
        /// <param name="metabase">
        /// The metabase.
        /// </param>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeCostCode> ReadEmployeeCostCodesByEmployee(string metabase, string accountid, string employeeId)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeCostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllEmployeeCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeCostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadSpecial(employeeId.ToString());

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update employee cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeCostCodes">
        /// The employee Cost Codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeCostCode> UpdateEmployeeCostCodes(string metabase, string accountid, List<EmployeeCostCode> employeeCostCodes)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeCostCode>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEmployeeCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeCostCode>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(employeeCostCodes);

                this.CloseConnection();

            }

            return result;
        }

        /// <summary>
        /// The delete employee cost codes.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        public EmployeeCostCode DeleteEmployeeCostCodes(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteEmployeeCostCodes", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeCostCode>(this.serviceUrl, this.ApiDbConnection ?? new ApiDbConnection(metabase, this.accountId));
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return new EmployeeCostCode();
        }

        #endregion

        #region CarAssignmentNumberAllocations

        /// <summary>
        /// The create car assignment number allocations.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="carAssignmentNumberAllocations">
        /// The car Assignment Number Allocations.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarAssignmentNumberAllocation> CreateCarAssignmentNumberAllocations(string metabase, string accountid, List<CarAssignmentNumberAllocation> carAssignmentNumberAllocations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarAssignmentNumberAllocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateCarAssignmentNumberAllocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarAssignmentNumberAllocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(carAssignmentNumberAllocations);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read car assignment number allocation.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        public CarAssignmentNumberAllocation ReadCarAssignmentNumberAllocation(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new CarAssignmentNumberAllocation();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadCarAssignmentNumberAllocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarAssignmentNumberAllocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all car assignment number allocations.
        /// </summary>
        /// <param name="metabase">
        /// The metabase.
        /// </param>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarAssignmentNumberAllocation> ReadAllCarAssignmentNumberAllocations(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarAssignmentNumberAllocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllCarAssignmentNumberAllocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarAssignmentNumberAllocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        public List<CarAssignmentNumberAllocation> ReadCarAssignmentNumberAllocationsByEmployee(string metabase, string accountid, string employeeId)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarAssignmentNumberAllocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllCarAssignmentNumberAllocationByEmployee", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarAssignmentNumberAllocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadSpecial(employeeId.ToString());

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update car assignment number allocations.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarAssignmentNumberAllocations">
        /// The car assignment number allocations.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarAssignmentNumberAllocation> UpdateCarAssignmentNumberAllocations(string metabase, string accountid, List<CarAssignmentNumberAllocation> CarAssignmentNumberAllocations)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarAssignmentNumberAllocation>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateCarAssignmentNumberAllocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarAssignmentNumberAllocation>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(CarAssignmentNumberAllocations);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete car assignment number allocations.
        /// </summary>
        /// <param name="metabase">
        /// The metabase.
        /// </param>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        public CarAssignmentNumberAllocation DeleteCarAssignmentNumberAllocations(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "DeleteCarAssignmentNumberAllocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarAssignmentNumberAllocation>(this.serviceUrl, this.ApiDbConnection ?? new ApiDbConnection(metabase, this.accountId));
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();


                return result;
            }

            return new CarAssignmentNumberAllocation();
        }
        #endregion

        #region Import Logs

        /// <summary>
        /// The create import logs.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportLogs">
        /// The import logs.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportLog> CreateImportLogs(string metabase, string accountid, List<ImportLog> ImportLogs)
        {
            this.CheckMetaBase(metabase);
            var result = new List<ImportLog>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateImportLogs", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<ImportLog>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(ImportLogs);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update import logs.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportLogs">
        /// The import logs.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportLog> UpdateImportLogs(string metabase, string accountid, List<ImportLog> ImportLogs)
        {
            this.CheckMetaBase(metabase);
            var result = new List<ImportLog>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateImportLogs", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<ImportLog>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(ImportLogs);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region ImportHistories

        /// <summary>
        /// The create import historys.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportHistorys">
        /// The import historys.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportHistory> CreateImportHistorys(string metabase, string accountid, List<ImportHistory> ImportHistorys)
        {
            this.CheckMetaBase(metabase);
            var result = new List<ImportHistory>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateImportHistorys", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<ImportHistory>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(ImportHistorys);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update import history records.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportHistorys">
        /// The import history items to update.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportHistory> UpdateImportHistorys(string metabase, string accountid, List<ImportHistory> ImportHistorys)
        {
            this.CheckMetaBase(metabase);
            var result = new List<ImportHistory>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateImportHistorys", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<ImportHistory>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(ImportHistorys);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region User Defined Match Field

        /// <summary>
        /// The read user defined match field.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UserDefinedMatchField> ReadUserDefinedMatchField(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<UserDefinedMatchField>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadCarAssignmentNumberAllocation", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<UserDefinedMatchField>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadSpecial(this.entityId.ToString());

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region Employee Role

        /// <summary>
        /// The create employee roles.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeRoles">
        /// The employee roles.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeRole> CreateEmployeeRoles(string metabase, string accountid, List<EmployeeRole> employeeRoles)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeRole>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEmployeeRoles", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeRole>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(employeeRoles);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// Read an EmployeeRole for an Employee.
        /// </summary>
        /// <param name="metabase">The metabase.</param>
        /// <param name="accountid">The account Id.</param>
        /// <param name="employeeid">The employee Id.</param>
        /// <returns>The <see cref="EmployeeRole"/>.</returns>
        public EmployeeRole ReadEmployeeRole(string metabase, string accountid, string employeeid)
        {
            this.CheckMetaBase(metabase);
            EmployeeRole result = null;

            if (this.ConvertParams(accountid, employeeid))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeRole", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeRole>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region EmployeeAccessRole

        /// <summary>
        /// The create employee access roles.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeAccessRoles">
        /// The employee access roles.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeAccessRole> CreateEmployeeAccessRoles(string metabase, string accountid, List<EmployeeAccessRole> employeeAccessRoles)
        {
            this.CheckMetaBase(metabase);
            var result = new List<EmployeeAccessRole>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateEmployeeAccessRoles", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeAccessRole>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(employeeAccessRoles);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// Read an EmployeeAccessRole for an Employee.
        /// </summary>
        /// <param name="metabase">The metabase.</param>
        /// <param name="accountid">The account Id.</param>
        /// <param name="employeeid">The employee Id.</param>
        /// <returns>The <see cref="EmployeeAccessRole"/>.</returns>
        public EmployeeAccessRole ReadEmployeeAccessRole(string metabase, string accountid, string employeeid)
        {
            this.CheckMetaBase(metabase);
            EmployeeAccessRole result = null;

            if (this.ConvertParams(accountid, employeeid))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadEmployeeAccessRole", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<EmployeeAccessRole>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Read(this.entityId);

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region Vehicle Journey Rates

        /// <summary>
        /// The read all vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<VehicleJourneyRate> ReadAllVehicleJourneyRates(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<VehicleJourneyRate>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllVehicleJourneyRates", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<VehicleJourneyRate>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        #endregion

        #region Car Vehicle Journey Rates

        /// <summary>
        /// The create car vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarVehicleJourneyRates">
        /// The car vehicle journey rates.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> CreateCarVehicleJourneyRates(string metabase, string accountid, List<CarVehicleJourneyRate> CarVehicleJourneyRates)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarVehicleJourneyRate>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateCarVehicleJourneyRate", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarVehicleJourneyRate>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Create(CarVehicleJourneyRates);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read car vehicle journey rate.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> ReadCarVehicleJourneyRate(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarVehicleJourneyRate>();
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadCarVehicleJourneyRate", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarVehicleJourneyRate>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadSpecial(this.entityId.ToString());

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The read all car vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> ReadAllCarVehicleJourneyRates(string metabase, string accountid)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarVehicleJourneyRate>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ReadAllCarVehicleJourneyRate", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarVehicleJourneyRate>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.ReadAll();

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The update car vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarVehicleJourneyRates">
        /// The car vehicle journey rates.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> UpdateCarVehicleJourneyRates(string metabase, string accountid, List<CarVehicleJourneyRate> CarVehicleJourneyRates)
        {
            this.CheckMetaBase(metabase);
            var result = new List<CarVehicleJourneyRate>();
            if (this.ConvertParams(accountid, string.Empty))
            {
                this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateCarVehicleJourneyRate", DateTime.Now), "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarVehicleJourneyRate>(this.serviceUrl, this.ApiDbConnection);
                result = dataAccess.Update(CarVehicleJourneyRates);

                this.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// The delete car vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public CarVehicleJourneyRate DeleteCarVehicleJourneyRates(string metabase, string accountid, string id)
        {
            this.CheckMetaBase(metabase);
            if (this.ConvertParams(accountid, id))
            {
                this.logger.WriteDebug(
                    metabase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "DeleteCarVehicleJourneyRate", DateTime.Now),
                    "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarVehicleJourneyRate>(
                    this.serviceUrl, this.ApiDbConnection ?? new ApiDbConnection(metabase, this.accountId));
                var result = dataAccess.Delete(this.entityId);

                this.CloseConnection();
                return result;
            }

            return new CarVehicleJourneyRate();
        }

        /// <summary>
        /// The delete car vehicle journey rates.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="carVehicleJourneyRate">
        /// The car vehicle journey rate.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public CarVehicleJourneyRate DeleteCarVehicleJourneyRates(
            string metaBase, string accountId, CarVehicleJourneyRate carVehicleJourneyRate)
        {
            this.CheckMetaBase(metaBase);
            if (this.ConvertParams(accountId, string.Empty))
            {
                this.logger.WriteDebug(
                    metaBase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "DeleteCarVehicleJourneyRate", DateTime.Now),
                    "ApiCrud");

                if (this.ApiDbConnection == null)
                {
                    this.ApiDbConnection = new ApiDbConnection(metaBase, this.accountId);
                }

                var dataAccess = DataAccessFactory.CreateDataAccess<CarVehicleJourneyRate>(
                    this.serviceUrl, this.ApiDbConnection ?? new ApiDbConnection(metaBase, this.accountId));
                var result = dataAccess.Delete(carVehicleJourneyRate);

                this.CloseConnection();
                return result;
            }

            return new CarVehicleJourneyRate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Convert string parameters to integers.
        /// </summary>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ConvertParams(string accountid, string id = "")
        {
            bool result = true;
            this.accountId = -1;
            this.accountId = -1;
            int.TryParse(accountid, out this.accountId);
            long.TryParse(id, out this.entityId);
            if (accountid != string.Empty && this.accountId == -1)
            {
                result = false;
            }

            this.logger.WriteExtra(string.Empty, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ConvertParams", DateTime.Now), "ApiCrud");

            if (id != string.Empty && this.entityId == -1)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// The check meta base.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <exception cref="WebException">
        /// METABASE must be valid
        /// </exception>
        private void CheckMetaBase(string metaBase)
        {
            this.logger.WriteExtra(metaBase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CheckMetaBase", DateTime.Now), "ApiCrud");
            try
            {
                if (!MemoryCache.Default.Contains(string.Format("Metabase_{0}", metaBase)))
                {
                    var validMetaBase = ConfigurationManager.ConnectionStrings[metaBase].ConnectionString;
                    MemoryCache.Default.Add(
                        string.Format("Metabase_{0}", metaBase),
                        validMetaBase,
                        new CacheItemPolicy
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(15)
                        });
                }
            }
            catch (Exception ex)
            {
                this.logger.Write(metaBase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Exception {1}", "CheckMetaBase", ex.Message), "ApiCrud");
                throw new ApiException
                {
                    ApiMessage = string.Format("Metabase {0} not valid", metaBase),
                    Result = ApiActionResult.Failure,
                    Source = "ApiCrud"
                };
            }
        }

        /// <summary>
        /// The close connection.
        /// </summary>
        private void CloseConnection()
        {
            this.ApiDbConnection.Dispose();
            this.ApiDbConnection = null;
        }

        /// <summary>
        /// The clear employee cache.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private void ClearCache(IEnumerable<Employee> result)
        {
            foreach (Employee dataClassBase in result)
            {
                cache.Delete(this.accountId, "employee", dataClassBase.employeeid.ToString());
            }
        }


        #endregion
    }
}
