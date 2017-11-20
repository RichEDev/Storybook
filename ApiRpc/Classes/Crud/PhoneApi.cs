namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;

    public class PhoneApi : IDataAccess<EsrPhone>
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
        /// Initialises a new instance of the <see cref="AssignmentApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public PhoneApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrPhone> Create(List<EsrPhone> entities)
        {
            return this.crudApi.CreateEsrPhones(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrPhone Read(int entityId)
        {
            return this.crudApi.ReadEsrPhone(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrPhone Read(long entityId)
        {
            return this.crudApi.ReadEsrPhone(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrPhone> ReadAll()
        {
            return this.crudApi.ReadAllEsrPhone(this.metaBase, this.accountId.ToString());
        }

        public List<EsrPhone> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadEsrPhoneByPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        public List<EsrPhone> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrPhone> Update(List<EsrPhone> entities)
        {
            return this.crudApi.UpdateEsrPhones(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrPhone Delete(long entityId)
        {
            return this.crudApi.DeleteEsrPhone(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        public EsrPhone Delete(EsrPhone entity)
        {
            return this.crudApi.DeleteEsrPhone(this.metaBase, this.accountId.ToString(), entity.KeyValue.ToString());
        }
    }
}