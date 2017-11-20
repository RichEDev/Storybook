namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;

    /// <summary>
    /// The ESR addresses crud.
    /// </summary>
    public class EsrAddressesCrud : EntityBase, IDataAccess<EsrAddress>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAddressesCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrAddressesCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> Create(List<EsrAddress> entities)
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
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrAddress>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrAddress>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrAddress>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="esrId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> ReadByEsrId(long esrId)
        {
            return this.EsrApiHandler.Execute<EsrAddress>(DataAccessMethod.ReadByEsrId, esrId.ToString(CultureInfo.InvariantCulture));
        }

        public List<EsrAddress> ReadSpecial(string reference)
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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAddress> Update(List<EsrAddress> entities)
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
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrAddress>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrAddress>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete by entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        public EsrAddress Delete(EsrAddress entity)
        {
            return this.EsrApiHandler.Execute<EsrAddress>(entity.KeyValue.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }
    }
}