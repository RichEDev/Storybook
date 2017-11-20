namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class EmployeeCostCodeApi : IDataAccess<EmployeeCostCode>
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
        public EmployeeCostCodeApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EmployeeCostCode> Create(List<EmployeeCostCode> entities)
        {
            return this.crudApi.CreateEmployeeCostCodes(this.metaBase, this.accountId.ToString(), entities);
        }

        public EmployeeCostCode Read(int entityId)
        {
            return this.crudApi.ReadEmployeeCostCode(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EmployeeCostCode Read(long entityId)
        {
            return this.crudApi.ReadEmployeeCostCode(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EmployeeCostCode> ReadAll()
        {
            return this.crudApi.ReadAllEmployeeCostCodes(this.metaBase, this.accountId.ToString());
        }

        public List<EmployeeCostCode> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeCostCode> ReadSpecial(string reference)
        {
            var splitRef = reference.Split('/');
            return this.crudApi.ReadEmployeeCostCodesByEmployee(this.metaBase, this.accountId.ToString(), splitRef[1]);
        }

        public List<EmployeeCostCode> Update(List<EmployeeCostCode> entities)
        {
            return this.crudApi.UpdateEmployeeCostCodes(this.metaBase, this.accountId.ToString(), entities);
        }

        public EmployeeCostCode Delete(long entityId)
        {
            return this.crudApi.DeleteEmployeeCostCodes(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EmployeeCostCode Delete(EmployeeCostCode entity)
        {
            throw new System.NotImplementedException();
        }
    }
}