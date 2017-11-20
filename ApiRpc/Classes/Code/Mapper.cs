namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The mapper for data class base.
    /// </summary>
    /// <typeparam name="T">
    /// Data class base class
    /// </typeparam>
    public static class Mapper<T> where T : DataClassBase, new()
    {
        /// <summary>
        /// Convert ESR class to data class base.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static List<DataClassBase> EsrToDataClass(List<T> entities)
        {
            return entities.Select(entity => entity as DataClassBase).ToList();
        }

        /// <summary>
        /// The set mapping field.
        /// </summary>
        /// <param name="targetCol">
        /// The target Col.
        /// </param>
        /// <param name="mappingSource">
        /// The mapping source.
        /// </param>
        /// <param name="targetFieldGuid">
        /// The target Field GUID.
        /// </param>
        /// <param name="mappingTarget">
        /// The mapping target.
        /// </param>
        /// <param name="fieldValue">
        /// The field Value.
        /// </param>
        /// <param name="targetField">
        /// The target Field.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        internal static T SetMappingField(int? targetCol, DataClassBase mappingSource, Guid targetFieldGuid, T mappingTarget, out string fieldValue, FieldInfo targetField)
        {
            if (mappingTarget == null)
            {
                mappingTarget = new T();
            }

            fieldValue = string.Empty;
            foreach (FieldInfo field in mappingSource.ClassFields())
            {
                var attribute = field.GetCustomAttribute<DataClassAttribute>();
                if (attribute != null)
                {
                    if (attribute.ColumnRef == targetCol)
                    {
                        var targetAttrib = targetField.GetCustomAttribute<DataClassAttribute>();
                        if (targetAttrib.FieldId == targetFieldGuid.ToString().ToUpper())
                        {
                            targetField.SetValue(mappingTarget, field.GetValue(mappingSource));
                            object fieldObject = targetField.GetValue(mappingTarget);
                            if (fieldObject != null)
                            {
                                fieldValue = fieldObject.ToString();
                            }

                            break;
                        }
                    }
                }
            }
            
            return mappingTarget;
        }

        /// <summary>
        /// The set User Defined Field value.
        /// </summary>
        /// <param name="templateMapping">
        /// The template Mapping.
        /// </param>
        /// <param name="mappingSource">
        /// The mapping Source.
        /// </param>
        /// <param name="mappingTarget">
        /// The mapping Target.
        /// </param>
        /// <param name="apiCrudLookupValue">
        /// The API Crud Lookup Value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal static UserDefinedField SetUdf(TemplateMapping templateMapping, DataClassBase mappingSource, T mappingTarget, LookupValueApi apiCrudLookupValue)
        {
            if (mappingTarget == null)
            {
                mappingTarget = new T();
            }

            UserDefinedField result = null;

            foreach (FieldInfo field in mappingSource.ClassFields())
            {
                bool udfValueSet = false;

                var attribute = field.GetCustomAttribute<DataClassAttribute>();
                if (attribute != null)
                {
                    if (attribute.ColumnRef == templateMapping.columnRef)
                    {
                        foreach (UserDefinedField udf in mappingTarget.UserDefinedFields.Fields)
                        {
                            if (udf.fieldid == templateMapping.fieldID)
                            {
                                switch (udf.fieldtype)
                                {
                                    case UserDefinedField.FieldType.DateTime:
                                        {
                                            DateTime tmpDate;
                                            if (field.GetValue(mappingSource) != null
                                                && DateTime.TryParse(
                                                    field.GetValue(mappingSource).ToString(), out tmpDate))
                                            {
                                                udf.Value = tmpDate.ToString("yyyy-MM-dd HH:mm:ss");
                                            }
                                        }

                                        break;
                                    case UserDefinedField.FieldType.Relationship:
                                        var lookupValue = field.GetValue(mappingSource);
                                        if (lookupValue != null && !string.IsNullOrEmpty(lookupValue.ToString()))
                                        {
                                            var foundValue = false;
                                            foreach (UserDefinedMatchField matchField in udf.UserDefinedMatchFields)
                                            {
                                                var lookup = apiCrudLookupValue.Read(udf.RelatedTable.ToString(), matchField.fieldId.ToString(), string.Format("'{0}'", lookupValue));
                                                if (lookup != null && lookup.FirstColumnValue != null)
                                                {
                                                    udf.Value = lookup.FirstColumnValue;
                                                    udf.ActionResult.Result = ApiActionResult.Success;
                                                    foundValue = true;
                                                }    
                                            }

                                            if (!foundValue)
                                            {
                                                udf.ActionResult.Result = ApiActionResult.ForeignKeyFail;
                                                udf.ActionResult.Message =
                                                    string.Format(
                                                        "The lookup for {0} = {1} has failed", field.Name, lookupValue);
                                                udf.ActionResult.LookupUpdateFailure = true;
                                            }
                                        }

                                        break;
                                    default:
                                        udf.Value = field.GetValue(mappingSource);
                                        break;
                                }

                                result = udf;
                                udfValueSet = true;
                                break;
                            }
                        }
                    }
                }

                if (udfValueSet)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse the dynamicVal string to see if any of the columns referenced within it match the current column for the employee.
        /// If there is a match replace the column reference with the actual value and return the parsed string value 
        /// </summary>
        /// <param name="dynamicVal">Formatted string value</param>
        /// <param name="destinationField">The string value of the destination field</param>
        /// <param name="fieldValue">The value of the field to replace</param>
        /// <returns>
        /// Formatted value
        /// </returns>
        internal static string SetStringFormatValues(string dynamicVal, string destinationField, string fieldValue)
        {
            var regex = new Regex(@"\[(?<myMatch>[^\]]*)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase); 
            MatchCollection matches = regex.Matches(dynamicVal);
            string newDynamicValue = dynamicVal;
            if (fieldValue != string.Empty)
            {
                foreach (Match match in matches)
                {
                    string matchVal = match.Groups["myMatch"].Value;
                    if (destinationField == matchVal)
                    {
                        newDynamicValue = newDynamicValue.Replace("[" + matchVal + "]", fieldValue);
                        break;
                    }
                }
            }
            
            return newDynamicValue;
        }

        /// <summary>
        /// Submit a list of entities to the create method in a batch.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <param name="dataAccess">
        /// The data access.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal static List<T> SubmitList(List<T> entities, IDataAccess<T> dataAccess)
        {
            int count = 0;
            var currentEntities = new List<T>();
            var entitiesResult = new List<T>();
            Log logger = null;
            foreach (T assignment in entities)
            {
                count += 1;
                currentEntities.Add(assignment);
                if (count >= EsrFile.RecordBatchSize)
                {
                    try
                    {
                        entitiesResult.AddRange(dataAccess.Create(currentEntities));
                    }
                    catch (Exception e)
                    {
                        if (logger == null)
                        {
                            logger = new Log();
                        }

                        logger.Write("", 0, 0, LogRecord.LogItemTypes.EsrServiceErrored, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, e.Message, "Mapper.SubmitList");    
                        foreach (T entity in currentEntities)
                        {
                            var entityData = new StringBuilder();
                            foreach (FieldInfo field in entity.ClassFields())
                            {
                                entityData.Append(field.Name);
                                entityData.Append("=");
                                entityData.Append(field.GetValue(entity));
                            }

                            logger.Write("", 0, 0, LogRecord.LogItemTypes.EsrServiceErrored, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, entityData.ToString(), "Mapper.SubmitList");
                        }
                    }
                    
                    currentEntities.Clear();
                    count = 0;
                }
            }

            if (count != 0)
            {
                try
                {
                    entitiesResult.AddRange(dataAccess.Create(currentEntities));
                }
                catch (Exception e)
                {
                    if (logger == null)
                    {
                        logger = new Log();
                    }

                    logger.Write("", 0, 0, LogRecord.LogItemTypes.EsrServiceErrored, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, e.Message, "Mapper.SubmitList");
                    foreach (T entity in currentEntities)
                    {
                        var entityData = new StringBuilder();
                        foreach (FieldInfo field in entity.ClassFields())
                        {
                            entityData.Append(field.Name);
                            entityData.Append("=");
                            entityData.Append(field.GetValue(entity));
                        }

                        logger.Write("", 0, 0, LogRecord.LogItemTypes.EsrServiceErrored, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, entityData.ToString(), "Mapper.SubmitList");
                    }
                }
            }

            return entitiesResult;
        }

        /// <summary>
        /// Static field mapping.
        /// From ESRStaticFieldMappings.xml
        /// </summary>
        /// <param name="inputValue">
        /// The input value.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte?</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal static string StaticFieldMapping(string inputValue, string fieldName)
        {
            var tableName = new T().ClassName().ToUpper();
            if (!MemoryCache.Default.Contains("EsrStaticFieldMappings"))
            {
                CacheFieldMappings("EsrStaticFieldMappings.xml");
            }

            var staticMappings = MemoryCache.Default.Get("EsrStaticFieldMappings") as List<StaticMapping>;
            if (staticMappings != null)
            {
                var map =
                    staticMappings.FirstOrDefault(mapping => mapping.Fieldname == fieldName.ToUpper() && mapping.Tablename == tableName
                                                             && mapping.InputValue.ToUpper() == inputValue.ToUpper());
                return map == null ? string.Empty : map.OutputValue;
            }

            return string.Empty;
        }

        /// <summary>
        /// Cache the static field mappings.
        /// </summary>
        /// <param name="xmlFilename">
        /// The xml filename.
        /// </param>
        private static void CacheFieldMappings(string xmlFilename)
        {
        string mappingsPath = ConfigurationManager.AppSettings["ImplementationSpreadsheetXMLTemplatePath"];
            var staticMappings = new List<StaticMapping>();
        XDocument xmlTemplate =
            XDocument.Load(mappingsPath + (mappingsPath.EndsWith("\\") ? xmlFilename : "\\" + xmlFilename));
            if (xmlTemplate.Root != null)
            {
                var tables = xmlTemplate.Root.Elements("Table");
                foreach (XElement table in tables)
                {
                    var fields = table.Elements();
                    foreach (XElement field in fields)
                    {
                        var mappings = field.Elements();
                        foreach (XElement fieldMap in mappings)
                        {
                            var mapping = new StaticMapping
                            {
                                Tablename = table.Attribute("Name").Value.ToUpper(),
                                Fieldname = field.Attribute("Name").Value.ToUpper(),
                                InputValue = fieldMap.Attribute("InputValue").Value,
                                OutputValue = fieldMap.Attribute("OutputValue").Value
                            };
                            staticMappings.Add(mapping);    
                        }
                        
                    }
                }
            }

            MemoryCache.Default.Add("EsrStaticFieldMappings", staticMappings, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
        }
    }
}