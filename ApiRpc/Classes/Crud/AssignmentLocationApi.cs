namespace ApiRpc.Classes.Crud
{
    using System;
    using System.Collections.Generic;

    using global::ApiCrud;
    using ApiLibrary.DataObjects.ESR;

    using Interfaces;
    public class AssignmentLocationApi : IDataAccess<EsrAssignmentLocation>
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
        public AssignmentLocationApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EsrAssignmentLocation> Create(List<EsrAssignmentLocation> entities)
        {
            return this.crudApi.CreateAssignmentLocations(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrAssignmentLocation Read(int entityId)
        {
            throw new NotImplementedException();
        }

        public EsrAssignmentLocation Read(long entityId)
        {
            throw new NotImplementedException();
        }

        public List<EsrAssignmentLocation> ReadAll()
        {
            throw new NotImplementedException();
        }

        public List<EsrAssignmentLocation> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
        }

        public List<EsrAssignmentLocation> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        public List<EsrAssignmentLocation> Update(List<EsrAssignmentLocation> entities)
        {
            throw new NotImplementedException();
        }

        public EsrAssignmentLocation Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public EsrAssignmentLocation Delete(EsrAssignmentLocation entity)
        {
            throw new NotImplementedException();
        }
    }
}