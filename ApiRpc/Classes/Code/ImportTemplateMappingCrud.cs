
namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.Spend_Management;
    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The import template crud.
    /// </summary>
    public class TemplateMappingCrud : EntityBase, IDataAccess<TemplateMapping>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TemplateMappingCrud"/> class.
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
        public TemplateMappingCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
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
        /// The <see cref="List"/>.
        /// </returns>
        public List<TemplateMapping> Create(List<TemplateMapping> entities)
        {
            return this.DataSources.TemplateMappingApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Read(int entityId)
        {
            return this.DataSources.TemplateMappingApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Read(long entityId)
        {
            return this.DataSources.TemplateMappingApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<TemplateMapping> ReadAll()
        {
            return this.DataSources.TemplateMappingApi.ReadAll();
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<TemplateMapping> ReadByEsrId(long personId)
        {
            return this.DataSources.TemplateMappingApi.ReadByEsrId(personId);
        }

        public List<TemplateMapping> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<TemplateMapping> Update(List<TemplateMapping> entity)
        {
            return this.DataSources.TemplateMappingApi.Update(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Delete(int entityId)
        {
            return this.DataSources.TemplateMappingApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Delete(long entityId)
        {
            return this.DataSources.TemplateMappingApi.Delete(entityId);
        }

        public TemplateMapping Delete(TemplateMapping entity)
        {
            throw new NotImplementedException();
        }
    }
}