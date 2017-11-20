namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;

    public class UdfApi : IDataAccess<UserDefinedField>
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
        public UdfApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<UserDefinedField> Create(List<UserDefinedField> entities)
        {
            return this.crudApi.CreateUdfValue(this.metaBase, this.accountId.ToString(), entities);
        }

        public UserDefinedField Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedField Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedField> ReadAll()
        {
            return this.crudApi.ReadAllUdf(this.metaBase, this.accountId.ToString());
        }

        public List<UserDefinedField> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedField> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedField> Update(List<UserDefinedField> entities)
        {
            return this.crudApi.CreateUdfValue(this.metaBase, this.accountId.ToString(), entities);
        }

        public UserDefinedField Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedField Delete(UserDefinedField entity)
        {
            throw new System.NotImplementedException();
        }
    }
}