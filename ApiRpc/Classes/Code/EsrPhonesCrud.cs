namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.ESR;
    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR phones crud helper class.
    /// </summary>
    public class EsrPhonesCrud : EntityBase, IDataAccess<EsrPhone>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPhonesCrud"/> class.
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
        /// The API Collection.
        /// </param>
        public EsrPhonesCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrPhoneApi == null)
            {
                this.DataSources.EsrPhoneApi = new PhoneApi(metabase, accountid);
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
        public List<EsrPhone> Create(List<EsrPhone> entities)
        {
            return this.DataSources.EsrPhoneApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone Read(int entityId)
        {
            return this.DataSources.EsrPhoneApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone Read(long entityId)
        {
            return this.DataSources.EsrPhoneApi.Read(entityId);
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
        public List<EsrPhone> ReadAll()
        {
            return this.DataSources.EsrPhoneApi.ReadAll();
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
        public List<EsrPhone> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrPhoneApi.ReadByEsrId(personId);
        }

        public List<EsrPhone> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
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
        public List<EsrPhone> Update(List<EsrPhone> entities)
        {
            return this.DataSources.EsrPhoneApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone Delete(int entityId)
        {
            return this.DataSources.EsrPhoneApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone Delete(long entityId)
        {
            return this.DataSources.EsrPhoneApi.Delete(entityId);
        }

        /// <summary>
        /// The delete by entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone Delete(EsrPhone entity)
        {
            return this.DataSources.EsrPhoneApi.Delete(entity);
        }
    }
}