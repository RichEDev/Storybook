namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;

    /// <summary>
    /// The ESR assignments crud helper class.
    /// </summary>
    public class EsrAssignmentLocationCrud : EntityBase, IDataAccess<EsrAssignmentLocation>
    {
        #region Public Methods

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentsCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrAssignmentLocationCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
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
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
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
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrAssignmentLocation> ReadByEsrId(long esrId)
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
        /// The <see><cref>List</cref></see>.
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
