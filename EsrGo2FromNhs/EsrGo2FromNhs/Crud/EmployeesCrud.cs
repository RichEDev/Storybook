namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The employees crud.
    /// </summary>
    public class EmployeesCrud : EntityBase, IDataAccess<Employee>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeesCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EmployeesCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        public List<Employee> Create(List<Employee> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, null, entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Read(int entityId)
        {
            return this.EsrApiHandler.Execute<Employee>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Read(long entityId)
        {
            return this.EsrApiHandler.Execute<Employee>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
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
        public List<Employee> ReadAll()
        {
            return this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="esrId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Employee> ReadByEsrId(long esrId)
        {
            return this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, esrId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The read special.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public List<Employee> ReadSpecial(string reference)
        {
            string username = reference.Split('/')[1];
            var result = this.EsrApiHandler.Execute<Employee>(username, DataAccessMethod.ReadSpecial);
            return result != null ? new List<Employee> { result } : new List<Employee>();
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
        public List<Employee> Update(List<Employee> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<Employee>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<Employee>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
        }

        public Employee Delete(Employee entity)
        {
            throw new System.NotImplementedException();
        }
    }
}