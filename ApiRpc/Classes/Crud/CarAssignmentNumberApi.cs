namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class CarAssignmentNumberApi : IDataAccess<CarAssignmentNumberAllocation>
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
        public CarAssignmentNumberApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<CarAssignmentNumberAllocation> Create(List<CarAssignmentNumberAllocation> entities)
        {
            return this.crudApi.CreateCarAssignmentNumberAllocations(this.metaBase, this.accountId.ToString(), entities);
        }

        public CarAssignmentNumberAllocation Read(int entityId)
        {
            return this.crudApi.ReadCarAssignmentNumberAllocation(
                this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public CarAssignmentNumberAllocation Read(long entityId)
        {
            return this.crudApi.ReadCarAssignmentNumberAllocation(
                this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<CarAssignmentNumberAllocation> ReadAll()
        {
            return this.crudApi.ReadAllCarAssignmentNumberAllocations(this.metaBase, this.accountId.ToString());
        }

        public List<CarAssignmentNumberAllocation> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadCarAssignmentNumberAllocationsByEmployee(
                this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        public List<CarAssignmentNumberAllocation> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<CarAssignmentNumberAllocation> Update(List<CarAssignmentNumberAllocation> entities)
        {
            return this.crudApi.UpdateCarAssignmentNumberAllocations(this.metaBase, this.accountId.ToString(), entities);
        }

        public CarAssignmentNumberAllocation Delete(long entityId)
        {
            return this.crudApi.DeleteCarAssignmentNumberAllocations(
                this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public CarAssignmentNumberAllocation Delete(CarAssignmentNumberAllocation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}