namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class AccountPropertyApi : IDataAccess<AccountProperty>
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
        /// Initialises a new instance of the <see cref="AssignmentApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public AccountPropertyApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<AccountProperty> Create(List<AccountProperty> entities)
        {
            throw new System.NotImplementedException();
        }

        public AccountProperty Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public AccountProperty Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<AccountProperty> ReadAll()
        {
            return this.crudApi.ReadAllAccountProperties(this.metaBase, this.accountId.ToString());
        }

        public List<AccountProperty> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<AccountProperty> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<AccountProperty> Update(List<AccountProperty> entities)
        {
            throw new System.NotImplementedException();
        }

        public AccountProperty Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public AccountProperty Delete(AccountProperty entity)
        {
            throw new System.NotImplementedException();
        }
    }
}