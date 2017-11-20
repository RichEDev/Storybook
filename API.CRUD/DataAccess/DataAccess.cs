namespace ApiCrud.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using ApiCrud.Interfaces;
    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;
    using Action = ApiLibrary.DataObjects.Base.Action;

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
        private readonly Log logger;

        /// <summary>
        /// The base url.
        /// </summary>
        private string baseUrl;

        /// <summary>
        /// The current type fields.
        /// </summary>
        private FieldInfo[] currentTypeFields = null;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataAccess{T}"/> class. 
        /// Initialises a new instance of the <see cref="DataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="dataConnection">
        /// The data Connection.
        /// </param>
        /// <param name="loggerDb">
        /// The logger Database connection.
        /// </param>
        public DataAccess(string baseUrl, IApiDbConnection dataConnection, ApiLibrary.Interfaces.IApiDbConnection loggerDb = null)
        {
            this.baseUrl = baseUrl;
            this.expdata = dataConnection;
            this.logger = loggerDb == null ? new Log() : new Log(loggerDb);
        }

        /// <summary>
        /// Gets or sets the base url.
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return this.baseUrl;
            }

            set
            {
                this.baseUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the API data connection.
        /// </summary>
        protected IApiDbConnection expdata { get; set; }

        #region I Data Access

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> Create(List<T> entity)
        {
            var result = new List<T>();
            if (entity != null)
            {
                foreach (T singleEntity in entity)
                {
                    if (singleEntity.Action == Action.Create || singleEntity.Action == Action.Update)
                    {
                        T createEntity = this.SetDateField(singleEntity, "createdon");
                        createEntity = this.SetDateField(createEntity, "modifiedon");
                        result.Add(this.StoredProcedureSave(createEntity));
                    }
                }
            }

            return result;
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
        /// The <see cref="List"/>.
        /// </returns>
        public List<T> ReadAll()
        {
            var result = this.StoredProcedureRead();
            return result;
        }

        /// <summary>
        /// The read by employee ID.
        /// </summary>
        /// <param name="personId">
        /// The employee Id.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<T> ReadByEsrId(long personId)
        {
            if (personId == 0)
            {
                return new List<T>();
            }

            var result = this.StoredProcedureReadById(personId);
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
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> Update(List<T> entity)
        {
            var result = new List<T>();
            foreach (T singleEntity in entity)
            {
                if (singleEntity.Action == Action.Create || singleEntity.Action == Action.Update)
                {
                    T anEntity = this.SetDateField(singleEntity, "createdon");
                    anEntity = this.SetDateField(anEntity, "modifiedon");
                    result.Add(this.StoredProcedureSave(anEntity));
                }
            }

            return result;
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
            this.expdata.Sqlexecute.Parameters.Clear();
            this.expdata.ErrorMessage = string.Empty;
            var propInfo = this.GetFields();
            if (entity.ActionResult == null)
            {
                entity.ActionResult = new ApiResult();
            }

            foreach (FieldInfo propertyInfo in propInfo)
            {
                var itemValue = propertyInfo.GetValue(entity);
                if (propertyInfo == currentType.KeyField)
                {
                    this.expdata.Sqlexecute.Parameters.Add(string.Format("@{0}", propertyInfo.Name), SqlDbType.BigInt);
                    this.expdata.Sqlexecute.Parameters[string.Format("@{0}", propertyInfo.Name)].Value = itemValue ?? DBNull.Value;
                    this.expdata.Sqlexecute.Parameters[string.Format("@{0}", propertyInfo.Name)].Direction = ParameterDirection.InputOutput;
                }
                else
                {
                    this.expdata.Sqlexecute.Parameters.AddWithValue(string.Format("@{0}", propertyInfo.Name), itemValue ?? DBNull.Value);    
                }
            }

            long returnvalue = this.expdata.ExecuteProc(saveStoredProcedure);
            try
            {
                if (returnvalue >= 0)
                {
                    foreach (SqlParameter parameter in this.expdata.Sqlexecute.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.InputOutput)
                        {
                            returnvalue = (long)parameter.Value;
                            break;
                        }
                    }
                }

                this.expdata.Sqlexecute.Parameters.Clear();
            }
            catch (Exception)
            {
                entity.ActionResult.Result = ApiActionResult.Failure;
                entity = this.SetKeyValue(entity, -1);
                entity.ActionResult.Message = this.expdata.ErrorMessage;
                this.logger.Write(
                    string.Empty,
                    0,
                    0,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    this.expdata.ErrorMessage,
                    string.Format("DataAccess.{0} - {1}", entity.ClassName(), entity.KeyValue));
            }

            entity = this.SetKeyValue(entity, returnvalue);
            if (entity.ActionResult.Result != ApiActionResult.ForeignKeyFail)
            {
                entity.ActionResult.Result = ApiActionResult.Success;
            }

            if (this.expdata.ErrorMessage != string.Empty && entity.ActionResult.Result != ApiActionResult.ForeignKeyFail)
            {
                entity.ActionResult.Result = ApiActionResult.Failure;
                entity.ActionResult.Message = this.expdata.ErrorMessage;
            }

            this.expdata.Sqlexecute.Parameters.Clear();
            return entity;
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

            if (this.expdata.ConnectionStringValid)
            {
                this.expdata.Sqlexecute.Parameters.Clear();
                this.expdata.ErrorMessage = string.Empty;
                if (keyFieldName != string.Empty)
                {
                    var valToUse = entityId > 0 ? entityId : bigEntityId;
                    this.expdata.Sqlexecute.Parameters.AddWithValue(string.Format("@{0}", keyFieldName), valToUse);
                }

                using (IDataReader reader = this.expdata.GetStoredProcReader(currentType.ReadStoredProcedure))
                {
                    while (reader != null && (reader.Read() && string.IsNullOrWhiteSpace(this.expdata.ErrorMessage)))
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
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        protected List<T> StoredProcedureReadById(long personId)
        {
            var result = new List<T>();
            var currentType = new T();

            if (this.expdata.ConnectionStringValid && currentType.ReadByPersonIdStoredProcedure != null)
            {
                this.expdata.Sqlexecute.Parameters.Clear();
                this.expdata.ErrorMessage = string.Empty;
                this.expdata.Sqlexecute.Parameters.AddWithValue("@personId", personId);

                using (IDataReader reader = this.expdata.GetStoredProcReader(currentType.ReadByPersonIdStoredProcedure))
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

            if (this.expdata.ConnectionStringValid && currentType.ReadSpecialStoredProcedure != null)
            {
                this.expdata.Sqlexecute.Parameters.Clear();
                this.expdata.ErrorMessage = string.Empty;
                this.expdata.Sqlexecute.Parameters.AddWithValue("@reference", reference);

                using (IDataReader reader = this.expdata.GetStoredProcReader(currentType.ReadSpecialStoredProcedure))
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

                this.expdata.Sqlexecute.Parameters.Clear();
                this.expdata.ErrorMessage = string.Empty;
                this.expdata.Sqlexecute.Parameters.AddWithValue(string.Format("@{0}", currentType.KeyField.Name), entity.KeyValue);
                this.expdata.Sqlexecute.Parameters.Add("@returnCode", SqlDbType.Int);
                this.expdata.Sqlexecute.Parameters["@returnCode"].Direction = ParameterDirection.ReturnValue;
                var returnvalue = this.expdata.ExecuteProc(deleteStoredProcedure);

                entity = this.SetKeyValue(entity, returnvalue);
                if (this.expdata.ErrorMessage != string.Empty)
                {
                    entity.ActionResult.Result = ApiActionResult.Failure;
                    entity.ActionResult.Message = this.expdata.ErrorMessage;
                }

                if (returnvalue <= -1)
                {
                    if (this.expdata.Sqlexecute.Parameters["@returnCode"].Value != null)
                    {
                        returnvalue = (int)this.expdata.Sqlexecute.Parameters["@returnCode"].Value;
                    }
                }

                if (returnvalue >= 0)
                {
                    entity.ActionResult.Result = ApiActionResult.Deleted;
                }
                else
                {
                    entity.ActionResult.Result = ApiActionResult.Failure;
                }

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

                this.expdata.Sqlexecute.Parameters.Clear();
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
                if (fieldInfo.FieldType == typeof (int))
                {
                    int integerValue;
                    int.TryParse(value.ToString(), out integerValue);
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

                        if (propertyInfo.FieldType == typeof(string))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetString(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(bool))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetBoolean(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(float))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetFloat(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(decimal))
                        {
                            var itemValue = reader.GetValue(reader.GetOrdinal(propertyInfo.Name));
                            propertyInfo.SetValue(newEsr, itemValue as decimal?);
                        }

                        if (propertyInfo.FieldType == typeof(decimal?))
                        {
                            var itemValue = reader.GetValue(reader.GetOrdinal(propertyInfo.Name));
                            propertyInfo.SetValue(newEsr, itemValue as decimal?);
                        }

                        if (propertyInfo.FieldType == typeof(char))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetChar(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(DateTime))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetDateTime(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(DateTime?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetDateTime(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(int?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt32(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(byte?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(byte))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(long?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt64(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(long))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetInt64(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(TemplateMapping.ImportElementType))
                        {
                            propertyInfo.SetValue(newEsr, (TemplateMapping.ImportElementType)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(Log.MessageLevel))
                        {
                            propertyInfo.SetValue(newEsr, (Log.MessageLevel)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(PwdMethod))
                        {
                            propertyInfo.SetValue(newEsr, (PwdMethod)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(EmployeeCreationMethod))
                        {
                            propertyInfo.SetValue(newEsr, (EmployeeCreationMethod)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(UserDefinedField.FieldType))
                        {
                            propertyInfo.SetValue(newEsr, (UserDefinedField.FieldType)reader.GetByte(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(Guid))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetGuid(reader.GetOrdinal(propertyInfo.Name)));
                        }

                        if (propertyInfo.FieldType == typeof(Guid?))
                        {
                            propertyInfo.SetValue(newEsr, reader.GetGuid(reader.GetOrdinal(propertyInfo.Name)));
                        }
                    }
                    else
                    {
                        {
                            propertyInfo.SetValue(newEsr, null);
                        }
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
                this.logger.Write(
                    string.Empty,
                    0,
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
        /// The get fields.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>FieldInfo[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        private FieldInfo[] GetFields()
        {
            return this.currentTypeFields ?? (this.currentTypeFields = new T().ClassFields());
        }
    }
}