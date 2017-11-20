namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR assignment costing crud.
    /// </summary>
    public class EsrAssignmentCostingCrud : EntityBase, IDataAccess<EsrAssignmentCostings>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentCostingCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base URL.
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
        public EsrAssignmentCostingCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrAssignmentCostingsApi == null)
            {
                this.DataSources.EsrAssignmentCostingsApi = new AssignmentCostingsApi(metabase, accountid);
            }
        }

        public List<EsrAssignmentCostings> Create(List<EsrAssignmentCostings> entities)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignmentCostings Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignmentCostings Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> Update(List<EsrAssignmentCostings> entities)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignmentCostings Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentCostings"/>.
        /// </returns>
        public EsrAssignmentCostings Delete(EsrAssignmentCostings entity)
        {
            return this.DataSources.EsrAssignmentCostingsApi.Delete(entity);
        }
    }
}