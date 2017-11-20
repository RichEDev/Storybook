
namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ApiLibrary.DataObjects.ESR;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR TRUST CRUD.
    /// </summary>
    public class EsrTrustCrud : EntityBase, IDataAccess<EsrTrust>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrTrustCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API collection.
        /// </param>
        public EsrTrustCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrTrustApi == null)
            {
                this.DataSources.EsrTrustApi = new TrustApi(metabase, accountid);
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
        public List<EsrTrust> Create(List<EsrTrust> entities)
        {
            return this.DataSources.EsrTrustApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Read(int entityId)
        {
            return this.DataSources.EsrTrustApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Read(long entityId)
        {
            return this.DataSources.EsrTrustApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrTrust> ReadAll()
        {
            return this.DataSources.EsrTrustApi.ReadAll();
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
        public List<EsrTrust> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrTrustApi.ReadByEsrId(personId);
        }

        public List<EsrTrust> ReadSpecial(string reference)
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
        public List<EsrTrust> Update(List<EsrTrust> entity)
        {
            return this.DataSources.EsrTrustApi.Update(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Delete(int entityId)
        {
            return this.DataSources.EsrTrustApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Delete(long entityId)
        {
            return this.DataSources.EsrTrustApi.Delete(entityId);
        }

        public EsrTrust Delete(EsrTrust entity)
        {
            throw new NotImplementedException();
        }
    }
}