namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class EmployeeHomeAddressApi : IDataAccess<EmployeeHomeAddress>
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
        /// Initialises a new instance of the <see cref="EmployeeHomeAddressApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public EmployeeHomeAddressApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<EmployeeHomeAddress> Create(List<EmployeeHomeAddress> entities)
        {
            return this.crudApi.CreateEmployeeHomeAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entities);
        }

        public EmployeeHomeAddress Read(int entityId)
        {
            return this.crudApi.ReadEmployeeHomeAddress(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public EmployeeHomeAddress Read(long entityId)
        {
            return this.crudApi.ReadEmployeeHomeAddress(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public List<EmployeeHomeAddress> ReadAll()
        {
            return this.crudApi.ReadAllEmployeeHomeAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture));
        }

        public List<EmployeeHomeAddress> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeHomeAddress> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EmployeeHomeAddress> Update(List<EmployeeHomeAddress> entities)
        {
            return this.crudApi.UpdateEmployeeHomeAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entities);
        }

        public EmployeeHomeAddress Delete(long entityId)
        {
            return this.crudApi.DeleteEmployeeHomeAddresses(
                this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public EmployeeHomeAddress Delete(EmployeeHomeAddress entity)
        {
            throw new System.NotImplementedException();
        }
    }
}