namespace ApiCrud.DataAccess
{
    using System.Collections.Generic;

    using ApiCrud.DataClasses;
    using ApiCrud.Interfaces;

    /// <summary>
    /// The ESR assignment data access.
    /// </summary>
    public class EsrAssignmentDataAccess : DataAccess, IDataAccess<EsrAssignment>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentDataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="dataConnection">
        /// The data connection.
        /// </param>
        protected EsrAssignmentDataAccess(string baseUrl, IApiDbConnection dataConnection)
            : base(baseUrl, dataConnection)
        {
        }

        /// <summary>
        /// Create ESR Assignment.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Create(EsrAssignment entity)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignment Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignment> ReadAll(DataAccessFilter filter)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignment Update(EsrAssignment entity)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignment Delete(int entityId)
        {
            throw new System.NotImplementedException();
        }
    }
}