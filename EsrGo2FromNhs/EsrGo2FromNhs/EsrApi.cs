using System;

namespace EsrGo2FromNhs
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Crud;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    using Utilities.DistributedCaching;

    public class EsrApi : IEsrApi
    {
        /// <summary>
        /// The cache.
        /// </summary>
        private static readonly Cache _cache;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log _logger;

        /// <summary>
        /// The account id.
        /// </summary>
        private int _accountId;

        /// <summary>
        /// The metabase.
        /// </summary>
        private readonly string _metabase;

        /// <summary>
        /// The entity id.
        /// </summary>
        private long _entityId;

        /// <summary>
        /// Initialises static members of the <see cref="ApiService"/> class.
        /// </summary>
        static EsrApi()
        {
            _cache = new Cache();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiService"/> class.
        /// </summary>
        public EsrApi(string metabase, int accountId)
        {
            this._metabase = metabase;
            this._accountId = accountId;
            this._logger = new Log();
        }

        /// <summary>
        /// Gets or sets the API DB connection.
        /// </summary>
        public IApiDbConnection ApiDbConnection
        {
            get;
            set;
        }

        #region Generic Api Calls

        /// <summary>
        /// Generic Api call returning a List of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessMethod"></param>
        /// <param name="identifier"></param>
        /// <param name="entities"></param>
        /// <param name="returnDefault"></param>
        /// <returns></returns>
        public List<T> Execute<T>(DataAccessMethod accessMethod, string identifier = "", List<T> entities = null, DataAccessMethodReturnDefault returnDefault = DataAccessMethodReturnDefault.NewObject) where T : DataClassBase, new()
        {
            Type type = typeof(T);

            bool clearingCache = false;
            this.CheckMetaBase(this._metabase);
            var result = new List<T>();
            string accessType = System.Enum.GetName(typeof(DataAccessMethod), accessMethod);

            if (!this.ConvertParams(this._accountId.ToString(CultureInfo.InvariantCulture), identifier))
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

            this.LoggerWrite(accessType);

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(this._metabase, this._accountId);
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
                                    }
                                }

                                currentAddresses.Add(address as T);
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
                    result = dataAccess.ReadByEsrId(this._entityId);
                    break;
                case DataAccessMethod.ReadSpecial:
                    result = dataAccess.ReadSpecial(identifier);
                    break;
                case DataAccessMethod.Read:
                    result = new List<T> { dataAccess.Read(this._entityId) };
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

        /// <summary>
        /// Generic Api call returning a specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="accessMethod"></param>
        /// <param name="returnDefault"></param>
        /// <returns></returns>
        public T Execute<T>(string identifier, DataAccessMethod accessMethod, DataAccessMethodReturnDefault returnDefault = DataAccessMethodReturnDefault.NewObject) where T : DataClassBase, new()
        {
            Type type = typeof(T);

            bool clearingCache = false;
            this.CheckMetaBase(this._metabase);
            var result = new T();
            string accessType = System.Enum.GetName(typeof(DataAccessMethod), accessMethod);

            if (!this.ConvertParams(this._accountId.ToString(CultureInfo.InvariantCulture), identifier))
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

            this.LoggerWrite(accessType);

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(this._metabase, this._accountId);
            }

            var dataAccess = DataAccessFactory.CreateDataAccess<T>(this.ApiDbConnection);

            switch (accessMethod)
            {
                case DataAccessMethod.Delete:
                    result = dataAccess.Delete(this._entityId);

                    if (type == typeof(Employee))
                    {
                        clearingCache = true;
                    }

                    break;
                case DataAccessMethod.Read:
                    result = dataAccess.Read(this._entityId);
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

        /// <summary>
        /// Generic Api call returning a specified type, handling deleting of a generic type entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessMethod"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Execute<T>(DataAccessMethod accessMethod, T entity) where T : DataClassBase, new()
        {
            this.CheckMetaBase(this._metabase);
            var result = new T();

            string accessType = System.Enum.GetName(typeof(DataAccessMethod), accessMethod);

            if (!this.ConvertParams(this._accountId.ToString(CultureInfo.InvariantCulture), string.Empty))
            {
                return result;
            }

            this.LoggerWrite(accessType);

            if (this.ApiDbConnection == null)
            {
                this.ApiDbConnection = new ApiDbConnection(this._metabase, this._accountId);
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

        # endregion

        /// <summary>
        /// Write to the logger
        /// </summary>
        /// <param name="accessType"></param>
        public void LoggerWrite(string accessType)
        {
            this._logger.WriteDebug(this._metabase, 0, this._accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", accessType, DateTime.Now), "ApiCrud");
        }

        #region special instance api calls

        /// <summary>
        /// Get templates where VPD = reference
        /// </summary>
        /// <param name="reference">The VPD to read.</param>
        /// <returns></returns>
        public List<TemplateMapping> TemplateMappingReadSpecial(string reference)
        {
            var cache = MemoryCache.Default;
            if (!cache.Contains(string.Format("Trust_{0}", reference)))
            {
                var allRecords = this.Execute<TemplateMapping>(DataAccessMethod.ReadAll);
                if (allRecords != null)
                {
                    var result = !string.IsNullOrEmpty(reference) ? allRecords.Where(templateMapping => templateMapping.trustVPD == reference).ToList() : allRecords;
                    cache.Add(
                        string.Format("Trust_{0}", reference),
                        result,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
                    return result;
                }

            }
            else
            {
                return cache.Get(string.Format("Trust_{0}", reference)) as List<TemplateMapping>;
            }

            return new List<TemplateMapping>();
        }

        /// <summary>
        /// The find ESR trust in a given meta base by id.
        /// </summary>
        /// <param name="esrTrust">
        /// The ESR Trust.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// Account that contains the NHS VPD
        /// </returns>
        public EsrTrust FindEsrTrust(string esrTrust)
        {
            this.CheckMetaBase(this._metabase);
            int vpd;
            int.TryParse(esrTrust, out vpd);
            this._logger.WriteDebug(this._metabase, vpd, this._accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "FindEsrTrust", DateTime.Now), "ApiCrud");
            using (var apiData = new ApiDbConnection(this._metabase))
            {
                var accountList = apiData.GetAccounts();

                if (accountList == null)
                {
                    return new EsrTrust();
                }

                foreach (Account account in accountList)
                {
                    apiData.AccountId = account.AccountId;
                    this._accountId = account.AccountId;
                    List<EsrTrust> result = this.Execute<EsrTrust>(DataAccessMethod.ReadAll);
                    if (result.Any(trust => trust.trustVPD == esrTrust))
                    {
                        EsrTrust foundTrust = result.Find(trust => trust.trustVPD == esrTrust);
                        foundTrust.AccountId = account.AccountId;
                        return foundTrust;
                    }
                }
            }

            return new EsrTrust();
        }

        /// <summary>
        /// Updates the SupervisorEsrAssignID for assignments where the SuperAssignmentID is not null at the end of the batch
        /// </summary>
        public void UpdateEsrAssignmentSupervisors()
        {
            using (var dataAccess = new ApiDbConnection(this._metabase, this._accountId))
            {
                dataAccess.Sqlexecute.Parameters.Clear();
                dataAccess.ExecuteProc("dbo.ApiBatchUpdateEsrAssignmentSupervisors");
            }
        }

        /// <summary>
        /// The read lookup value.
        /// </summary>
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
        public Lookup ReadLookupValue(string tableid, string fieldid, string keyvalue)
        {
            this.CheckMetaBase(this._metabase);
            var result = new Lookup();
            if (this.ConvertParams(this._accountId.ToString(CultureInfo.InvariantCulture), string.Empty))
            {
                this._logger.WriteDebug(
                    this._metabase,
                    0,
                    this._accountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} - {1}", "ReadLookupValue", DateTime.Now),
                    "ApiCrud");
                using (var dataAccess = new ApiDbConnection(this._metabase, this._accountId))
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

        # endregion

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
        public bool ConvertParams(string accountid, string id = "")
        {
            bool result = true;
            this._accountId = -1;
            this._entityId = -1;
            int.TryParse(accountid, out this._accountId);
            long.TryParse(id, out this._entityId);
            if (accountid != string.Empty && this._accountId == -1)
            {
                result = false;
            }

            this._logger.WriteExtra(string.Empty, 0, this._accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "ConvertParams", DateTime.Now), "ApiCrud");

            if (id != string.Empty && this._entityId == -1)
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
        public void CheckMetaBase(string metaBase)
        {
            this._logger.WriteExtra(metaBase, 0, this._accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CheckMetaBase", DateTime.Now), "ApiCrud");
            try
            {
                if (MemoryCache.Default.Contains(string.Format("Metabase_{0}", metaBase)))
                {
                    return;
                }

                var validMetaBase = ConfigurationManager.ConnectionStrings[metaBase].ConnectionString;
                MemoryCache.Default.Add(
                    string.Format("Metabase_{0}", metaBase),
                    validMetaBase,
                    new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(15)
                    });
            }
            catch (Exception ex)
            {
                this._logger.Write(metaBase, 0, this._accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Exception {1}", "CheckMetaBase", ex.Message), "ApiCrud");
                throw new ApiExceptionBase
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
        public void CloseConnection()
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
        public void ClearCache(IEnumerable<Employee> result)
        {
            foreach (Employee dataClassBase in result)
            {
                _cache.Delete(this._accountId, "employee", dataClassBase.employeeid.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion
    }
}
