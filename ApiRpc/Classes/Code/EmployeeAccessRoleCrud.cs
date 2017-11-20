namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud.Interfaces;

    using global::ApiRpc.Classes.ApiCrud;

    /// <summary>
    /// The employee access role crud.
    /// </summary>
    public class EmployeeAccessRoleCrud : EntityBase, IDataAccess<EmployeeAccessRole>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeAccessRoleCrud"/> class.
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
        /// The api collection.
        /// </param>
        public EmployeeAccessRoleCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EmployeeAccessRoleApi == null)
            {
                this.DataSources.EmployeeAccessRoleApi = new ApiCrudBase<EmployeeAccessRole>(baseUrl, metabase, accountid);
            }
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
        public List<EmployeeAccessRole> Create(List<EmployeeAccessRole> entity)
        {
            return this.DataSources.EmployeeAccessRoleApi.Create(entity);
        }

        public EmployeeAccessRole Read(int entityId)
        {
            throw new System.NotImplementedException();
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

        public List<EmployeeAccessRole> Update(List<EmployeeAccessRole> entity)
        {
            return this.DataSources.EmployeeAccessRoleApi.Update(entity);
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