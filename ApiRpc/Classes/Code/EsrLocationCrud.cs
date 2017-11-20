namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR location crud.
    /// </summary>
    public class EsrLocationCrud : EntityBase, IDataAccess<EsrLocation>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrLocationCrud"/> class.
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
        public EsrLocationCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrLocationApi == null)
            {
                this.DataSources.EsrLocationApi = new LocationApi(metabase, accountid);
            }

            if (this.DataSources.AddressApi == null)
            {
                this.DataSources.AddressApi = new AddressApi(metabase, accountid);
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
        public List<EsrLocation> Create(List<EsrLocation> entities)
        {
            return this.DataSources.EsrLocationApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Read(int entityId)
        {
            return this.DataSources.EsrLocationApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Read(long entityId)
        {
            return this.DataSources.EsrLocationApi.Read(entityId);
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
        public List<EsrLocation> ReadAll()
        {
            return this.DataSources.EsrLocationApi.ReadAll();
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
        public List<EsrLocation> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrLocationApi.ReadByEsrId(personId);
        }

        public List<EsrLocation> ReadSpecial(string reference)
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
        public List<EsrLocation> Update(List<EsrLocation> entities)
        {
            return this.DataSources.EsrLocationApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Delete(int entityId)
        {
            var addressResult = this.DataSources.AddressApi.ReadByEsrId(entityId);
            if (addressResult != null && addressResult.Count > 0)
            {
                this.DataSources.AddressApi.Delete(addressResult[0].AddressID);
            }

            return this.DataSources.EsrLocationApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Delete(long entityId)
        {
            var addressResult = this.DataSources.AddressApi.ReadByEsrId(entityId);
            if (addressResult != null && addressResult.Count > 0)
            {
                this.DataSources.AddressApi.Delete(addressResult[0].AddressID);
            }

            return this.DataSources.EsrLocationApi.Delete(entityId);
        }

        public EsrLocation Delete(EsrLocation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}