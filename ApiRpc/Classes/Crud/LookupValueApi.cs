namespace ApiRpc.Classes.Crud
{
    using global::ApiCrud;

    using ApiLibrary.DataObjects.Spend_Management;

    public class LookupValueApi
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
        public LookupValueApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public Lookup Read(string tableid, string fieldid, string keyvalue)
        {
            return this.crudApi.ReadLookupValue(this.metaBase, this.accountId.ToString(), tableid, fieldid, keyvalue);
        }
    }
}