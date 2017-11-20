namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Interfaces;
    public class ImportLogApi : IDataAccess<ImportLog>
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
        public ImportLogApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<ImportLog> Create(List<ImportLog> entities)
        {
            return this.crudApi.CreateImportLogs(this.metaBase, this.accountId.ToString(), entities);
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

        public List<ImportLog> Update(List<ImportLog> entities)
        {
            return this.crudApi.UpdateImportLogs(this.metaBase, this.accountId.ToString(), entities);
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