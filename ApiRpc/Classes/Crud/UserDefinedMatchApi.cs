namespace ApiRpc.Classes.Crud
{
    using System;
    using System.Collections.Generic;

    using global::ApiCrud;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The user defined match API.
    /// </summary>
    public class UserDefinedMatchApi : IDataAccess<UserDefinedMatchField>
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
        /// Initialises a new instance of the <see cref="UserDefinedMatchApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public UserDefinedMatchApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<UserDefinedMatchField> Create(List<UserDefinedMatchField> entities)
        {
            throw new NotImplementedException();
        }

        public UserDefinedMatchField Read(int entityId)
        {
            throw new NotImplementedException();
        }

        public UserDefinedMatchField Read(long entityId)
        {
            throw new NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadAll()
        {
            throw new NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
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
        public List<UserDefinedMatchField> ReadSpecial(string reference)
        {
            return this.crudApi.ReadUserDefinedMatchField(this.metaBase, this.accountId.ToString(), reference);
        }

        public List<UserDefinedMatchField> Update(List<UserDefinedMatchField> entities)
        {
            throw new NotImplementedException();
        }

        public UserDefinedMatchField Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public UserDefinedMatchField Delete(UserDefinedMatchField entity)
        {
            throw new NotImplementedException();
        }
    }
}