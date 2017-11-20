namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class AssignmentCostingsApi : IDataAccess<EsrAssignmentCostings>
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
        public AssignmentCostingsApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrAssignmentCostings> Create(List<EsrAssignmentCostings> entities)
        {
            return this.crudApi.CreateEsrAssignmentCostingss(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrAssignmentCostings Read(int entityId)
        {
            return this.crudApi.ReadEsrAssignmentCostings(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrAssignmentCostings Read(long entityId)
        {
            return this.crudApi.ReadEsrAssignmentCostings(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrAssignmentCostings> ReadAll()
        {
            return this.crudApi.ReadAllEsrAssignmentCostings(this.metaBase, this.accountId.ToString());
        }

        public List<EsrAssignmentCostings> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadEsrAssignmentCostingsByPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        public List<EsrAssignmentCostings> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> Update(List<EsrAssignmentCostings> entities)
        {
            return this.crudApi.UpdateEsrAssignmentCostingss(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrAssignmentCostings Delete(long entityId)
        {
            return this.crudApi.DeleteEsrAssignmentCostings(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrAssignmentCostings Delete(EsrAssignmentCostings entity)
        {
            return this.crudApi.DeleteEsrAssignmentCostings(this.metaBase, this.accountId.ToString(), entity.KeyValue.ToString());
        }
    }
}