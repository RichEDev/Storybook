namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class EmployeeWorkAddressApi : IDataAccess<EmployeeWorkAddress>
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
        private readonly ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeWorkAddressApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public EmployeeWorkAddressApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EmployeeWorkAddress> Create(List<EmployeeWorkAddress> entities)
        {
            return this.crudApi.CreateEmployeeWorkAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entities);
        }

        public EmployeeWorkAddress Read(int entityId)
        {
            return this.crudApi.ReadEmployeeWorkAddress(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public EmployeeWorkAddress Read(long entityId)
        {
            return this.crudApi.ReadEmployeeWorkAddress(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public List<EmployeeWorkAddress> ReadAll()
        {
            return this.crudApi.ReadAllEmployeeWorkAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture));
        }

        public List<EmployeeWorkAddress> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeWorkAddress> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeWorkAddress> Update(List<EmployeeWorkAddress> entities)
        {
            return this.crudApi.UpdateEmployeeWorkAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entities);
        }

        public EmployeeWorkAddress Delete(long entityId)
        {
            return this.crudApi.DeleteEmployeeWorkAddresses(
                this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public EmployeeWorkAddress Delete(EmployeeWorkAddress entity)
        {
            throw new System.NotImplementedException();
        }
    }
}