namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using global::ApiCrud;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The address API.
    /// </summary>
    public class EsrAddressApi : IDataAccess<EsrAddress>
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int accountId;

        /// <summary>
        /// The crud API.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAddressApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public EsrAddressApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
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
        public List<EsrAddress> Create(List<EsrAddress> entities)
        {
            return this.crudApi.CreateEsrAddresss(this.metaBase, this.accountId.ToString(), entities);
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
            return this.crudApi.ReadEsrAddress(this.metaBase, this.accountId.ToString(), entityId.ToString());
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
            return this.crudApi.ReadEsrAddress(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrAddress> ReadAll()
        {
            return this.crudApi.ReadAllEsrAddress(this.metaBase, this.accountId.ToString());
        }

        /// <summary>
        /// The read by ESR id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrAddress> ReadByEsrId(long personId)
        {
            var result = this.crudApi.ReadEsrAddressByPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
            return result ?? new List<EsrAddress>();
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
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrAddress> Update(List<EsrAddress> entities)
        {
            return this.crudApi.UpdateEsrAddresss(this.metaBase, this.accountId.ToString(), entities);
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
            return this.crudApi.DeleteEsrAddress(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(EsrAddress entity)
        {
            return this.crudApi.DeleteEsrAddress(this.metaBase, this.accountId.ToString(), entity.KeyValue.ToString());
        }
    }
}