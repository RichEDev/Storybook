namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;

    /// <summary>
    /// The ESR vehicles crud helper class.
    /// </summary>
    public class EsrVehiclesCrud : EntityBase, IDataAccess<EsrVehicle>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrVehiclesCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrVehiclesCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public List<EsrVehicle> Create(List<EsrVehicle> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public List<EsrVehicle> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public List<EsrVehicle> ReadByEsrId(long esrId)
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(DataAccessMethod.ReadByEsrId, esrId.ToString(CultureInfo.InvariantCulture));
        }

        public List<EsrVehicle> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public List<EsrVehicle> Update(List<EsrVehicle> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
        }

        public EsrVehicle Delete(EsrVehicle entity)
        {
            return this.EsrApiHandler.Execute<EsrVehicle>(entity.ESRVehicleAllocationId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
        }
    }
}