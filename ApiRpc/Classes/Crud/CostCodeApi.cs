namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class CostCodeApi : IDataAccess<CostCode>
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
        public CostCodeApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<CostCode> Create(List<CostCode> entities)
        {
            return this.crudApi.CreateCostCodes(this.metaBase, this.accountId.ToString(), entities);
        }

        public CostCode Read(int entityId)
        {
            return this.crudApi.ReadCostCode(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public CostCode Read(long entityId)
        {
            return this.crudApi.ReadCostCode(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<CostCode> ReadAll()
        {
            return this.crudApi.ReadAllCostCodes(this.metaBase, this.accountId.ToString());
        }

        public List<CostCode> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<CostCode> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<CostCode> Update(List<CostCode> entities)
        {
            return this.crudApi.UpdateCostCodes(this.metaBase, this.accountId.ToString(), entities);
        }

        public CostCode Delete(long entityId)
        {
            return this.crudApi.DeleteCostCodes(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public CostCode Delete(CostCode entity)
        {
            throw new System.NotImplementedException();
        }
    }
}