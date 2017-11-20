
namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.Spend_Management;
    using ApiCrud;
    using Crud;
    using Interfaces;

    /// <summary>
    /// The import template crud.
    /// </summary>
    public class TemplateCrud : EntityBase, IDataAccess<Template>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TemplateCrud"/> class.
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
        public TemplateCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.TemplateApi == null)
            {
                this.DataSources.TemplateApi = new TemplateApi(metabase, accountid);
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
        public List<Template> Create(List<Template> entities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Read(int entityId)
        {
            return this.DataSources.TemplateApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Read(long entityId)
        {
            return this.DataSources.TemplateApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Template> ReadAll()
        {
            throw new NotImplementedException();
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
        public List<Template> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
        }

        public List<Template> ReadSpecial(string reference)
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
        public List<Template> Update(List<Template> entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Delete(int entityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public Template Delete(Template entity)
        {
            throw new NotImplementedException();
        }
    }
}