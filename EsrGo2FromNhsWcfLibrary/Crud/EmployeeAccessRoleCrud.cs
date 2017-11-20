namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The employee access role crud.
    /// </summary>
    public class EmployeeAccessRoleCrud : EntityBase, IDataAccess<EmployeeAccessRole>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeAccessRoleCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EmployeeAccessRoleCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeAccessRole"/>.
        /// </returns>
        public List<EmployeeAccessRole> Create(List<EmployeeAccessRole> entity)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entity);
        }

        public EmployeeAccessRole Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public EmployeeAccessRole Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        List<EmployeeAccessRole> IDataAccess<EmployeeAccessRole>.ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> Update(List<EmployeeAccessRole> entity)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entity);
        }

        public EmployeeAccessRole Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public EmployeeAccessRole Delete(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public EmployeeAccessRole Delete(EmployeeAccessRole entity)
        {
            throw new System.NotImplementedException();
        }
    }
}