namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The ESR location crud.
    /// </summary>
    public class EsrLocationCrud : EntityBase, IDataAccess<EsrLocation>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrLocationCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        public EsrLocationCrud(string metabase, int accountId, IEsrApi esrApiHandler = null)
            : base(metabase, accountId, esrApiHandler)
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
        public List<EsrLocation> Create(List<EsrLocation> entities)
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
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrLocation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrLocation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
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
        public List<EsrLocation> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrLocation>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrLocation> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrLocation> ReadSpecial(string reference)
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
        public List<EsrLocation> Update(List<EsrLocation> entities)
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
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Delete(int entityId)
        {
            var addressResult = this.EsrApiHandler.Execute<Address>(DataAccessMethod.ReadByEsrId, entityId.ToString(CultureInfo.InvariantCulture));
            if (addressResult != null && addressResult.Count > 0)
            {
                this.EsrApiHandler.Execute<Address>(addressResult[0].AddressID.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
            }

            return this.EsrApiHandler.Execute<EsrLocation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        public EsrLocation Delete(long entityId)
        {
            var addressResult = this.EsrApiHandler.Execute<Address>(DataAccessMethod.ReadByEsrId, entityId.ToString(CultureInfo.InvariantCulture));
            if (addressResult != null && addressResult.Count > 0)
            {
                this.EsrApiHandler.Execute<Address>(addressResult[0].AddressID.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
            }

            return this.EsrApiHandler.Execute<EsrLocation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        public EsrLocation Delete(EsrLocation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}