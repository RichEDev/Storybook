namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.ESR;
    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR addresses crud.
    /// </summary>
    public class EsrAddressesCrud : EntityBase, IDataAccess<EsrAddress>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAddressesCrud"/> class.
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
        public EsrAddressesCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrAddressApi == null)
            {
                this.DataSources.EsrAddressApi = new EsrAddressApi(metabase, accountid);
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
        public List<EsrAddress> Create(List<EsrAddress> entities)
        {
            return this.DataSources.EsrAddressApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Read(int entityId)
        {
            return this.DataSources.EsrAddressApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Read(long entityId)
        {
            return this.DataSources.EsrAddressApi.Read(entityId);
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
        public List<EsrAddress> ReadAll()
        {
            return this.DataSources.EsrAddressApi.ReadAll();
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
        public List<EsrAddress> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrAddressApi.ReadByEsrId(personId);
        }

        public List<EsrAddress> ReadSpecial(string reference)
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
        public List<EsrAddress> Update(List<EsrAddress> entities)
        {
            return this.DataSources.EsrAddressApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(int entityId)
        {
            return this.DataSources.EsrAddressApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(long entityId)
        {
            return this.DataSources.EsrAddressApi.Delete(entityId);
        }

        /// <summary>
        /// The delete by entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(EsrAddress entity)
        {
            return this.DataSources.EsrAddressApi.Delete(entity);
        }
    }
}