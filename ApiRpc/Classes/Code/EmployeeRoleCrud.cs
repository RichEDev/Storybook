namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The employee role crud.
    /// </summary>
    public class EmployeeRoleCrud : EntityBase, IDataAccess<EmployeeRole>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeRoleCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API collection.
        /// </param>
        public EmployeeRoleCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EmployeeRoleApi == null)
            {
                this.DataSources.EmployeeRoleApi = new ApiCrudBase<EmployeeRole>(baseUrl, metabase, accountid);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeRole> Create(List<EmployeeRole> entities)
        {
            return this.DataSources.EmployeeRoleApi.Create(entities);
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

        public List<EmployeeRole> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeRole> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeRole> Update(List<EmployeeRole> entities)
        {
            return this.DataSources.EmployeeRoleApi.Update(entities);
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