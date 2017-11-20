namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class AddressApi : IDataAccess<Address>
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
        /// Initialises a new instance of the <see cref="AddressApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public AddressApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<Address> Create(List<Address> entities)
        {
            return this.crudApi.CreateAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entities);
        }

        public Address Read(int entityId)
        {
            return this.crudApi.ReadAddress(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public Address Read(long entityId)
        {
            return this.crudApi.ReadAddress(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public List<Address> ReadAll()
        {
            return this.crudApi.ReadAllAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture));
        }

        public List<Address> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadAddressByEsr(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), personId.ToString(CultureInfo.InvariantCulture));
        }

        public List<Address> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<Address> Update(List<Address> entities)
        {
            return this.crudApi.UpdateAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entities);
        }

        public Address Delete(long entityId)
        {
            return this.crudApi.DeleteAddresses(this.metaBase, this.accountId.ToString(CultureInfo.InvariantCulture), entityId.ToString(CultureInfo.InvariantCulture));
        }

        public Address Delete(Address entity)
        {
            throw new System.NotImplementedException();
        }
    }
}