namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.ESR;
    using ApiCrud;
    using Crud;
    using Interfaces;

    /// <summary>
    /// The ESR assignments crud helper class.
    /// </summary>
    public class EsrAssignmentLocationCrud : EntityBase, IDataAccess<EsrAssignmentLocation>
    {
        #region Public Methods

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentLocationCrud"/> class.
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
        public EsrAssignmentLocationCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrAssignmentLocationApi == null)
            {
                this.DataSources.EsrAssignmentLocationApi = new AssignmentLocationApi(metabase, accountid);
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
        public List<EsrAssignmentLocation> Create(List<EsrAssignmentLocation> entities)
        {
            return this.DataSources.EsrAssignmentLocationApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentLocation"/>.
        /// </returns>
        public EsrAssignmentLocation Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentLocation"/>.
        /// </returns>
        public EsrAssignmentLocation Read(long entityId)
        {
            throw new System.NotImplementedException();
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
        public List<EsrAssignmentLocation> ReadAll()
        {
            throw new System.NotImplementedException();
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
        public List<EsrAssignmentLocation> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentLocation> ReadSpecial(string reference)
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
        public List<EsrAssignmentLocation> Update(List<EsrAssignmentLocation> entities)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentLocation"/>.
        /// </returns>
        public EsrAssignmentLocation Delete(int entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentLocation"/>.
        /// </returns>
        public EsrAssignmentLocation Delete(long entityId)
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
        /// The <see cref="EsrAssignmentLocation"/>.
        /// </returns>
        public EsrAssignmentLocation Delete(EsrAssignmentLocation entity)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}