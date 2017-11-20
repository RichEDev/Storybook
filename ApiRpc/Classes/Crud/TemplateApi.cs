namespace ApiRpc.Classes.Crud
{
    using System;
    using System.Collections.Generic;

    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;

    using Interfaces;
    public class TemplateApi : IDataAccess<Template>
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
        public TemplateApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<Template> Create(List<Template> entities)
        {
            throw new NotImplementedException();
        }

        public Template Read(int entityId)
        {
            return this.crudApi.ReadTemplate(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public Template Read(long entityId)
        {
            return this.crudApi.ReadTemplate(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<Template> ReadAll()
        {
            throw new NotImplementedException();
        }

        public List<Template> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
        }

        public List<Template> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        public List<Template> Update(List<Template> entities)
        {
            throw new NotImplementedException();
        }

        public Template Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public Template Delete(Template entity)
        {
            throw new NotImplementedException();
        }
    }
}