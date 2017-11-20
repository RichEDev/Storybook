namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.ESR;
    using global::ApiCrud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The person API.
    /// </summary>
    public class PersonApi : IDataAccess<EsrPerson>
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
        /// Initialises a new instance of the <see cref="PersonApi"/> class.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public PersonApi(string metaBase, int accountId)
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
        public List<EsrPerson> Create(List<EsrPerson> entities)
        {
            return this.crudApi.CreateEsrPersons(this.metaBase, this.accountId.ToString(), entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPerson Read(int entityId)
        {
            return this.crudApi.ReadEsrPerson(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPerson Read(long entityId)
        {
            return this.crudApi.ReadEsrPerson(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrPerson> ReadAll()
        {
            return this.crudApi.ReadAllEsrPerson(this.metaBase, this.accountId.ToString());
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
        public List<EsrPerson> ReadByEsrId(long personId)
        {
            var result = this.crudApi.ReadEsrPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
            return result != null ? new List<EsrPerson> { result } : new List<EsrPerson>();
        }

        /// <summary>
        /// The read special.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrPerson> ReadSpecial(string reference)
        {
            return new List<EsrPerson> { this.crudApi.ReadEsrPerson(this.metaBase, this.accountId.ToString(), reference) };
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
        public List<EsrPerson> Update(List<EsrPerson> entities)
        {
            return this.crudApi.UpdateEsrPersons(this.metaBase, this.accountId.ToString(), entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPerson Delete(long entityId)
        {
            return this.crudApi.DeleteEsrPerson(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrPerson Delete(EsrPerson entity)
        {
            throw new System.NotImplementedException();
        }
    }
}