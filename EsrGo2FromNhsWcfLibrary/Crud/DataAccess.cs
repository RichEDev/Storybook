namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The data access.
    /// </summary>
    /// <typeparam name="T">
    /// The DataAccess class for this instance
    /// </typeparam>
    public class DataAccess<T> : IDataAccess<T>
        where T : DataClassBase, new()
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log _logger;

        /// <summary>
        /// The current type fields.
        /// </summary>
        private FieldInfo[] currentTypeFields;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataAccess{T}"/> class. 
        /// Initialises a new instance of the <see /> class.
        /// </summary>
        /// <param name="dataConnection">
        /// The data Connection.
        /// </param>
        /// <param name="loggerDb">
        /// The logger Database connection.
        /// </param>
        public DataAccess(IApiDbConnection dataConnection, IApiDbConnection loggerDb = null)
        {
            this.Connection = dataConnection;
            this._logger = loggerDb == null ? new Log() : new Log(loggerDb);
        }

        /// <summary>
        /// Gets or sets the API data connection.
        /// </summary>
        protected IApiDbConnection Connection { get; set; }

        #region I Data Access

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> Create(List<T> entities)
        {
            var results = new List<T>();

            if (entities == null || entities.Count == 0)
            {
                return results;
            }

            results.AddRange(from singleEntity in entities
                            where singleEntity.Action == Action.Create || singleEntity.Action == Action.Update
                            select this.SetDateField(singleEntity, "createdon")
                                into entity
                                select this.SetDateField(entity, "modifiedon"));

                return this.StoredProcedureSaveBatch(results);
            }

        // TODO This needs to worked on for batch reads to increase system performance
        private IEnumerable<T> ReadBatch(List<long> entityIdList)
        {
            if (entityIdList.Count == 0)
            {
                return null;
            }

            var result = this.StoredProcedureReadBatch(entityIdList);
            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Read(long entityId)
        {
            if (entityId == 0)
            {
                return null;
            }

            var result = this.StoredProcedureRead(bigEntityId: entityId);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Read(int entityId)
        {
            if (entityId == 0)
            {
                return null;
            }

            var result = this.StoredProcedureRead(entityId);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> ReadAll()
        {
            var result = this.StoredProcedureRead();
            return result;
        }

        /// <summary>
        /// The read by employee ID.
        /// </summary>
        /// <param name="esrId">
        /// The employee Id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<T> ReadByEsrId(long esrId)
        {
            if (esrId == 0)
            {
                return new List<T>();
            }

            var result = this.StoredProcedureReadById(esrId);
            return result;
        }

        /// <summary>
        /// The read by assignment Id
        /// </summary>
        /// <param name="assignmentId">The assignment Id</param>
        /// <returns>The <see cref="List{T}"/></returns>
        public List<T> ReadByAssignmentId(long assignmentId)
        {
            if (assignmentId == 0)
            {
                return new List<T>();
            }

            var result = this.StoredProcedureReadByAssignmentId(assignmentId);
            return result;
        }

        /// <summary>
        /// Special read.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> ReadSpecial(string reference)
        {
            var result = this.StoredProcedureReadSpecial(reference);
            return result;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> Update(List<T> entities)
        {
            var results = (from singleEntity in entities
                              where singleEntity.Action == Action.Create || singleEntity.Action == Action.Update
                              select this.SetDateField(singleEntity, "createdon")
                                  into newEntity
                                  select this.SetDateField(newEntity, "modifiedon")).ToList();

                return this.StoredProcedureSaveBatch(results);
            }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Delete(long entityId)
        {
            var entity = this.Read(entityId);
            return this.StoredProcedureDelete(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Delete(int entityId)
        {
            var entity = this.Read(entityId);
            return this.StoredProcedureDelete(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Delete(T entity)
        {
            return this.StoredProcedureDelete(entity);
        }

        #endregion

        /// <summary>
        /// The stored procedure save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T StoredProcedureSave(T entity)
        {
            var currentType = new T();
            var saveStoredProcedure = currentType.SaveStoredProcedure;
            var propInfo = this.GetFields();

            this.Connection.Sqlexecute.Parameters.Clear();
            this.Connection.ErrorMessage = string.Empty;

            if (entity.ActionResult == null)
            {
                entity.ActionResult = new ApiResult();
            }

            foreach (FieldInfo propertyInfo in propInfo)
            {
                var itemValue = propertyInfo.GetValue(entity);

                if (propertyInfo == currentType.KeyField)
                {
                    this.Connection.Sqlexecute.Parameters.Add(string.Format("@{0}", propertyInfo.Name), SqlDbType.BigInt);
                    this.Connection.Sqlexecute.Parameters[string.Format("@{0}", propertyInfo.Name)].Value = itemValue ?? DBNull.Value;
                    this.Connection.Sqlexecute.Parameters[string.Format("@{0}", propertyInfo.Name)].Direction = ParameterDirection.InputOutput;
                }
                else
                {
                    this.Connection.Sqlexecute.Parameters.AddWithValue(string.Format("@{0}", propertyInfo.Name), itemValue ?? DBNull.Value);
                }
            }

            long returnvalue = this.Connection.ExecuteProc(saveStoredProcedure);

            try
            {
                if (returnvalue >= 0)
                {
                    foreach (SqlParameter parameter in this.Connection.Sqlexecute.Parameters)
                    {
                        if (parameter.Direction != ParameterDirection.InputOutput)
                        {
                            continue;
                        }

                        returnvalue = (long)parameter.Value;
                        break;
                    }
                }

                this.Connection.Sqlexecute.Parameters.Clear();
            }
            catch (Exception)
            {
                entity.ActionResult.Result = ApiActionResult.Failure;
                entity = this.SetKeyValue(entity, -1);
                entity.ActionResult.Message = this.Connection.ErrorMessage;
                this._logger.Write(
                    string.Empty,
                    "0",
                    0,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    this.Connection.ErrorMessage,
                    string.Format("DataAccess.{0} - {1}", entity.ClassName(), entity.KeyValue));
            }

            entity = this.SetKeyValue(entity, returnvalue);

            if (entity.ActionResult.Result != ApiActionResult.ForeignKeyFail)
            {
                entity.ActionResult.Result = ApiActionResult.Success;
            }

            if (this.Connection.ErrorMessage != string.Empty && entity.ActionResult.Result != ApiActionResult.ForeignKeyFail)
            {
                entity.ActionResult.Result = ApiActionResult.Failure;
                entity.ActionResult.Message = this.Connection.ErrorMessage;
            }

            this.Connection.Sqlexecute.Parameters.Clear();
            return entity;
        }

        /// <summary>
        /// The stored procedure save for a batch of entities.
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected List<T> StoredProcedureSaveBatch(List<T> entityList)
        {
            var currentType = new T();
            var returnList = new List<T>();

            var saveStoredProcedure = currentType.SaveStoredProcedureBatch;

            this.Connection.Sqlexecute.Parameters.Clear();
            this.Connection.ErrorMessage = string.Empty;

            var table = new DataTable();
            var propInfo = this.GetFields();
            var propertyInfos = propInfo as IList<FieldInfo> ?? propInfo.ToList();

            if (entityList.Count == 0)
            {
                return returnList;
            }

            foreach (FieldInfo propertyInfo in propertyInfos)
            {
                Type columnType = propertyInfo.FieldType;

                if (propertyInfo.FieldType == typeof(Object))
                {
                    // The `Object` type is not supported by ADO.NET
                    columnType = typeof(String);
                }

                else if (propertyInfo.FieldType == typeof(DateTime?))
                {
                    columnType = typeof(DateTime);
                }

                else if (propertyInfo.FieldType == typeof(int?))
                {
                    columnType = typeof(int);
                }

                else if (propertyInfo.FieldType == typeof(long?))
                {
                    columnType = typeof(long);
                }

                else if (propertyInfo.FieldType == typeof(decimal?))
                {
                    columnType = typeof(decimal);
                }

                else if (propertyInfo.FieldType == typeof(bool?))
                {
                    columnType = typeof(bool);
                }

                else if (propertyInfo.FieldType == typeof(byte?))
                {
                    columnType = typeof(byte);
                }

                else if (propertyInfo.FieldType == typeof(AddressCreationMethod) || propertyInfo.FieldType == typeof(AddressCreationMethod?))
                {
                    columnType = typeof(int);
                }

                else if (propertyInfo.FieldType == typeof(TemplateMapping.ImportElementType) || propertyInfo.FieldType == typeof(TemplateMapping.ImportElementType?)
                    || propertyInfo.FieldType == typeof(Log.MessageLevel) || propertyInfo.FieldType == typeof(Log.MessageLevel?)
                    || propertyInfo.FieldType == typeof(PwdMethod) || propertyInfo.FieldType == typeof(PwdMethod?)
                    || propertyInfo.FieldType == typeof(EmployeeCreationMethod) || propertyInfo.FieldType == typeof(EmployeeCreationMethod?)
                    || propertyInfo.FieldType == typeof(UserDefinedField.FieldType) || propertyInfo.FieldType == typeof(UserDefinedField.FieldType?))
                {
                    columnType = typeof(byte);
                }

                table.Columns.Add(propertyInfo.Name, columnType);
            }

            foreach (T entity in entityList)
            {
                if (entity.ActionResult == null)
                {
                    entity.ActionResult = new ApiResult();
                }

                DataRow row = table.NewRow();

                if (propertyInfos.Count > 0)
                {
                    foreach (FieldInfo propertyInfo in propertyInfos)
                    {
                        row[propertyInfo.Name] = propertyInfo.GetValue(entity) ?? DBNull.Value;
                    }

                    string[] namespaceArray = entity.GetType().ToString().ToLower().Split('.');

                    if (namespaceArray[namespaceArray.Length - 1].StartsWith("esr") && row.ItemArray[0].ToString() != "0")
                    {
                        DataRow existingRow =
                            table.Select(string.Format("{0}={1}", table.Columns[0].ColumnName, row[0])).FirstOrDefault();

                        if (existingRow != null)
                        {
                            table.Rows.Remove(existingRow);
                        }
                    }

                    table.Rows.Add(row);
                }

                entity.ActionResult.Result = ApiActionResult.Success;
                returnList.Add(entity);
            }

            table.AcceptChanges();

            if (table.Rows.Count > 0)
            {
                this.Connection.Sqlexecute.Parameters.Add(new SqlParameter("@list", SqlDbType.Structured) { TypeName = string.Format("{0}Type", saveStoredProcedure), Value = table });

                if (typeof(T) == typeof(Employee))
                {
                    List<Employee> employees = entityList.Cast<Employee>().ToList();

                    using (IDataReader reader = this.Connection.GetStoredProcReader(saveStoredProcedure))
                    {
                        while (reader != null && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                        {
                            int employeeIdOrdinal = reader.GetOrdinal("employeeId");
                            int esrPersonIdOrdinal = reader.GetOrdinal("ESRPersonId");
                            int returnCodeOrdinal = reader.GetOrdinal("ReturnCode");
                            int employeeId = reader.GetInt32(employeeIdOrdinal);
                            long esrPersonId = reader.GetInt64(esrPersonIdOrdinal);
                            int returnCode = reader.GetInt32(returnCodeOrdinal);

                            Employee employee = employees.FirstOrDefault(x => x.ESRPersonId == esrPersonId);

                            if (employee != null)
                            {
                                employee.employeeid = "-1 -2".Contains(returnCode.ToString(CultureInfo.InvariantCulture)) ? returnCode : employeeId;
                            }
                        }
                    }
                }
                else if (typeof(T) == typeof(Car))
                {
                    List<Car> cars = entityList.Cast<Car>().ToList();

                    using (IDataReader reader = this.Connection.GetStoredProcReader(saveStoredProcedure))
                    {
                        while (reader != null && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                        {
                            int registrationOrdinal = reader.GetOrdinal("registration");
                            int carIdOrdinal = reader.GetOrdinal("carid");
                            string registration = reader.GetString(registrationOrdinal);
                            int carId = reader.GetInt32(carIdOrdinal);

                            Car car = cars.FirstOrDefault(x => x.registration == registration);

                            if (car != null)
                            {
                                car.carid = carId;
                            }
                        }
                    }
                }
                else if (typeof(T) == typeof(Address))
                {
                    List<Address> addresses = entityList.Cast<Address>().ToList();

                    using (IDataReader reader = this.Connection.GetStoredProcReader(saveStoredProcedure))
                    {
                        while (reader != null && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                        {
                            int addressTypeOrdinal = reader.GetOrdinal("EsrAddressType");
                            int esrIdOrdinal = reader.GetOrdinal("EsrId");
                            var addressType = (EsrAddressType)reader.GetInt32(addressTypeOrdinal);
                            int addressIdOrdinal = reader.GetOrdinal("AddressId");
                            long esrId = reader.GetInt64(esrIdOrdinal);
                            int addressId = reader.GetInt32(addressIdOrdinal);

                            Address address = addressType == EsrAddressType.UseEsrAddressType ? addresses.FirstOrDefault(x => x.EsrAddressID == esrId) : addresses.FirstOrDefault(x => x.EsrLocationID == esrId);

                            if (address != null)
                            {
                                address.AddressID = addressId;
                            }
                        }
                    }
                }
                else if (typeof(T) == typeof(EsrAssignment))
                {
                    List<EsrAssignment> assignments = entityList.Cast<EsrAssignment>().ToList();

                    using (IDataReader reader = this.Connection.GetStoredProcReader(saveStoredProcedure))
                    {
                        int esrAssignIdOrdinal = reader.GetOrdinal("esrAssignID");
                        int esrPersonIdOrdinal = reader.GetOrdinal("ESRPersonId");
                        int assignmentNumberOrdinal = reader.GetOrdinal("AssignmentNumber");
                        int effectiveStartDateOrdinal = reader.GetOrdinal("EffectiveStartDate");
                        while ((reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                        {
                            int esrAssignId = reader.GetInt32(esrAssignIdOrdinal);
                            long personId = reader.GetInt64(esrPersonIdOrdinal);
                            string assignmentNumber = reader.GetString(assignmentNumberOrdinal);
                            DateTime effectiveStartDate = reader.GetDateTime(effectiveStartDateOrdinal);

                            foreach (EsrAssignment assignment in assignments.Where(x => x.ESRPersonId == personId && x.AssignmentNumber == assignmentNumber && x.EffectiveStartDate == effectiveStartDate))
                        {
                                assignment.esrAssignID = esrAssignId;
                            }
                        }

                        reader.Close();
                    }
                }
                else if (typeof(T) == typeof(ImportLog))
                {
                    List<ImportLog> importLogs = entityList.Cast<ImportLog>().ToList();
                    int logId = 0;

                    using (IDataReader reader = this.Connection.GetStoredProcReader(saveStoredProcedure))
                    {
                        while (reader != null
                               && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                        {
                            logId = reader.GetInt32(0);
                            break;
                        }
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }

                    if (logId > 0)
                    {
                        foreach (ImportLog importLog in importLogs)
                        {
                            importLog.logID = logId;
                        }
                    }
                }
                else if (typeof(T) == typeof(ImportHistory))
                {
                    List<ImportHistory> historyLogs = entityList.Cast<ImportHistory>().ToList();
                    int historyId = 0;

                    using (IDataReader reader = this.Connection.GetStoredProcReader(saveStoredProcedure))
                    {
                        while (reader != null
                               && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                        {
                            historyId = reader.GetInt32(0);
                            break;
                        }
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }

                    if (historyId > 0)
                    {
                        foreach (ImportHistory historyLog in historyLogs)
                        {
                            historyLog.historyId = historyId;
                        }
                        }
                    }
                else
                {
                    this.Connection.ExecuteProc(saveStoredProcedure);
                }
            }
            this.Connection.Sqlexecute.Parameters.Clear();
            table.Rows.Clear();
            table.Dispose();

            return entityList;
        }

        /// <summary>
        /// The generic read method.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="bigEntityId">
        /// The entity id as a long.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        protected List<T> StoredProcedureRead(int entityId = 0, long bigEntityId = 0)
        {
            var result = new List<T>();
            var currentType = new T();
            string keyFieldName = string.Empty;
            var keyField = currentType.KeyField;
            if (keyField != null)
            {
                keyFieldName = keyField.Name;
            }

            if (this.Connection.ConnectionStringValid)
            {
                this.Connection.Sqlexecute.Parameters.Clear();
                this.Connection.ErrorMessage = string.Empty;
                if (keyFieldName != string.Empty)
                {
                    var valToUse = entityId > 0 ? entityId : bigEntityId;
                    this.Connection.Sqlexecute.Parameters.AddWithValue(string.Format("@{0}", keyFieldName), valToUse);
                }

                using (IDataReader reader = this.Connection.GetStoredProcReader(currentType.ReadStoredProcedure))
                {
                    while (reader != null && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
                    {
                        result.Add(this.CreateNewObject(reader));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The generic read method.
        /// </summary>
        /// <param name="entityIdList"></param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        ///  TODO This needs to worked on for batch reads to increase system performance
        protected List<T> StoredProcedureReadBatch(List<long> entityIdList)
        {
            var result = new List<T>();
            var currentType = new T();

            if (!this.Connection.ConnectionStringValid)
            {
                return result;
            }

            this.Connection.Sqlexecute.Parameters.Clear();
            this.Connection.ErrorMessage = string.Empty;

            var table = new DataTable();
            table.Columns.Add("c1", typeof(int));

            foreach (long id in entityIdList)
            {
                DataRow row = table.NewRow();
                row["c1"] = id;
                table.Rows.Add(row);
            }

            this.Connection.Sqlexecute.Parameters.Add(new SqlParameter("@CHANGEME", SqlDbType.Structured) { TypeName = "dbo.CHANGEME", Value = table });

            using (IDataReader reader = this.Connection.GetStoredProcReader(currentType.ReadBatchStoredProcedure))
            {
                result = this.CreateNewBatchObjects(reader);
            }

            return result;
        }

        /// <summary>
        /// The stored procedure read by id.
        /// </summary>
        /// <param name="esrId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        protected List<T> StoredProcedureReadById(long esrId)
        {
            var result = new List<T>();
            var currentType = new T();

            if (this.Connection.ConnectionStringValid && currentType.ReadByEsrIdStoredProcedure != null)
            {
                this.Connection.Sqlexecute.Parameters.Clear();
                this.Connection.ErrorMessage = string.Empty;
                this.Connection.Sqlexecute.Parameters.AddWithValue("@EsrId", esrId);

                using (IDataReader reader = this.Connection.GetStoredProcReader(currentType.ReadByEsrIdStoredProcedure))
                {
                    while (reader.Read())
                    {
                        result.Add(this.CreateNewObject(reader));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The stored procedure read by id.
        /// </summary>
        /// <param name="assignmentId">
        /// The assignment id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        protected List<T> StoredProcedureReadByAssignmentId(long assignmentId)
        {
            var result = new List<T>();
            var currentType = new T();

            if (this.Connection.ConnectionStringValid && currentType.ReadByAssignmentIdStoredProcedure != null)
            {
                this.Connection.Sqlexecute.Parameters.Clear();
                this.Connection.ErrorMessage = string.Empty;
                this.Connection.Sqlexecute.Parameters.AddWithValue("@AssignmentId", assignmentId);

                using (IDataReader reader = this.Connection.GetStoredProcReader(currentType.ReadByAssignmentIdStoredProcedure))
                {
                    while (reader.Read())
                    {
                        result.Add(this.CreateNewObject(reader));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The stored procedure read special.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected List<T> StoredProcedureReadSpecial(string reference)
        {
            var result = new List<T>();
            var currentType = new T();

            if (this.Connection.ConnectionStringValid && currentType.ReadSpecialStoredProcedure != null)
            {
                this.Connection.Sqlexecute.Parameters.Clear();
                this.Connection.ErrorMessage = string.Empty;
                this.Connection.Sqlexecute.Parameters.AddWithValue("@reference", reference);

                using (IDataReader reader = this.Connection.GetStoredProcReader(currentType.ReadSpecialStoredProcedure))
                {
                    while (reader.Read())
                    {
                        result.Add(this.CreateNewObject(reader));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The stored procedure delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T StoredProcedureDelete(T entity)
        {
            var currentType = new T();
            var deleteStoredProcedure = currentType.DeleteStoredProcedure;
            if (entity != null)
            {
                if (entity.ActionResult == null)
                {
                    entity.ActionResult = new ApiResult();
                }

                this.Connection.Sqlexecute.Parameters.Clear();
                this.Connection.ErrorMessage = string.Empty;
                this.Connection.Sqlexecute.Parameters.AddWithValue(string.Format("@{0}", currentType.KeyField.Name), entity.KeyValue);
                this.Connection.Sqlexecute.Parameters.Add("@returnCode", SqlDbType.Int);
                this.Connection.Sqlexecute.Parameters["@returnCode"].Direction = ParameterDirection.ReturnValue;
                var returnvalue = this.Connection.ExecuteProc(deleteStoredProcedure);

                entity = this.SetKeyValue(entity, returnvalue);
                if (this.Connection.ErrorMessage != string.Empty)
                {
                    entity.ActionResult.Result = ApiActionResult.Failure;
                    entity.ActionResult.Message = this.Connection.ErrorMessage;
                }

                if (returnvalue <= -1)
                {
                    if (this.Connection.Sqlexecute.Parameters["@returnCode"].Value != null)
                    {
                        returnvalue = (int)this.Connection.Sqlexecute.Parameters["@returnCode"].Value;
                    }
                }

                entity.ActionResult.Result = returnvalue >= 0 ? ApiActionResult.Deleted : ApiActionResult.Failure;

                if (entity.ClassName() == "Employee")
                {
                    switch (returnvalue)
                    {
                        case 1:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are assigned to one or more Signoff Groups.";
                            break;
                        case 2:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they have one or more advances allocated to them.";
                            break;
                        case 3:
                            entity.ActionResult.Message = "This employee is currently set as a budget holder.";
                            break;
                        case 4:
                            entity.ActionResult.Message = "You must archive an employee before it can be deleted.";
                            break;
                        case 5:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are the owner of one or more contracts.";
                            break;
                        case 6:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are on one or more contract audiences as an individual.";
                            break;
                        case 7:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are on one or more attachment audiences as an individual.";
                            break;
                        case 8:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are on one or more contract notification lists.";
                            break;
                        case 9:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are the leader of one or more teams.";
                            break;
                        case 10:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as it would leave one or more empty teams.";
                            break;
                        case 11:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are associated to one or more contracts history.";
                            break;
                        case 12:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are associated to one or more report folders.";
                            break;
                        case 13:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are associated to one or more audiences.";
                            break;
                        case -10:
                            entity.ActionResult.Message =
                                "This employee cannot be deleted as they are referenced in either a GreenLight or by a user defined field.";
                            break;
                    }
                }

                this.Connection.Sqlexecute.Parameters.Clear();
            }

            return entity;
        }

        /// <summary>
        /// The set key value integer.
        /// </summary>
        /// <param name="currentType">
        /// The current type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T SetKeyValue(T currentType, int value)
        {
            var fieldInfo = currentType.KeyField;
            fieldInfo.SetValue(currentType, value);
            return currentType;
        }

        /// <summary>
        /// The set key value long.
        /// </summary>
        /// <param name="currentType">
        /// The current type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T SetKeyValue(T currentType, long value)
        {
            var fieldInfo = currentType.KeyField;
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType == typeof(int))
                {
                    int integerValue;
                    int.TryParse(value.ToString(CultureInfo.InvariantCulture), out integerValue);
                    fieldInfo.SetValue(currentType, integerValue);
                }
                else
                {
                    fieldInfo.SetValue(currentType, value);
                }
            }

            return currentType;
        }


        /// <summary>
        /// The set date field.
        /// </summary>
        /// <param name="currentType">
        /// The current type.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="nullField">
        /// The null Field.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected T SetDateField(T currentType, string field, string nullField = "")
        {
            var currentTypeProperties = this.GetFields();
            field = field.ToLower();
            foreach (FieldInfo fieldInfo in currentTypeProperties)
            {
                if (fieldInfo.Name.ToLower() == field)
                {
                    fieldInfo.SetValue(currentType, DateTime.Now);
                }

                if (fieldInfo.Name.ToLower() == nullField)
                {
                    fieldInfo.SetValue(currentType, null);
                }
            }

            return currentType;
        }

        /// <summary>
        /// The create new object.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private T CreateNewObject(IDataReader reader)
        {
            var newEsr = new T();
            var propInfo = this.GetFields();
            string fieldName = string.Empty;
            try
            {
                foreach (FieldInfo propertyInfo in propInfo)
                {
                    fieldName = propertyInfo.Name;
                    if (!reader.IsDBNull(reader.GetOrdinal(propertyInfo.Name)))
                    {
                        if (propertyInfo.FieldType == typeof(int))
                        {
                            var itemValue = reader.GetInt32(reader.GetOrdinal(propertyInfo.Name));
                            propertyInfo.SetValue(newEsr, itemValue);
                        }

                        else if (propertyInfo.FieldType == typeof(string))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetString(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(bool))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetBoolean(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(float))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetFloat(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(decimal))
                        {
                            var itemValue = reader.GetValue(reader.GetOrdinal(propertyInfo.Name));
                            propertyInfo.SetValue(newEsr, itemValue as decimal?);
                        }

                        else if (propertyInfo.FieldType == typeof(decimal?))
                        {
                            var itemValue = reader.GetValue(reader.GetOrdinal(propertyInfo.Name));
                            propertyInfo.SetValue(newEsr, itemValue as decimal?);
                        }

                        else if (propertyInfo.FieldType == typeof(char))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetChar(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(DateTime))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetDateTime(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(DateTime?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetDateTime(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(int?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(byte?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(byte))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(long?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt64(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(long))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt64(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(AddressCreationMethod))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(AddressCreationMethod?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(TemplateMapping.ImportElementType))
                        {
                            propertyInfo.SetValue(newEsr, (TemplateMapping.ImportElementType)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(Log.MessageLevel))
                        {
                            propertyInfo.SetValue(newEsr, (Log.MessageLevel)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(PwdMethod))
                        {
                            propertyInfo.SetValue(newEsr, (PwdMethod)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(EmployeeCreationMethod))
                        {
                            propertyInfo.SetValue(newEsr, (EmployeeCreationMethod)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(UserDefinedField.FieldType))
                        {
                            propertyInfo.SetValue(newEsr, (UserDefinedField.FieldType)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(Guid))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetGuid(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        else if (propertyInfo.FieldType == typeof(Guid?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetGuid(reader.GetOrdinal(propertyInfo.Name)));
                        }
                    }
                    else
                    {
                        propertyInfo.SetValue(newEsr, null);
                    }
                }

                newEsr.ActionResult.Result = ApiActionResult.Success;
            }
            catch (Exception ex)
            {
                var errorData = new T
                {
                    ActionResult =
                    {
                        Result = ApiActionResult.Failure,
                        Message = ex.Message
                    }
                };
                newEsr = errorData;
                this._logger.Write(
                    string.Empty,
                    "0",
                    0,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    ex.Message,
                    string.Format("DataAccess.{0} - {1} - {2}", newEsr.ClassName(), newEsr.KeyValue, fieldName));
            }

            return newEsr;
        }

        /// <summary>
        /// The create new object.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private List<T> CreateNewBatchObjects(IDataReader reader)
        {
            var returnBatch = new List<T>();

            while (reader != null && (reader.Read() && string.IsNullOrWhiteSpace(this.Connection.ErrorMessage)))
            {
                var newEsr = new T();
                var propInfo = this.GetFields();
                string fieldName = string.Empty;
                try
                {
                    foreach (FieldInfo propertyInfo in propInfo)
                    {
                        fieldName = propertyInfo.Name;
                        if (!reader.IsDBNull(reader.GetOrdinal(propertyInfo.Name)))
                        {
                            if (propertyInfo.FieldType == typeof(int))
                            {
                                var itemValue = reader.GetInt32(reader.GetOrdinal(propertyInfo.Name));
                                propertyInfo.SetValue(newEsr, itemValue);
                            }

                            else if (propertyInfo.FieldType == typeof(string))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetString(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(bool))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetBoolean(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(float))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetFloat(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(decimal))
                            {
                                var itemValue = reader.GetValue(reader.GetOrdinal(propertyInfo.Name));
                                propertyInfo.SetValue(newEsr, itemValue as decimal?);
                            }

                            else if (propertyInfo.FieldType == typeof(decimal?))
                            {
                                var itemValue = reader.GetValue(reader.GetOrdinal(propertyInfo.Name));
                                propertyInfo.SetValue(newEsr, itemValue as decimal?);
                            }

                            else if (propertyInfo.FieldType == typeof(char))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetChar(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(DateTime))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetDateTime(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(DateTime?))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetDateTime(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(int?))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(byte?))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(byte))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(long?))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetInt64(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(long))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetInt64(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(AddressCreationMethod))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(AddressCreationMethod?))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(TemplateMapping.ImportElementType))
                            {
                                propertyInfo.SetValue(newEsr, (TemplateMapping.ImportElementType)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(Log.MessageLevel))
                            {
                                propertyInfo.SetValue(newEsr, (Log.MessageLevel)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(PwdMethod))
                            {
                                propertyInfo.SetValue(newEsr, (PwdMethod)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(EmployeeCreationMethod))
                            {
                                propertyInfo.SetValue(newEsr, (EmployeeCreationMethod)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(UserDefinedField.FieldType))
                            {
                                propertyInfo.SetValue(newEsr, (UserDefinedField.FieldType)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(Guid))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetGuid(reader.GetOrdinal(propertyInfo.Name)));
                            }

                            else if (propertyInfo.FieldType == typeof(Guid?))
                            {
                                propertyInfo.SetValue(newEsr, reader.GetGuid(reader.GetOrdinal(propertyInfo.Name)));
                            }
                        }
                        else
                        {
                            propertyInfo.SetValue(newEsr, null);
                        }
                    }

                    newEsr.ActionResult.Result = ApiActionResult.Success;
                }
                catch (Exception ex)
                {
                    var errorData = new T
                    {
                        ActionResult =
                        {
                            Result = ApiActionResult.Failure,
                            Message = ex.Message
                        }
                    };
                    newEsr = errorData;
                    this._logger.Write(
                        string.Empty,
                        "0",
                        0,
                        LogRecord.LogItemTypes.None,
                        LogRecord.TransferTypes.None,
                        0,
                        string.Empty,
                        LogRecord.LogReasonType.None,
                        ex.Message,
                        string.Format("DataAccess.{0} - {1} - {2}", newEsr.ClassName(), newEsr.KeyValue, fieldName));
                }

                returnBatch.Add(newEsr);
            }

            return returnBatch;
        }
        /// <summary>
        /// The get fields.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>FieldInfo[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        private IEnumerable<FieldInfo> GetFields()
        {
            return this.currentTypeFields ?? (this.currentTypeFields = new T().ClassFields());
        }
    }
}