namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.ESR;
    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR assignments crud helper class.
    /// </summary>
    public class EsrAssignmentsCrud : EntityBase, IDataAccess<EsrAssignment>
    {
        #region Public Methods

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentsCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API Collection.
        /// </param>
        public EsrAssignmentsCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrAssignmentApi == null)
            {
                this.DataSources.EsrAssignmentApi = new AssignmentApi(metabase, accountid);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> Create(List<EsrAssignment> entities)
        {
            return this.DataSources.EsrAssignmentApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Read(int entityId)
        {
            return this.DataSources.EsrAssignmentApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Read(long entityId)
        {
            return this.DataSources.EsrAssignmentApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> ReadAll()
        {
            return this.DataSources.EsrAssignmentApi.ReadAll();
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrAssignmentApi.ReadByEsrId(personId);
        }

        public List<EsrAssignment> ReadSpecial(string reference)
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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignment> Update(List<EsrAssignment> entities)
        {
            return this.DataSources.EsrAssignmentApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Delete(int entityId)
        {
            return this.DataSources.EsrAssignmentApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Delete(long entityId)
        {
            return this.DataSources.EsrAssignmentApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Delete(EsrAssignment entity)
        {
            return this.DataSources.EsrAssignmentApi.Delete(entity);
        }

        #endregion
    }
}