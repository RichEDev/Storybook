namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The vehicle journey rate crud.
    /// </summary>
    public class VehicleJourneyRateCrud : EntityBase, IDataAccess<VehicleJourneyRate>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VehicleJourneyRateCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public VehicleJourneyRateCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
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
        /// The <see cref="VehicleJourneyRate"/>.
        /// </returns>
        public List<VehicleJourneyRate> ReadAll()
        {
            return this.EsrApiHandler.Execute<VehicleJourneyRate>(DataAccessMethod.ReadAll);
        }

        public List<VehicleJourneyRate> ReadByEsrId(long esrId)
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