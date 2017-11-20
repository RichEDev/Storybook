namespace EsrGo2FromNhs
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

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Crud;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    using ApiException = global::EsrGo2FromNhs.Base.ApiExceptionBase;
    using ApiResult = global::EsrGo2FromNhs.Base.ApiResult;
    using Cache = Utilities.DistributedCaching.Cache;
    using Log = global::EsrGo2FromNhs.Base.Log;
    using LogRecord = global::EsrGo2FromNhs.Base.LogRecord;

    /// <summary>
    /// The API service .
    /// </summary>
    public class ApiService
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
        public ApiService(IApiDbConnection connection)
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

        public List<T> Execute<T>(string metabase, string theAccountId, DataAccessMethod accessMethod, string identifier = "", List<T> entities = null, DataAccessMethodReturnDefault returnDefault = DataAccessMethodReturnDefault.NewObject) where T : DataClassBase, new()
        {
            Type type = typeof(T);

            bool clearingCache = false;
            this.CheckMetaBase(metabase);
            var result = new List<T>();
            string accessType = System.Enum.GetName(typeof(DataAccessMethod), accessMethod);

            if (!this.ConvertParams(theAccountId, identifier))
            {
                switch (returnDefault)
                {
                    case DataAccessMethodReturnDefault.NewObject:
                        break;
                    case DataAccessMethodReturnDefault.NewObjectWithFailureFlag:
                        if (type == typeof(EsrAssignment))
                        {
                            var assignment = new T { ActionResult = new ApiResult { Result = ApiActionResult.Failure } };
                            return new List<T> { assignment };
                        }

                        break;
                    case DataAccessMethodReturnDefault.Null:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException("returnDefault");
                }

                return result;
            }

            this.LoggerWrite(metabase, accessType);

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
            }

            var dataAccess = DataAccessFactory.CreateDataAccess<T>(this.ApiDbConnection);

            switch (accessMethod)
            {
                case DataAccessMethod.Create:
                    if (type == typeof(Address))
                    {
                        var currentAddresses = new List<T>();
                        var addresses = entities as List<Address>;
                        if (addresses != null)
                        {
                            foreach (var address in addresses)
                            {
                                T singleAddress = null;
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
                                    var checkAddress = singleAddress as Address;
                                    if (checkAddress != null)
                                    {
                                        address.AddressID = checkAddress.AddressID;
                                        currentAddresses.Add(address as T);
                                    }
                                }
                            }
                        }

                        result = dataAccess.Create(currentAddresses);
                    }
                    else
                    {
                        result = dataAccess.Create(entities);
                    }

                    if (type == typeof(Employee))
                    {
                        clearingCache = true;
                    }
                    break;
                case DataAccessMethod.Update:
                    result = dataAccess.Update(entities);

                    if (type == typeof(Employee))
                    {
                        clearingCache = true;
                    }

                    break;
                case DataAccessMethod.ReadAll:
                    if (type == typeof(Field))
                    {
                        var interimResult = dataAccess.ReadAll() as List<Field>;
                        if (interimResult != null)
                        {
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
                                    result.Add(field as T);
                                }
                            }
                        }
                    }
                    else
                    {
                        result = dataAccess.ReadAll();
                    }

                    break;
                case DataAccessMethod.ReadByEsrId:
                    long id;
                    long.TryParse(identifier, out id);
                    result = dataAccess.ReadByEsrId(id);
                    break;
                case DataAccessMethod.ReadSpecial:
                    result = dataAccess.ReadSpecial(identifier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("accessMethod");
            }

            this.CloseConnection();

            if (clearingCache)
            {
                this.ClearCache(result as List<Employee>);
            }

            return result;
        }

        public T Execute<T>(string metabase, string theAccountId, string identifier, DataAccessMethod accessMethod, DataAccessMethodReturnDefault returnDefault = DataAccessMethodReturnDefault.NewObject) where T : DataClassBase, new()
        {
            Type type = typeof(T);

            bool clearingCache = false;
            this.CheckMetaBase(metabase);
            var result = new T();
            string accessType = System.Enum.GetName(typeof(DataAccessMethod), accessMethod);

            if (!this.ConvertParams(theAccountId, identifier))
            {
                switch (returnDefault)
                {
                    case DataAccessMethodReturnDefault.NewObject:
                        break;
                    case DataAccessMethodReturnDefault.NewObjectWithFailureFlag:
                        if (type == typeof(EsrAssignment))
                        {
                            return new T { ActionResult = new ApiResult { Result = ApiActionResult.Failure } };
                        }

                        break;
                    case DataAccessMethodReturnDefault.Null:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException("returnDefault");
                }
                return result;
            }

            this.LoggerWrite(metabase, accessType);

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
            }

            var dataAccess = DataAccessFactory.CreateDataAccess<T>(this.ApiDbConnection);

            switch (accessMethod)
            {
                case DataAccessMethod.Delete:
                    result = dataAccess.Delete(this.entityId);

                    if (type == typeof(Employee))
                    {
                        clearingCache = true;
                    }

                    break;
                case DataAccessMethod.Read:
                    result = dataAccess.Read(this.entityId);
                    break;
                case DataAccessMethod.ReadSpecial:
                    result = dataAccess.ReadSpecial(identifier).FirstOrDefault();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("accessMethod");
            }

            this.CloseConnection();

            if (clearingCache)
            {
                this.ClearCache(new List<Employee> { result as Employee });
            }

            return result;
        }

        public T Execute<T>(string metabase, string theAccountId, DataAccessMethod accessMethod, T entity) where T : DataClassBase, new()
        {
            this.CheckMetaBase(metabase);
            var result = new T();

            string accessType = System.Enum.GetName(typeof(DataAccessMethod), accessMethod);

            if (!this.ConvertParams(theAccountId, string.Empty))
            {
                return result;
            }

            this.LoggerWrite(metabase, accessType);

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(metabase, this.accountId);
            }

            var dataAccess = DataAccessFactory.CreateDataAccess<T>(this.ApiDbConnection);

            switch (accessMethod)
            {
                case DataAccessMethod.Delete:
                    result = dataAccess.Delete(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("accessMethod");
            }

            this.CloseConnection();

            return result;
        }

        private void LoggerWrite(string metabase, string accessType)
        {

            this.logger.WriteDebug(metabase, 0, this.accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", accessType, DateTime.Now), "ApiCrud");
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, null, employees);
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
            return this.Execute<Employee>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// <param name="username">
        /// The user name.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee ReadEmployeeByUsername(string metabase, string accountid, string username)
        {
            return this.Execute<Employee>(metabase, accountid, username, DataAccessMethod.ReadSpecial);
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
            return this.Execute<Employee>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", employees);
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
            return this.Execute<Employee>(metabase, accountid, id, DataAccessMethod.Delete);
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
            return this.Execute<Employee>(metabase, accountid, DataAccessMethod.ReadByEsrId, id);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrPersons);
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
            return this.Execute<EsrPerson>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrPerson>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrPersons);
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
            return this.Execute<EsrPerson>(metabase, accountid, id, DataAccessMethod.Delete);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrAssignments);
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
            return this.Execute<EsrAssignment>(metabase, accountid, id, DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
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
            return this.Execute<EsrAssignment>(metabase, accountid, DataAccessMethod.ReadByEsrId, id, null, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
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
            return this.Execute<EsrAssignment>(metabase, accountid, DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read ESR assignment by assignment.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        public List<EsrAssignment> ReadEsrAssignmentByAssignment(string metabase, string accountid, string reference)
        {
            return this.Execute<EsrAssignment>(metabase, accountid, DataAccessMethod.ReadSpecial, reference);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrAssignments);
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
            return this.Execute<EsrAssignment>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrVehicles);
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
            return this.Execute<EsrVehicle>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrVehicle>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute<EsrVehicle>(metabase, accountid, DataAccessMethod.ReadByEsrId, id);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrVehicles);
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
            return this.Execute<EsrVehicle>(metabase, accountid, id, DataAccessMethod.Delete);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrPhones);
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
            return this.Execute<EsrPhone>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrPhone>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute<EsrPhone>(metabase, accountid, DataAccessMethod.ReadByEsrId, id);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrPhones);
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
            return this.Execute<EsrPhone>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrAddress);
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
            return this.Execute<EsrAddress>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrAddress>(metabase, accountid, DataAccessMethod.ReadByEsrId, id);
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
            return this.Execute<EsrAddress>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrAddress);
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
            return this.Execute<EsrAddress>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrAssignmentCostings);
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
            return this.Execute<EsrAssignmentCostings>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrAssignmentCostings>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrAssignmentCostings);
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
            return this.Execute<EsrAssignmentCostings>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        #endregion

        #region EsrAssignmentLocations

        /// <summary>
        /// Creates or updates ESR Assignment Locations
        /// </summary>
        /// <param name="metabase">The metabase</param>
        /// <param name="accountid">The account id</param>
        /// <param name="esrAssignmentLocations">A collection of ESR Assignments to create/update</param>
        /// <returns></returns>
        public List<EsrAssignmentLocation> CreateAssignmentLocations(string metabase, string accountid, List<EsrAssignmentLocation> esrAssignmentLocations)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, String.Empty, esrAssignmentLocations);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrElementFields);
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
            return this.Execute<EsrElementFields>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrElementFields>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrElementFields);
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
            return this.Execute<EsrElementFields>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrElements);
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
            return this.Execute<EsrElement>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrElement>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrElements);
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
            return this.Execute<EsrElement>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrTrusts);
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
            return this.Execute<EsrTrust>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrTrust>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrTrusts);
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
            return this.Execute<EsrTrust>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrLocations);
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
            return this.Execute<EsrLocation>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrLocation>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrLocations);
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
            return this.Execute<EsrLocation>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrOrganisations);
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
            return this.Execute<EsrOrganisation>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrOrganisation>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrOrganisations);
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
            return this.Execute<EsrOrganisation>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrPositions);
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
            return this.Execute<EsrPosition>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrPosition>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrPositions);
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
            return this.Execute<EsrPosition>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", esrElementSubCats);
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
            return this.Execute<EsrElementSubCat>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<EsrElementSubCat>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", esrElementSubCats);
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
            return this.Execute<EsrElementSubCat>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", cars);
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
            return this.Execute<Car>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<Car>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute<Car>(metabase, accountid, DataAccessMethod.ReadByEsrId, id);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", cars);
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
            return this.Execute<Car>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        #endregion

        #region Misc

        /// <summary>
        /// The read all fields for imports.
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
        public List<Field> ReadAllFields(string metabase, string accountid)
        {
            return this.Execute<Field>(metabase, accountid, DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read all User Defined Fields.
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
        public List<UserDefinedField> ReadAllUdf(string metabase, string accountid)
        {
            return this.Execute<UserDefinedField>(metabase, accountid, DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The create UDF value.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<UserDefinedField> CreateUdfValue(string metabase, string accountid, List<UserDefinedField> entities)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", entities);
        }

        /// <summary>
        /// The read all account properties.
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
        public List<AccountProperty> ReadAllAccountProperties(string metabase, string accountid)
        {
            return this.Execute<AccountProperty>(metabase, accountid, DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read lookup value.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
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
        public Lookup ReadLookupValue(string metabase, string accountid, string tableid, string fieldid, string keyvalue)
        {
            this.CheckMetaBase(metabase);
            var result = new Lookup();
            if (this.ConvertParams(accountid, string.Empty))
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
                    string.Format("{0} - {1}", "ReadLookupValue", DateTime.Now),
                    "ApiCrud");
                using (var dataAccess = this.ApiDbConnection ?? new ApiDbConnection(metabase, this.accountId))
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
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", templateMappings);
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
            return this.Execute<TemplateMapping>(metabase, accountid, id, DataAccessMethod.Read);
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
            return this.Execute<TemplateMapping>(metabase, accountid, DataAccessMethod.ReadAll);
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
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", templateMappings);
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
            return this.Execute<TemplateMapping>(metabase, accountid, id, DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
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
        /// The <see cref="Address"/>.
        /// </returns>
        public List<Address> CreateAddresses(string metabase, string accountid, List<Address> addresses)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", addresses);
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
            return this.Execute<Address>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// The <see cref="Address"/>.
        /// </returns>
        public List<Address> ReadAllAddresses(string metabase, string accountid)
        {
            return this.Execute<Address>(metabase, accountid, DataAccessMethod.ReadAll);
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
        /// The <see cref="Address"/>.
        /// </returns>
        public List<Address> UpdateAddresses(string metabase, string accountid, List<Address> addresses)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", addresses);
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
            return this.Execute<Address>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// The <see cref="Address"/>.
        /// </returns>
        public List<Address> ReadAddressByEsr(string metabase, string accountid, string id)
        {
            return this.Execute<Address>(metabase, accountid, DataAccessMethod.ReadByEsrId, id);
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
        /// The <see cref="Address"/>.
        /// </returns>
        public List<EmployeeHomeAddress> CreateEmployeeHomeAddresses(string metabase, string accountid, List<EmployeeHomeAddress> employeeHomeAddresses)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", employeeHomeAddresses);
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
            return this.Execute<EmployeeHomeAddress>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// The <see cref="EmployeeHomeAddress" />.
        /// </returns>
        public List<EmployeeHomeAddress> ReadAllEmployeeHomeAddresses(string metabase, string accountid)
        {
            return this.Execute<EmployeeHomeAddress>(metabase, accountid, DataAccessMethod.ReadAll);
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
        /// The <see cref="EmployeeHomeAddress"/>.
        /// </returns>
        public List<EmployeeHomeAddress> UpdateEmployeeHomeAddresses(string metabase, string accountid, List<EmployeeHomeAddress> employeeHomeAddresses)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", employeeHomeAddresses);
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
            return this.Execute<EmployeeHomeAddress>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// The <see cref="EmployeeWorkAddress"/>.
        /// </returns>
        public List<EmployeeWorkAddress> CreateEmployeeWorkAddresses(string metabase, string accountid, List<EmployeeWorkAddress> employeeWorkAddresses)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", employeeWorkAddresses);
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
            return this.Execute<EmployeeWorkAddress>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// The <see cref="EmployeeWorkAddress"/>.
        /// </returns>
        public List<EmployeeWorkAddress> ReadAllEmployeeWorkAddresses(string metabase, string accountid)
        {
            return this.Execute<EmployeeWorkAddress>(metabase, accountid, DataAccessMethod.ReadAll);
        }

        public List<EmployeeWorkAddress> ReadEmployeeWorkAddressesByAssignment(string metabase, string accountid, string assignmentnumber)
        {
            return this.Execute<EmployeeWorkAddress>(metabase, accountid, DataAccessMethod.ReadSpecial, assignmentnumber);
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
        /// The <see cref="EmployeeWorkAddress"/>.
        /// </returns>
        public List<EmployeeWorkAddress> UpdateEmployeeWorkAddresses(string metabase, string accountid, List<EmployeeWorkAddress> employeeWorkAddresses)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", employeeWorkAddresses);
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
            return this.Execute<EmployeeWorkAddress>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// The <see cref="CostCode"/>.
        /// </returns>
        public List<CostCode> CreateCostCodes(string metabase, string accountid, List<CostCode> costCodes)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", costCodes);
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
            return this.Execute<CostCode>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// The <see cref="CostCode"/>.
        /// </returns>
        public List<CostCode> ReadAllCostCodes(string metabase, string accountid)
        {
            return this.Execute<CostCode>(metabase, accountid, DataAccessMethod.ReadAll);
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
        /// The <see cref="CostCode"/>.
        /// </returns>
        public List<CostCode> UpdateCostCodes(string metabase, string accountid, List<CostCode> costCodes)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", costCodes);
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
            return this.Execute<CostCode>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        public List<EmployeeCostCode> CreateEmployeeCostCodes(string metabase, string accountid, List<EmployeeCostCode> employeeCostCodes)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", employeeCostCodes);
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
            return this.Execute<EmployeeCostCode>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        public List<EmployeeCostCode> ReadAllEmployeeCostCodes(string metabase, string accountid)
        {
            return this.Execute<EmployeeCostCode>(metabase, accountid, DataAccessMethod.ReadAll);
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
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        public List<EmployeeCostCode> ReadEmployeeCostCodesByEmployee(string metabase, string accountid, string employeeId)
        {
            return this.Execute<EmployeeCostCode>(metabase, accountid, DataAccessMethod.ReadSpecial, employeeId);
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
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        public List<EmployeeCostCode> UpdateEmployeeCostCodes(string metabase, string accountid, List<EmployeeCostCode> employeeCostCodes)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", employeeCostCodes);
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
            return this.Execute<EmployeeCostCode>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        public List<CarAssignmentNumberAllocation> CreateCarAssignmentNumberAllocations(string metabase, string accountid, List<CarAssignmentNumberAllocation> carAssignmentNumberAllocations)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", carAssignmentNumberAllocations);
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
            return this.Execute<CarAssignmentNumberAllocation>(metabase, accountid, id, DataAccessMethod.Read);
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
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        public List<CarAssignmentNumberAllocation> ReadAllCarAssignmentNumberAllocations(string metabase, string accountid)
        {
            return this.Execute<CarAssignmentNumberAllocation>(metabase, accountid, DataAccessMethod.ReadAll);
        }

        public List<CarAssignmentNumberAllocation> ReadCarAssignmentNumberAllocationsByEmployee(string metabase, string accountid, string employeeId)
        {
            return this.Execute<CarAssignmentNumberAllocation>(metabase, accountid, DataAccessMethod.ReadSpecial, employeeId);
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
        /// <param name="carAssignmentNumberAllocations">
        /// The car assignment number allocations.
        /// </param>
        /// <returns>
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        public List<CarAssignmentNumberAllocation> UpdateCarAssignmentNumberAllocations(string metabase, string accountid, List<CarAssignmentNumberAllocation> carAssignmentNumberAllocations)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", carAssignmentNumberAllocations);
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
            return this.Execute<CarAssignmentNumberAllocation>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// <param name="importLogs">
        /// The import logs.
        /// </param>
        /// <returns>
        /// The <see cref="ImportLog"/>.
        /// </returns>
        public List<ImportLog> CreateImportLogs(string metabase, string accountid, List<ImportLog> importLogs)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", importLogs);
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
        /// <param name="importLogs">
        /// The import logs.
        /// </param>
        /// <returns>
        /// The <see cref="ImportLog"/>.
        /// </returns>
        public List<ImportLog> UpdateImportLogs(string metabase, string accountid, List<ImportLog> importLogs)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", importLogs);
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
        /// <param name="importHistorys">
        /// The import historys.
        /// </param>
        /// <returns>
        /// The <see cref="ImportHistory"/>.
        /// </returns>
        public List<ImportHistory> CreateImportHistorys(string metabase, string accountid, List<ImportHistory> importHistorys)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", importHistorys);
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
        /// <param name="importHistorys">
        /// The import history items to update.
        /// </param>
        /// <returns>
        /// The <see cref="ImportHistory"/>.
        /// </returns>
        public List<ImportHistory> UpdateImportHistorys(string metabase, string accountid, List<ImportHistory> importHistorys)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", importHistorys);
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
        /// The <see cref="UserDefinedMatchField"/>.
        /// </returns>
        public List<UserDefinedMatchField> ReadUserDefinedMatchField(string metabase, string accountid, string id)
        {
            return this.Execute<UserDefinedMatchField>(metabase, accountid, DataAccessMethod.ReadSpecial, id);
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
        /// The <see cref="EmployeeRole"/>.
        /// </returns>
        public List<EmployeeRole> CreateEmployeeRoles(string metabase, string accountid, List<EmployeeRole> employeeRoles)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", employeeRoles);
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
            return this.Execute<EmployeeRole>(metabase, accountid, employeeid, DataAccessMethod.Read);
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
        /// The <see cref="EmployeeAccessRole"/>.
        /// </returns>
        public List<EmployeeAccessRole> CreateEmployeeAccessRoles(string metabase, string accountid, List<EmployeeAccessRole> employeeAccessRoles)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", employeeAccessRoles);
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
            return this.Execute<EmployeeAccessRole>(metabase, accountid, employeeid, DataAccessMethod.Read);
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
        /// The <see cref="VehicleJourneyRate"/>.
        /// </returns>
        public List<VehicleJourneyRate> ReadAllVehicleJourneyRates(string metabase, string accountid)
        {
            return this.Execute<VehicleJourneyRate>(metabase, accountid, DataAccessMethod.ReadAll);
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
        /// <param name="carVehicleJourneyRates">
        /// The car vehicle journey rates.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> CreateCarVehicleJourneyRates(string metabase, string accountid, List<CarVehicleJourneyRate> carVehicleJourneyRates)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Create, "", carVehicleJourneyRates);
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
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> ReadCarVehicleJourneyRate(string metabase, string accountid, string id)
        {
            return this.Execute<CarVehicleJourneyRate>(metabase, accountid, DataAccessMethod.ReadSpecial, id);
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
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> ReadAllCarVehicleJourneyRates(string metabase, string accountid)
        {
            return this.Execute<CarVehicleJourneyRate>(metabase, accountid, DataAccessMethod.ReadAll);
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
        /// <param name="carVehicleJourneyRates">
        /// The car vehicle journey rates.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> UpdateCarVehicleJourneyRates(string metabase, string accountid, List<CarVehicleJourneyRate> carVehicleJourneyRates)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Update, "", carVehicleJourneyRates);
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
            return this.Execute<CarVehicleJourneyRate>(metabase, accountid, id, DataAccessMethod.Delete);
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
        /// <param name="carVehicleJourneyRate">
        /// The car vehicle journey rate.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public CarVehicleJourneyRate DeleteCarVehicleJourneyRates(string metabase, string accountid, CarVehicleJourneyRate carVehicleJourneyRate)
        {
            return this.Execute(metabase, accountid, DataAccessMethod.Delete, carVehicleJourneyRate);
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
            this.entityId = -1;
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
                cache.Delete(this.accountId, "employee", dataClassBase.employeeid.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion
    }
}
