namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;
    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;

    /// <summary>
    /// The entity base.
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        internal readonly string MetaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        internal readonly int AccountId;

        /// <summary>
        /// Initialises a new instance of the <see cref="EntityBase"/> class. 
        /// </summary>
        /// <param name="baseUrl">
        /// The base URL.
        /// </param>
        /// <param name="metabase">
        /// the METABASE
        /// </param>
        /// <param name="accountid">
        /// Account Id
        /// </param>
        /// <param name="apiCollection">
        /// The API Collection.
        /// </param>
        public EntityBase(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
        {
            if (!string.IsNullOrEmpty(baseUrl) && !baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            this.Uri = baseUrl;
            this.MetaBase = metabase;
            this.AccountId = accountid;
            this.DataSources = apiCollection ?? new ApiCrudList();
            this.Logger = new Log();
            this.ApiCrudLookupValue = new LookupValueApi(this.MetaBase, this.AccountId);
        }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the trust VPD.
        /// </summary>
        public string TrustVpd { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        internal Log Logger { get; set; }

        /// <summary>
        /// Gets or sets the API CRUD lookup value.
        /// </summary>
        internal LookupValueApi ApiCrudLookupValue { get; set; }

        /// <summary>
        /// Gets or sets the data sources.
        /// </summary>
        protected ApiCrudList DataSources { get; set; }

        /// <summary>
        /// The lookup field.
        /// </summary>
        /// <param name="templateMapping">
        /// The template mapping.
        /// </param>
        /// <param name="mappingSource">
        /// The mapping source.
        /// </param>
        /// <param name="mappingTarget">
        /// The mapping target.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        /// <param name="targetField">
        /// The target Field.
        /// </param>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        internal DataClassBase LookupField(
            TemplateMapping templateMapping, DataClassBase mappingSource, DataClassBase mappingTarget, out string fieldValue, FieldInfo targetField, List<DataClassBase> entities = null)
        {
            fieldValue = string.Empty;
            foreach (FieldInfo field in mappingSource.ClassFields())
            {
                var attribute = field.GetCustomAttribute<DataClassAttribute>();
                if (attribute != null)
                {
                    if (attribute.ColumnRef == templateMapping.columnRef)
                    {
                        var targetValue = field.GetValue(mappingSource);
                        if (targetValue != null)
                        {
                            var result = this.ApiCrudLookupValue.Read(templateMapping.lookupTable.ToString(), templateMapping.matchField.ToString(), targetValue.ToString());
                            if (result != null && result.FirstColumnValue != null)
                            {
                                if (result.SecondColumnName == null)
                                {
                                    field.SetValue(mappingTarget, result.FirstColumnValue);
                                }
                                else
                                {
                                    field.SetValue(mappingTarget, result.SecondColumnValue);
                                    if (result.FirstColumnValue != null && !field.Name.Contains("Parent") && !field.Name.Contains("Supervisor"))
                                    {
                                        if ((int)result.FirstColumnValue == 0)
                                        {
                                            mappingTarget.UpdateField(result.FirstColumnName, null);
                                        }
                                        else
                                        {
                                            mappingTarget.UpdateField(result.FirstColumnName, (int)result.FirstColumnValue);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (mappingTarget.ActionResult == null)
                                {
                                    mappingTarget.ActionResult = new ApiResult();
                                }

                                field.SetValue(mappingTarget, null);

                                mappingTarget.ActionResult.Result = ApiActionResult.ForeignKeyFail;
                                if (field.Name.Contains("Parent") || field.Name.Contains("Supervisor"))
                                {
                                    mappingTarget.ActionResult.LookupUpdateFailure = true;
                                }

                                long keyValue = mappingTarget.ClassName() == "EsrAssignmentRecord" ? ((EsrAssignmentRecord)mappingTarget).AssignmentID : mappingTarget.KeyValue;

                                mappingTarget.ActionResult.Message = this.FormatFailure(
                                    mappingTarget.ClassName(), keyValue, string.Format("Lookup for {0} with value of {1} failed", templateMapping.destinationField, targetValue), ApiActionResult.ForeignKeyFail);
                            }

                            break;
                        }
                    }
                }
            }

            return mappingTarget;
        }

        /// <summary>
        /// The format failure.
        /// </summary>
        /// <param name="objectName">
        /// The object name.
        /// </param>
        /// <param name="keyValue">
        /// The key value.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string FormatFailure(string objectName, int keyValue, string message, ApiActionResult result)
        {
            string formatted = string.Format(result == ApiActionResult.ForeignKeyFail ? "\n {0} {1} Failed - {2}" : "\n {0} {1} did not update - {2}", objectName, keyValue, message);
            return formatted;
        }

        /// <summary>
        /// The format failure.
        /// </summary>
        /// <param name="objectName">
        /// The object name.
        /// </param>
        /// <param name="keyValue">
        /// The key value.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string FormatFailure(string objectName, long keyValue, string message, ApiActionResult result)
        {
            string formatted = string.Format(result == ApiActionResult.ForeignKeyFail ? "\n {0} {1} Failed - {2}" : "\n {0} {1} did not update - {2}", objectName, keyValue, message);
            return formatted;
        }

        /// <summary>
        /// Import the given element type?.
        /// </summary>
        /// <param name="elementType">
        /// The element type.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool ImportElementType(TemplateMapping.ImportElementType elementType, string trustVpd)
        {
            var templateMappingApi = new TemplateMappingApi(this.MetaBase, this.AccountId);
            var cache = templateMappingApi.ReadSpecial(trustVpd);
            return cache != null && cache.Any(templateMapping => templateMapping.importElementType == elementType && templateMapping.importField);
        }
    }
}