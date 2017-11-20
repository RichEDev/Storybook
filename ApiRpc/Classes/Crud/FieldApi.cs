namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;
    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Interfaces;
    public class FieldApi : IDataAccess<Field>
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
        public FieldApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<Field> Create(List<Field> entities)
        {
            throw new System.NotImplementedException();
        }

        public Field Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public Field Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<Field> ReadAll()
        {
            return this.crudApi.ReadAllFields(this.metaBase, this.accountId.ToString());
        }

        public List<Field> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<Field> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<Field> Update(List<Field> entities)
        {
            throw new System.NotImplementedException();
        }

        public Field Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public Field Delete(Field entity)
        {
            throw new System.NotImplementedException();
        }
    }
}