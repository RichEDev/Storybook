namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The employee access role API.
    /// </summary>
    public class EmployeeAccessRoleApi : IDataAccess<EmployeeAccessRole>
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
        /// Initialises a new instance of the <see cref="EmployeeAccessRoleApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public EmployeeAccessRoleApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EmployeeAccessRole> Create(List<EmployeeAccessRole> entities)
        {
            return this.crudApi.CreateEmployeeAccessRoles(this.metaBase, this.accountId.ToString(), entities);
        }

        public EmployeeAccessRole Read(int employeeId)
        {
            return this.crudApi.ReadEmployeeAccessRole(this.metaBase, this.accountId.ToString(), employeeId.ToString());
        }

        public EmployeeAccessRole Read(long employeeId)
        {
            return this.crudApi.ReadEmployeeAccessRole(this.metaBase, this.accountId.ToString(), employeeId.ToString());
        }

        public List<EmployeeAccessRole> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeAccessRole> Update(List<EmployeeAccessRole> entities)
        {
            return this.crudApi.CreateEmployeeAccessRoles(this.metaBase, this.accountId.ToString(), entities);
        }

        public EmployeeAccessRole Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public EmployeeAccessRole Delete(EmployeeAccessRole entity)
        {
            throw new System.NotImplementedException();
        }
    }
}