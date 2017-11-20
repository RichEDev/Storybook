namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The import log CRUD.
    /// </summary>
    public class ImportLogCrud : EntityBase, IDataAccess<ImportLog>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ImportLogCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public ImportLogCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// Create import Log.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="ImportLog"/>.
        /// </returns>
        public List<ImportLog> Create(List<ImportLog> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        public ImportLog Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public ImportLog Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<ImportLog> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<ImportLog> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<ImportLog> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Update Import Log.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="ImportLog"/>.
        /// </returns>
        public List<ImportLog> Update(List<ImportLog> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        public ImportLog Delete(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public ImportLog Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public ImportLog Delete(ImportLog entity)
        {
            throw new System.NotImplementedException();
        }
    }
}