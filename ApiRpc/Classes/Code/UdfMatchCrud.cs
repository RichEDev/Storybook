namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The UDF MATCH crud.
    /// </summary>
    public class UdfMatchCrud : EntityBase, IDataAccess<UserDefinedMatchField>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UdfMatchCrud"/> class.
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
        /// The API collection.
        /// </param>
        public UdfMatchCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.UserDefinedMatchApi == null)
            {
                this.DataSources.UserDefinedMatchApi = new UserDefinedMatchApi(metabase, accountid);
            }
        }

        public List<UserDefinedMatchField> Create(List<UserDefinedMatchField> entities)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadSpecial(string reference)
        {
            return this.DataSources.UserDefinedMatchApi.ReadSpecial(reference);
        }

        public List<UserDefinedMatchField> Update(List<UserDefinedMatchField> entities)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Delete(UserDefinedMatchField entity)
        {
            throw new System.NotImplementedException();
        }
    }
}