namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The import log CRUD.
    /// </summary>
    public class ImportLogCrud : EntityBase, IDataAccess<ImportLog>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ImportLogCrud"/> class.
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
        public ImportLogCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.ImportLogApi == null)
            {
                this.DataSources.ImportLogApi = new ImportLogApi(metabase, accountid);
            }
        }

        /// <summary>
        /// Create import Log.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportLog> Create(List<ImportLog> entities)
        {
            return this.DataSources.ImportLogApi.Create(entities);
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

        public List<ImportLog> ReadByEsrId(long personId)
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
        /// The <see cref="List"/>.
        /// </returns>
        public List<ImportLog> Update(List<ImportLog> entities)
        {
            return this.DataSources.ImportLogApi.Update(entities);
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