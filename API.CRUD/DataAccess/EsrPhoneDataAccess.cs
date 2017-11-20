namespace ApiCrud.DataAccess
{
    using System.Collections.Generic;

    using ApiCrud.DataClasses;
    using ApiCrud.Interfaces;

    /// <summary>
    /// The ESR phone data access.
    /// </summary>
    public class EsrPhoneDataAccess : DataAccess, IDataAccess<EsrCar>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPhoneDataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="dataConnection">
        /// The data connection.
        /// </param>
        protected EsrPhoneDataAccess(string baseUrl, IApiDbConnection dataConnection)
            : base(baseUrl, dataConnection)
        {
        }

        public EsrCar Create(EsrCar entity)
        {
            throw new System.NotImplementedException();
        }

        public EsrCar Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrCar> ReadAll(DataAccessFilter filter)
        {
            throw new System.NotImplementedException();
        }

        public EsrCar Update(EsrCar entity)
        {
            throw new System.NotImplementedException();
        }

        public EsrCar Delete(int entityId)
        {
            throw new System.NotImplementedException();
        }
    }
}