namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using global::ApiCrud;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiRpc.Interfaces;
    public class PositionApi : IDataAccess<EsrPosition>
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
        public PositionApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrPosition> Create(List<EsrPosition> entities)
        {
            return this.crudApi.CreateEsrPositions(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrPosition Read(int entityId)
        {
            return this.crudApi.ReadEsrPosition(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrPosition Read(long entityId)
        {
            return this.crudApi.ReadEsrPosition(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrPosition> ReadAll()
        {
            return this.crudApi.ReadAllEsrPosition(this.metaBase, this.accountId.ToString());
        }

        public List<EsrPosition> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrPosition> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrPosition> Update(List<EsrPosition> entities)
        {
            return this.crudApi.UpdateEsrPositions(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrPosition Delete(long entityId)
        {
            return this.crudApi.DeleteEsrPosition(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrPosition Delete(EsrPosition entity)
        {
            throw new System.NotImplementedException();
        }
    }
}