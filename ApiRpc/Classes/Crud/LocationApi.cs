namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class LocationApi : IDataAccess<EsrLocation>
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
        /// The crud API.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="AssignmentApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public LocationApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrLocation> Create(List<EsrLocation> entities)
        {
            return this.crudApi.CreateEsrLocations(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrLocation Read(int entityId)
        {
            return this.crudApi.ReadEsrLocation(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrLocation Read(long entityId)
        {
            return this.crudApi.ReadEsrLocation(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrLocation> ReadAll()
        {
            return this.crudApi.ReadAllEsrLocation(this.metaBase, this.accountId.ToString());
        }

        public List<EsrLocation> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrLocation> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrLocation> Update(List<EsrLocation> entities)
        {
            return this.crudApi.UpdateEsrLocations(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrLocation Delete(long entityId)
        {
            return this.crudApi.DeleteEsrLocation(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrLocation Delete(EsrLocation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}