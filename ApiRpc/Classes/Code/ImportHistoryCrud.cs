namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The import history CRUD.
    /// </summary>
    public class ImportHistoryCrud : EntityBase, IDataAccess<ImportHistory>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ImportHistoryCrud"/> class.
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
        public ImportHistoryCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.ImportHistoryApi == null)
            {
                this.DataSources.ImportHistoryApi = new ImportHistoryApi(metabase, accountid);
            }
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
        public List<ImportHistory> Create(List<ImportHistory> entities)
        {
            return this.DataSources.ImportHistoryApi.Create(entities);
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

        public List<ImportHistory> ReadByEsrId(long personId)
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
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportHistory> Update(List<ImportHistory> entities)
        {
            return this.DataSources.ImportHistoryApi.Update(entities);
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