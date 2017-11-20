namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Interfaces;
    public class ImportHistoryApi : IDataAccess<ImportHistory>
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int accountId;

        /// <summary>
        /// The crud api.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeApi"/> class.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public ImportHistoryApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<ImportHistory> Create(List<ImportHistory> entities)
        {
            return this.crudApi.CreateImportHistorys(this.metaBase, this.accountId.ToString(), entities);
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

        public List<ImportHistory> Update(List<ImportHistory> entities)
        {
            return this.crudApi.UpdateImportHistorys(this.metaBase, this.accountId.ToString(), entities);
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