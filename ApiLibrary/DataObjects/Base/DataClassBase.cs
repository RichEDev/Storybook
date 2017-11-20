namespace ApiLibrary.DataObjects.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Web.Script.Serialization;

    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The API action result.
    /// </summary>
    public enum ApiActionResult
    {
        /// <summary>
        /// The success return code.
        /// </summary>
        [EnumMember]
        Success = 0,

        /// <summary>
        /// The failure return code.
        /// </summary>
        [EnumMember]
        Failure = 1,

        /// <summary>
        /// The no action return code - no action taken.
        /// </summary>
        [EnumMember]
        NoAction = -1,

        /// <summary>
        /// The deleted return code.
        /// </summary>
        [EnumMember]
        Deleted = 2,

        /// <summary>
        /// The partial success return code.
        /// The main record has updated but some of the sub objects have not, see message for details.
        /// </summary>
        [EnumMember]
        PartialSuccess = -3,

        /// <summary>
        /// The foreign key fail.
        /// </summary>
        [EnumMember]
        ForeignKeyFail = -4,

        /// <summary>
        /// The Validate has failed on required fields return code.
        /// See message for details.
        /// </summary>
        [EnumMember]
        ValidationFailed = -9
    }

    /// <summary>
    /// The action for this record.
    /// </summary>
    public enum Action
    {
        /// <summary>
        /// The no action.
        /// </summary>
        [EnumMember]
        NoAction = 0,

        /// <summary>
        /// The create.
        /// </summary>
        [EnumMember]
        Create = 1,

        /// <summary>
        /// The read.
        /// </summary>
        [EnumMember]
        Read = 2,

        /// <summary>
        /// The update.
        /// </summary>
        [EnumMember]
        Update = 3,

        /// <summary>
        /// The delete.
        /// </summary>
        [EnumMember]
        Delete = 4
    }

    /// <summary>
    /// The data class base.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [KnownType(typeof(EsrAddressRecord))]
    [KnownType(typeof(EsrAssignmentRecord))]
    [KnownType(typeof(EsrPersonRecord))]
    [KnownType(typeof(EsrPhoneRecord))]
    [KnownType(typeof(EsrVehicleRecord))]
    [KnownType(typeof(EsrAssignmentCostingRecord))]
    public partial class DataClassBase
    {
        /// <summary>
        /// The class name.
        /// </summary>
        private string className = string.Empty;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataClassBase"/> class.
        /// </summary>
        public DataClassBase()
        {
            this.ActionResult = new ApiResult();
        }

        /// <summary>
        /// Gets or sets the API action result.
        /// </summary>
        [DataMember]
        public ApiResult ActionResult { get; set; }

        /// <summary>
        /// Gets or sets the action for this record.
        /// </summary>
        [DataMember(IsRequired = true)]
        public Action Action { get; set; }

        /// <summary>
        /// Gets the read stored procedure.
        /// </summary>
        [ScriptIgnore]
        public string ReadStoredProcedure
        {
            get
            {
                var readClassName = this.ClassName();
                if (readClassName.EndsWith("ss"))
                {
                    return string.Format("dbo.APIget{0}es", readClassName);
                }

                if (readClassName.EndsWith("s"))
                {
                    return string.Format("dbo.APIget{0}", readClassName);
                }

                return string.Format("dbo.APIget{0}s", readClassName);
            }
        }

        /// <summary>
        /// Gets the delete stored procedure.
        /// </summary>
        [ScriptIgnore]
        public string DeleteStoredProcedure
        {
            get
            {
                return string.Format("dbo.APIdelete{0}", this.ClassName());
            }
        }

        /// <summary>
        /// Gets the save stored procedure.
        /// </summary>
        [ScriptIgnore]
        public string SaveStoredProcedure
        {
            get
            {
                return string.Format("dbo.APIsave{0}", this.ClassName());
            }
        }

        /// <summary>
        /// Gets the read by person id stored procedure.
        /// </summary>
        [ScriptIgnore]
        public string ReadByPersonIdStoredProcedure
        {
            get
            {
                var readClassName = this.ClassName();
                return string.Format(readClassName.EndsWith("s") ? "dbo.APIget{0}ByPerson" : "dbo.APIget{0}sByPerson", readClassName);
            }
        }

        /// <summary>
        /// Gets the read special stored procedure.
        /// </summary>
        [ScriptIgnore]
        public string ReadSpecialStoredProcedure
        {
            get
            {
                var readClassName = this.ClassName();
                return string.Format(readClassName.EndsWith("s") ? "dbo.APIget{0}Special" : "dbo.APIget{0}sSpecial", readClassName);
            }
        }

        /// <summary>
        /// Gets the key field for this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        [ScriptIgnore]
        public FieldInfo KeyField
        {
            get
            {
                var currentTypeProperties = this.ClassFields();

                foreach (FieldInfo fieldInfo in currentTypeProperties)
                {
                    var fieldAttribute = fieldInfo.GetCustomAttribute<DataClassAttribute>();
                    if (fieldAttribute != null && fieldAttribute.IsKeyField)
                    {
                        return fieldInfo;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the key value from current instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [ScriptIgnore]
        public long KeyValue
        {
            get
            {
                var keyValue = this.KeyField.GetValue(this);
                long keyLong;
                long.TryParse(keyValue.ToString(), out keyLong);
                return keyLong;
            }
        }
        
        /// <summary>
        /// Gets or sets the user defined fields.
        /// </summary>
        [ScriptIgnore]
        public UserDefinedFields UserDefinedFields { get; set; }

        /// <summary>
        /// Gets the table id.
        /// </summary>
        [ScriptIgnore]
        public Guid TableId
        {
            get
            {
                var attribute = this.GetType().GetCustomAttribute<DataClassAttribute>();
                if (attribute != null)
                {
                    return new Guid(attribute.TableId);
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the class attribute.
        /// </summary>
        [ScriptIgnore]
        public DataClassAttribute ClassAttribute
        {
            get
            {
                return this.GetType().GetCustomAttribute<DataClassAttribute>();
            }
        }

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        protected List<Field> Cache { get; set; }

        /// <summary>
        /// The get data class from record.
        /// </summary>
        /// <param name="sourceObject">
        /// The source object.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        public static DataClassBase GetDataClassFromRecord(DataClassBase sourceObject, Type outputType)
        {
            if (sourceObject == null)
            {
                return null;
            }

            var result = Activator.CreateInstance(outputType) as DataClassBase;
            var fields = sourceObject.ClassFields();

            foreach (FieldInfo fieldInfo in fields)
            {
                try
                {
                    fieldInfo.SetValue(result, fieldInfo.GetValue(sourceObject));
                }
                catch (Exception)
                {
                    fieldInfo.SetValue(result, null);
                }
            }

            var properties = sourceObject.ClassProperties();
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    if (property.CanWrite && !property.ToString().Contains("Generic.List"))
                    {
                        property.SetValue(result, property.GetValue(sourceObject));    
                    }
                }
                catch (Exception)
                {
                    property.SetValue(result, null);
                }
            }

            return result;
        }

        /// <summary>
        /// The get record from data class.
        /// </summary>
        /// <param name="sourceObject">
        /// The source object.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        public static DataClassBase GetRecordFromDataClass(DataClassBase sourceObject, Type outputType)
        {
            var result = Activator.CreateInstance(outputType) as DataClassBase;
            if (sourceObject != null)
            {
                var fields = sourceObject.ClassFields();

                foreach (FieldInfo fieldInfo in fields)
                {
                    try
                    {
                        fieldInfo.SetValue(result, fieldInfo.GetValue(sourceObject));
                    }
                    catch (Exception)
                    {
                        fieldInfo.SetValue(result, null);
                    }
                }

                var properties = sourceObject.ClassProperties();
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        if (property.CanWrite && !property.ToString().Contains("Generic.List"))
                        {
                            property.SetValue(result, property.GetValue(sourceObject));    
                        }
                    }
                    catch (Exception)
                    {
                        property.SetValue(result, null);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Validate current record.
        /// Checks to see if required date time fields have something other than the default value.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsValid()
        {
            var result = new StringBuilder();
            var fields = this.ClassFields();

            foreach (FieldInfo fieldInfo in fields)
            {
                try
                {
                    if (fieldInfo.FieldType.ToString() == "System.DateTime")
                    {
                        var attribute = fieldInfo.GetCustomAttribute<DataMemberAttribute>();
                        if (attribute != null && attribute.IsRequired)
                        {
                            var currentValue = fieldInfo.GetValue(this);
                            if ((DateTime)currentValue == new DateTime())
                            {
                                result.Append(
                                    string.Format(
                                        "{0} - {1}: Field Name:{2} should be a valid date",
                                        this.ClassName(),
                                        this.KeyValue,
                                        fieldInfo.Name));
                            }
                        }
                    }

                    if (fieldInfo.FieldType.ToString() == "System.String")
                    {
                        var attribute = fieldInfo.GetCustomAttribute<DataMemberAttribute>();
                        if (attribute != null && attribute.IsRequired)
                        {
                            var currentValue = fieldInfo.GetValue(this);

                            if (currentValue == null)
                            {
                                result.Append(
                                    string.Format(
                                        "{0} - {1}: Field Name:{2} should not be null",
                                        this.ClassName(),
                                        this.KeyValue,
                                        fieldInfo.Name));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Append(ex.Message);
                }
            }

            if (this.Action == Action.NoAction)
            {
                result.Append(string.Format("{0} - {1}: Field Name:{2} should be a valid Action Value", this.ClassName(), this.KeyValue, "Action"));
            }

            if (result.ToString() == string.Empty)
            {
                return true;
            }
            else
            {
                this.ActionResult = new ApiResult { Message = result.ToString(), Result = ApiActionResult.ValidationFailed };
                return false;
            }
        }

        /// <summary>
        /// Gets the fields in the current object.
        /// </summary>
        /// <returns>
        /// The <see cref="FieldInfo[]"/>.
        /// </returns>
        public FieldInfo[] ClassFields()
        {
            if (MemoryCache.Default.Contains(this.FieldKey()))
            {
                return MemoryCache.Default.Get(this.FieldKey()) as FieldInfo[];
            }

            var fields = this.GetType().GetFields();

            MemoryCache.Default.Add(
                this.FieldKey(), fields, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(60) });
            return fields;
        }

        /// <summary>
        /// Gets the class name of the current instance.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ClassName()
        {
            if (!string.IsNullOrEmpty(this.className) )
            {
                return this.className;
            }

            var typeString = this.GetType().ToString().Split('.');
            this.className = typeString[typeString.GetUpperBound(0)];
            return this.className;
        }

        /// <summary>
        /// Gets the class properties.
        /// </summary>
        /// <returns>
        /// The <see cref="PropertyInfo[]"/>.
        /// </returns>
        public PropertyInfo[] ClassProperties()
        {
            if (MemoryCache.Default.Contains(this.PropertyKey()))
            {
                return MemoryCache.Default.Get(this.PropertyKey()) as PropertyInfo[];
            }

            var properties = this.GetType().GetProperties();

            MemoryCache.Default.Add(
                this.PropertyKey(), properties, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(60) });
            return properties;
        }

        /// <summary>
        /// Get user defined fields for this entity.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public void GetUserDefined(int accountId, List<UserDefinedField> userdefined)
        {
            if (userdefined != null)
            {
                var element = this.ClassAttribute.ElementType;
                switch (element)
                {
                    case TemplateMapping.ImportElementType.Employee:
                        var employeeudf = new Guid("972AC42D-6646-4EFC-9323-35C2C9F95B62");
                        this.UserDefinedFields = new UserDefinedFields(employeeudf, "userdefined_employees");
                        foreach (UserDefinedField userDefinedField in userdefined.Where(userDefinedField => userDefinedField.tableid == employeeudf))
                        {
                            this.UserDefinedFields.Fields.Add(userDefinedField.MemberwiseClone() as UserDefinedField);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Update a field in this instance with a given integer.
        /// </summary>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        public void UpdateField(string fieldName, int? fieldValue)
        {
            var fields = this.ClassFields();
            fieldName = fieldName.ToLower();
            foreach (FieldInfo fieldInfo in fields.Where(fieldInfo => fieldInfo.Name.ToLower() == fieldName))
            {
                fieldInfo.SetValue(this, fieldValue);
                break;
            }
        }

        /// <summary>
        /// Gets the field key for the memory cache.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FieldKey()
        {
            return string.Format("Fields_{0}", this.ClassName());
        }

        /// <summary>
        /// Gets the property key.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string PropertyKey()
        {
            return string.Format("Properties_{0}", this.ClassName());
        }
    }
}