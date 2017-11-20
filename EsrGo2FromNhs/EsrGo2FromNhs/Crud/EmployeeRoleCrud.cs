namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The employee role crud.
    /// </summary>
    public class EmployeeRoleCrud : EntityBase, IDataAccess<EmployeeRole>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeRoleCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EmployeeRoleCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see cref="EmployeeRole"/>.
        /// </returns>
        public List<EmployeeRole> Create(List<EmployeeRole> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        public EmployeeRole Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public EmployeeRole Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeRole> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeRole> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeRole> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeRole> Update(List<EmployeeRole> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        public EmployeeRole Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public EmployeeRole Delete(EmployeeRole entity)
        {
            throw new System.NotImplementedException();
        }
    }
}