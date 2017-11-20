namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Base;
    using Enum;
    using ESR;
    using Interfaces;
    using Spend_Management;

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
        /// <param name="esrApi">
        /// The API Crud Lookup Value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal static UserDefinedField SetUdf(TemplateMapping templateMapping, DataClassBase mappingSource, T mappingTarget, IEsrApi esrApi)
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
                                                var lookup = esrApi.ReadLookupValue(udf.RelatedTable.ToString(), matchField.fieldId.ToString(), string.Format("'{0}'", lookupValue));
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
        internal static List<T> SubmitList(List<T> entities, IEsrApi dataAccess)
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
                        entitiesResult.AddRange(dataAccess.Execute(DataAccessMethod.Create, "", currentEntities));
                    }
                    catch (Exception e)
                    {
                        if (logger == null)
                        {
                            logger = new Log();
                        }

                        logger.Write("", "0", 0, LogRecord.LogItemTypes.EsrServiceErrored, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, e.Message, "Mapper.SubmitList");    
                    }
                    
                    currentEntities.Clear();
                    count = 0;
                }
            }

            if (count != 0)
            {
                try
                {
                    entitiesResult.AddRange(dataAccess.Execute(DataAccessMethod.Create, "", currentEntities));
                }
                catch (Exception e)
                {
                    if (logger == null)
                    {
                        logger = new Log();
                    }

                    logger.Write("", "0", 0, LogRecord.LogItemTypes.EsrServiceErrored, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, e.Message, "Mapper.SubmitList");
                }
            }

            return entitiesResult;
        }

    }
}