namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The vehicle journey rate API.
    /// </summary>
    public class VehicleJourneyRateApi : IDataAccess<VehicleJourneyRate>
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
        /// Initialises a new instance of the <see cref="VehicleJourneyRateApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public VehicleJourneyRateApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<VehicleJourneyRate> Create(List<VehicleJourneyRate> entities)
        {
            throw new System.NotImplementedException();
        }

        VehicleJourneyRate IDataAccess<VehicleJourneyRate>.Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        VehicleJourneyRate IDataAccess<VehicleJourneyRate>.Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<VehicleJourneyRate> IDataAccess<VehicleJourneyRate>.ReadAll()
        {
            return this.crudApi.ReadAllVehicleJourneyRates(this.metaBase, this.accountId.ToString());
        }

        List<VehicleJourneyRate> IDataAccess<VehicleJourneyRate>.ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        List<VehicleJourneyRate> IDataAccess<VehicleJourneyRate>.ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<VehicleJourneyRate> Update(List<VehicleJourneyRate> entities)
        {
            throw new System.NotImplementedException();
        }

        VehicleJourneyRate IDataAccess<VehicleJourneyRate>.Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public VehicleJourneyRate Delete(VehicleJourneyRate entity)
        {
            throw new System.NotImplementedException();
        }
    }
}