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

    /// <summary>
    /// The ESR position crud.
    /// </summary>
    public class EsrPositionCrud : EntityBase, IDataAccess<EsrPosition>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPositionCrud"/> class.
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
        public EsrPositionCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrPositionApi == null)
            {
                this.DataSources.EsrPositionApi = new PositionApi(metabase, accountid);
            }

            if (this.DataSources.TemplateMappingApi == null)
            {
                this.DataSources.TemplateMappingApi = new TemplateMappingApi(metabase, accountid);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="positions">
        /// The positions.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> Create(List<EsrPosition> positions)
        {
            positions = this.CreateUpdateEsrPositions(positions);
            return this.DataSources.EsrPositionApi.Create(positions);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Read(int entityId)
        {
            return this.DataSources.EsrPositionApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Read(long entityId)
        {
            return this.DataSources.EsrPositionApi.Read(entityId);
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
        public List<EsrPosition> ReadAll()
        {
            return this.DataSources.EsrPositionApi.ReadAll();
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
        public List<EsrPosition> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrPositionApi.ReadByEsrId(personId);
        }

        public List<EsrPosition> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="positions">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> Update(List<EsrPosition> positions)
        {
            positions = this.CreateUpdateEsrPositions(positions);
            return this.DataSources.EsrPositionApi.Update(positions);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Delete(int entityId)
        {
            return this.DataSources.EsrPositionApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Delete(long entityId)
        {
            return this.DataSources.EsrPositionApi.Delete(entityId);
        }

        public EsrPosition Delete(EsrPosition entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create update ESR positions.
        /// </summary>
        /// <param name="positions">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<EsrPosition> CreateUpdateEsrPositions(List<EsrPosition> positions)
        {
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
            ////TODO: Mapping for country?
            var esrPositions = new List<EsrPosition>();

            if (mapping != null)
            {
                foreach (EsrPosition position in positions)
                {
                    var esrPosition = position;
                    var baseAttribute = position.ClassAttribute.ElementType;

                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        if (templateMapping.importElementType != baseAttribute)
                        {
                            continue;
                        }

                        foreach (FieldInfo field in esrPosition.ClassFields())
                        {
                            string fieldValue;
                            var attribute = field.GetCustomAttribute<DataClassAttribute>();

                            if ((attribute == null || attribute.FieldId == null) || (attribute.FieldId != templateMapping.fieldID.ToString().ToUpper()) || (templateMapping.dataType != 10 && templateMapping.dataType != 12))
                            {
                                continue;
                            }

                            esrPosition = (EsrPosition)this.LookupField(templateMapping, position, esrPosition, out fieldValue, field, Mapper<EsrPosition>.EsrToDataClass(positions));
                        }
                    }

                    esrPositions.Add(esrPosition);
                }
            }

            return esrPositions;
        }
    }
}