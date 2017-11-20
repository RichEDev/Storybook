namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The import history CRUD.
    /// </summary>
    public class ImportHistoryCrud : EntityBase, IDataAccess<ImportHistory>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ImportHistoryCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public ImportHistoryCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see cref="ImportHistory"/>.
        /// </returns>
        public List<ImportHistory> Create(List<ImportHistory> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        public ImportHistory Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public ImportHistory Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<ImportHistory> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<ImportHistory> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<ImportHistory> ReadSpecial(string reference)
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
        /// The <see cref="ImportHistory"/>.
        /// </returns>
        public List<ImportHistory> Update(List<ImportHistory> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        public ImportHistory Delete(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public ImportHistory Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public ImportHistory Delete(ImportHistory entity)
        {
            throw new System.NotImplementedException();
        }
    }
}