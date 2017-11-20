namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

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
        /// <param name="metabase">
        /// the METABASE
        /// </param>
        /// <param name="accountId">
        /// Account Id
        /// </param>
        /// <param name="apiHandler">
        /// The API handler.
        /// </param>
        /// <param name="logger">
        /// Optional logger
        /// </param>
        public EntityBase(string metabase, int accountId, IEsrApi apiHandler = null, Log logger = null)
        {
            this.MetaBase = metabase;
            this.AccountId = accountId;
            this.EsrApiHandler = apiHandler ?? new EsrApi(metabase, accountId);
            this.Logger = logger ?? new Log();
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
        /// Gets or sets the data sources.
        /// </summary>
        protected IEsrApi EsrApiHandler { get; set; }

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
                            var result = this.EsrApiHandler.ReadLookupValue(templateMapping.lookupTable.ToString(), templateMapping.matchField.ToString(), targetValue.ToString());
                            if (result != null && result.FirstColumnValue != null)
                            {
                                if (result.SecondColumnName == null)
                                {
                                    field.SetValue(mappingTarget, result.FirstColumnValue);
                                }
                                else
                                {
                                    field.SetValue(mappingTarget, result.SecondColumnValue);
                                    if (result.FirstColumnValue != null && !field.Name.Contains("Parent") && !field.Name.Contains("Supervisor") && !field.Name.Contains("DepartmentManager"))
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
                                mappingTarget.ActionResult.Result = ApiActionResult.Success;
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
            var cache = this.EsrApiHandler.TemplateMappingReadSpecial(trustVpd);
            return cache != null && cache.Any(templateMapping => templateMapping.importElementType == elementType && templateMapping.importField);
        }
    }
}