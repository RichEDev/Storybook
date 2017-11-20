namespace ApiRpc.Classes.Crud
{
    using System;
    using System.Collections.Generic;
    using global::ApiCrud;
    using global::ApiRpc.Interfaces;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The employee role API.
    /// </summary>
    public class EmployeeRoleApi : IDataAccess<EmployeeRole>
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int accountId;

        /// <summary>
        /// The crud API.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeRoleApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public EmployeeRoleApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeRole> Create(List<EmployeeRole> entity)
        {
            return this.crudApi.CreateEmployeeRoles(this.metaBase, this.accountId.ToString(), entity);
        }

        public EmployeeRole Read(int employeeId)
        {
            return this.crudApi.ReadEmployeeRole(this.metaBase, this.accountId.ToString(), employeeId.ToString());
        }

        public EmployeeRole Read(long employeeId)
        {
            return this.crudApi.ReadEmployeeRole(this.metaBase, this.accountId.ToString(), employeeId.ToString());
        }

        public List<EmployeeRole> ReadAll()
        {
            throw new NotImplementedException();
        }

        public List<EmployeeRole> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
        }

        public List<EmployeeRole> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EmployeeRole> Update(List<EmployeeRole> entity)
        {
            return this.crudApi.CreateEmployeeRoles(this.metaBase, this.accountId.ToString(), entity);
        }

        public EmployeeRole Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public EmployeeRole Delete(EmployeeRole entity)
        {
            throw new NotImplementedException();
        }

        public EmployeeRole Delete(int entityId)
        {
            throw new NotImplementedException();
        }
    }
}