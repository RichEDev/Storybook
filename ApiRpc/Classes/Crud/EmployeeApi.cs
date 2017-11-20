namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The employee API.
    /// </summary>
    public class EmployeeApi : IDataAccess<Employee>
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
        /// The crud api.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeApi"/> class.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public EmployeeApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
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
        public List<Employee> Create(List<Employee> entities)
        {
            return this.crudApi.CreateEmployees(this.metaBase, this.accountId.ToString(), entities);
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
            return this.crudApi.ReadEmployee(this.metaBase, this.accountId.ToString(), entityId.ToString());
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
            return this.crudApi.ReadEmployee(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Employee> ReadAll()
        {
            return this.crudApi.ReadAllEmployees(this.metaBase, this.accountId.ToString());
        }

        /// <summary>
        /// The read by ESR id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Employee> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadEmployeeByPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        /// <summary>
        /// The read special.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Employee> ReadSpecial(string reference)
        {
            var splitRef = reference.Split('/');
            var result = this.crudApi.ReadEmployeeByUsername(this.metaBase, this.accountId.ToString(), splitRef[1]);
            return result != null ? new List<Employee> { result } : new List<Employee>();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Employee> Update(List<Employee> entities)
        {
            return this.crudApi.UpdateEmployees(this.metaBase, this.accountId.ToString(), entities);
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
            return this.crudApi.DeleteEmployees(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public Employee Delete(Employee entity)
        {
            throw new System.NotImplementedException();
        }
    }
}