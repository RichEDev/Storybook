namespace ApiCrud.DataAccess
{
    using System;
    using System.Collections.Generic;

    using ApiCrud.DataClasses;
    using ApiCrud.Interfaces;

    /// <summary>
    /// The ESR car data access.
    /// </summary>
    public class EsrCarDataAccess : DataAccess, IDataAccess<EsrCar>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrCarDataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="dataConnection">
        /// The data connection.
        /// </param>
        protected EsrCarDataAccess(string baseUrl, IApiDbConnection dataConnection)
            : base(baseUrl, dataConnection)
        {
        }

        public EsrCar Create(EsrCar entity)
        {
            throw new NotImplementedException();
        }

        public EsrCar Read(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<EsrCar> ReadAll(DataAccessFilter filter)
        {
            throw new NotImplementedException();
        }

        public EsrCar Update(EsrCar entity)
        {
            throw new NotImplementedException();
        }

        public EsrCar Delete(int entityId)
        {
            throw new NotImplementedException();
        }
    }
}