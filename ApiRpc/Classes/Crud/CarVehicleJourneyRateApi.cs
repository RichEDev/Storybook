namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The car vehicle journey rate API.
    /// </summary>
    public class CarVehicleJourneyRateApi : IDataAccess<CarVehicleJourneyRate>
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
        /// Initialises a new instance of the <see cref="CarVehicleJourneyRateApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public CarVehicleJourneyRateApi(string metaBase, int accountId)
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
        public List<CarVehicleJourneyRate> Create(List<CarVehicleJourneyRate> entities)
        {
            return this.crudApi.CreateCarVehicleJourneyRates(this.metaBase, this.accountId.ToString(), entities);
        }

        public CarVehicleJourneyRate Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public CarVehicleJourneyRate Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> ReadAll()
        {
            return this.crudApi.ReadAllCarVehicleJourneyRates(this.metaBase, this.accountId.ToString());
        }

        public List<CarVehicleJourneyRate> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Read all rates for the given car id.
        /// </summary>
        /// <param name="reference">
        /// car id to read.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarVehicleJourneyRate> ReadSpecial(string reference)
        {
            return this.crudApi.ReadCarVehicleJourneyRate(this.metaBase, this.accountId.ToString(), reference);
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
        public List<CarVehicleJourneyRate> Update(List<CarVehicleJourneyRate> entities)
        {
            return this.crudApi.UpdateCarVehicleJourneyRates(this.metaBase, this.accountId.ToString(), entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public CarVehicleJourneyRate Delete(long entityId)
        {
            return this.crudApi.DeleteCarVehicleJourneyRates(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="carVehicleJourneyRate">
        /// The car vehicle journey rate.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        public CarVehicleJourneyRate Delete(CarVehicleJourneyRate carVehicleJourneyRate)
        {
            return this.crudApi.DeleteCarVehicleJourneyRates(this.metaBase, this.accountId.ToString(), carVehicleJourneyRate);
        }
    }
}