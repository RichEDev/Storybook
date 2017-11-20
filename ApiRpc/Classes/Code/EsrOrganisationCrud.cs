namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Caching;

    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// The ESR organisation crud.
    /// </summary>
    public class EsrOrganisationCrud : EntityBase, IDataAccess<EsrOrganisation>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private Log logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrOrganisationCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API collection.
        /// </param>
        public EsrOrganisationCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            this.logger = new Log();
            if (this.DataSources.EsrOrganisationApi == null)
            {
                this.DataSources.EsrOrganisationApi = new EsrOrganisationApi(metabase, accountid);
            }

            if (this.DataSources.TemplateMappingApi == null)
            {
                this.DataSources.TemplateMappingApi = new TemplateMappingApi(metabase, accountid);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> Create(List<EsrOrganisation> entities)
        {
            entities = this.CreateUpdateEsrOrganisations(entities);
            return this.DataSources.EsrOrganisationApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Read(int entityId)
        {
            return this.DataSources.EsrOrganisationApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Read(long entityId)
        {
            return this.DataSources.EsrOrganisationApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> ReadAll()
        {
            return this.DataSources.EsrOrganisationApi.ReadAll();
        }

        /// <summary>
        /// The read by person id.
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
        public List<EsrOrganisation> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrOrganisationApi.ReadByEsrId(personId);
        }

        public List<EsrOrganisation> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> Update(List<EsrOrganisation> entities)
        {
            entities = this.CreateUpdateEsrOrganisations(entities);
            return this.DataSources.EsrOrganisationApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Delete(int entityId)
        {
            return this.DataSources.EsrOrganisationApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Delete(long entityId)
        {
            return this.DataSources.EsrOrganisationApi.Delete(entityId);
        }

        public EsrOrganisation Delete(EsrOrganisation entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create update ESR organisations.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<EsrOrganisation> CreateUpdateEsrOrganisations(List<EsrOrganisation> entities)
        {
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);

            if (mapping != null)
            {
                ////TODO: Mapping for country?

                var organisations = new List<EsrOrganisation>();
                FieldInfo[] fields = null;
                foreach (EsrOrganisation organisation in entities)
                {
                    var esrOrganisation = organisation;
                    var baseAttribute = organisation.ClassAttribute.ElementType;
                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        if (templateMapping.importElementType == baseAttribute)
                        {
                            string fieldValue;
                            if (fields == null)
                            {
                                fields = organisation.ClassFields();
                            }

                            foreach (FieldInfo field in fields)
                            {
                                var attribute = field.GetCustomAttribute<DataClassAttribute>();
                                if (attribute != null && attribute.FieldId != null)
                                {
                                    if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                    {
                                        {
                                            if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                            {
                                                esrOrganisation =
                                                    (EsrOrganisation)this.LookupField(
                                                        templateMapping,
                                                        organisation,
                                                        esrOrganisation,
                                                        out fieldValue,
                                                        field,
                                                        Mapper<EsrOrganisation>.EsrToDataClass(entities));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    organisations.Add(esrOrganisation);
                }

                return organisations;
            }

            return new List<EsrOrganisation>();
        }
    }
}