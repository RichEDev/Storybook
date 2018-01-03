using System;
using System.Collections.Generic;
using EsrGo2FromNhsWcfLibrary.Base;
using EsrGo2FromNhsWcfLibrary.Enum;
using EsrGo2FromNhsWcfLibrary.ESR;
using EsrGo2FromNhsWcfLibrary.Interfaces;

namespace EsrGo2FromNhsWcfLibrary.Crud
{
    /// <summary>
    /// A class to handle database interaction for <see cref="EmployeeLocation"/>
    /// </summary>
    public class EmployeeLocationCrud : EntityBase, IDataAccess<EmployeeLocation>
    {
        /// <summary>
        /// Create a new instance of <see cref="EmployeeLocationCrud"/>
        /// </summary>
        /// <param name="metabaseName">The name of the metabase to connect to (from config file)</param>
        /// <param name="accountId">The current Account Id</param>
        /// <param name="apiHandler">An instance of <see cref="IEsrApi"/></param>
        /// <param name="logger">An instance of <see cref="Log"/></param>
        public EmployeeLocationCrud(string metabaseName, int accountId, IEsrApi apiHandler = null, Log logger = null) : base(metabaseName, accountId, apiHandler, logger)
        {
        }

        /// <summary>
        /// Create <see cref="EmployeeLocation"/> records.
        /// Currently Not implemented
        /// </summary>
        /// <param name="entities">
        /// A <see cref="List{T}"/> of <seealso cref="EmployeeLocation"/>
        /// </param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<EmployeeLocation> Create(List<EmployeeLocation> entities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="entityId">the ID of the record to read.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public EmployeeLocation Read(int entityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="entityId">the ID of the record to read.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public EmployeeLocation Read(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// read all <see cref="EmployeeLocation"/> records for a specific account.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <seealso cref="EmployeeLocation"/>
        /// </returns>
        public List<EmployeeLocation> ReadAll()
        {
            return this.EsrApiHandler.Execute<EmployeeLocation>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// Read a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="esrId">the ID of the esr record to read.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<EmployeeLocation> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="reference">the special value of the record to read.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<EmployeeLocation> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="entities">a <see cref="List{T}"/> of <seealso cref="EmployeeLocation"/>.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<EmployeeLocation> Update(List<EmployeeLocation> entities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="entityId">the ID of the record to delete.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public EmployeeLocation Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a specific <see cref="EmployeeLocation"/> from the database
        /// Not currently implemented
        /// </summary>
        /// <param name="entity">An instance of <see cref="EmployeeLocation"/> to delete.</param>
        /// <returns><exception cref="NotImplementedException"></exception></returns>
        /// <exception cref="NotImplementedException"></exception>
        public EmployeeLocation Delete(EmployeeLocation entity)
        {
            throw new NotImplementedException();
        }
    }
}
