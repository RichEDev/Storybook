namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;

    using Base;
    using Enum;
    using Interfaces;
    using Spend_Management;

    /// <summary>
    /// The vehicle journey rate crud.
    /// </summary>
    public class VehicleEngineTypeCrud : EntityBase, IDataAccess<VehicleEngineType>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VehicleEngineTypeCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public VehicleEngineTypeCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        public List<VehicleEngineType> Create(List<VehicleEngineType> entities)
        {
            throw new System.NotImplementedException();
        }

        public VehicleEngineType Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public VehicleEngineType Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<VehicleEngineType> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<VehicleEngineType> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<VehicleEngineType> ReadSpecial(string code)
        {
            return this.EsrApiHandler.Execute<VehicleEngineType>(DataAccessMethod.ReadSpecial, code);
        }

        public List<VehicleEngineType> Update(List<VehicleEngineType> entities)
        {
            throw new System.NotImplementedException();
        }

        public VehicleEngineType Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public VehicleEngineType Delete(VehicleEngineType entity)
        {
            throw new System.NotImplementedException();
        }
    }
}