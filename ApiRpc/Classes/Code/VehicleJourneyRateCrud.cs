namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The vehicle journey rate crud.
    /// </summary>
    public class VehicleJourneyRateCrud : EntityBase, IDataAccess<VehicleJourneyRate>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VehicleJourneyRateCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API collection.
        /// </param>
        public VehicleJourneyRateCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.VehicleJourneyRateApi == null)
            {
                this.DataSources.VehicleJourneyRateApi = new VehicleJourneyRateApi(metabase, accountid);
            }
        }

        public List<VehicleJourneyRate> Create(List<VehicleJourneyRate> entities)
        {
            throw new System.NotImplementedException();
        }

        public VehicleJourneyRate Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public VehicleJourneyRate Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<VehicleJourneyRate> ReadAll()
        {
            return this.DataSources.VehicleJourneyRateApi.ReadAll();
        }

        public List<VehicleJourneyRate> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<VehicleJourneyRate> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<VehicleJourneyRate> Update(List<VehicleJourneyRate> entities)
        {
            throw new System.NotImplementedException();
        }

        public VehicleJourneyRate Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public VehicleJourneyRate Delete(VehicleJourneyRate entity)
        {
            throw new System.NotImplementedException();
        }
    }
}