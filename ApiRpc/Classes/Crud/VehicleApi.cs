namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;

    public class VehicleApi : IDataAccess<EsrVehicle>
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
        public VehicleApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrVehicle> Create(List<EsrVehicle> entities)
        {
            return this.crudApi.CreateEsrVehicles(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrVehicle Read(int entityId)
        {
            return this.crudApi.ReadEsrVehicle(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrVehicle Read(long entityId)
        {
            return this.crudApi.ReadEsrVehicle(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrVehicle> ReadAll()
        {
            return this.crudApi.ReadAllEsrVehicle(this.metaBase, this.accountId.ToString());
        }

        public List<EsrVehicle> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadEsrVehicleByPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        public List<EsrVehicle> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrVehicle> Update(List<EsrVehicle> entities)
        {
            return this.crudApi.UpdateEsrVehicles(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrVehicle Delete(long entityId)
        {
            return this.crudApi.DeleteEsrVehicle(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrVehicle Delete(EsrVehicle entity)
        {
            return this.crudApi.DeleteEsrVehicle(this.metaBase, this.accountId.ToString(), entity.ESRVehicleAllocationId.ToString());
        }
    }
}