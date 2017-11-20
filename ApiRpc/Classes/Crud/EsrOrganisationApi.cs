namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class EsrOrganisationApi : IDataAccess<EsrOrganisation>
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
        public EsrOrganisationApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrOrganisation> Create(List<EsrOrganisation> entities)
        {
            return this.crudApi.CreateEsrOrganisations(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrOrganisation Read(int entityId)
        {
            return this.crudApi.ReadEsrOrganisation(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrOrganisation Read(long entityId)
        {
            return this.crudApi.ReadEsrOrganisation(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrOrganisation> ReadAll()
        {
            return this.crudApi.ReadAllEsrOrganisation(this.metaBase, this.accountId.ToString());
        }

        public List<EsrOrganisation> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrOrganisation> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrOrganisation> Update(List<EsrOrganisation> entities)
        {
            return this.crudApi.UpdateEsrOrganisations(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrOrganisation Delete(long entityId)
        {
            return this.crudApi.DeleteEsrOrganisation(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrOrganisation Delete(EsrOrganisation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}