
namespace ApiCrud.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using ApiCrud.DataClasses;
    using ApiCrud.Interfaces;

    /// <summary>
    /// The ESR person data access.
    /// </summary>
    public class EsrPersonDataAccess : DataAccess, IDataAccess<EsrPerson>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPersonDataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="dataConnection">
        /// The data connection.
        /// </param>
        public EsrPersonDataAccess(string baseUrl, IApiDbConnection dataConnection)
            : base(baseUrl, dataConnection)
        {
            this.BaseUrl = baseUrl;
            this.expdata = dataConnection;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPersonDataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        public EsrPerson Create(EsrPerson entity)
        {
            throw new NotImplementedException();
        }

        public EsrPerson Read(int entityId)
        {
            return new EsrPerson
            {
                ActionResult = ApiResult.Success,
                Uri = this.FormatUrl(),
                esrFirstName = "esr first name"
            };
        }

        public List<EsrPerson> ReadAll(DataAccessFilter filter)
        {
            throw new NotImplementedException();
        }

        public EsrPerson Update(EsrPerson entity)
        {
            throw new NotImplementedException();
        }

        public EsrPerson Delete(int entityId)
        {
            throw new NotImplementedException();
        }
    }
}