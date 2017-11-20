namespace ApiRpc.Classes.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;

    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.ESR;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class TrustApi : IDataAccess<EsrTrust>
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

        private readonly Log logger;

        MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// Initialises a new instance of the <see cref="AssignmentApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public TrustApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
            this.logger = new Log();
        }

        public List<EsrTrust> Create(List<EsrTrust> entities)
        {
            return this.crudApi.CreateEsrTrusts(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrTrust Read(int entityId)
        {
            return this.crudApi.ReadEsrTrust(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrTrust Read(long entityId)
        {
            return this.crudApi.ReadEsrTrust(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrTrust> ReadAll()
        {
            return this.crudApi.ReadAllEsrTrust(this.metaBase, this.accountId.ToString());
        }

        public List<EsrTrust> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
        }

        public List<EsrTrust> ReadSpecial(string reference)
        {
            EsrTrust result;
            if (this.cache.Contains(string.Format("TrustRecord_{0}", reference)))
            {
                result = this.cache.Get(string.Format("TrustRecord_{0}", reference)) as EsrTrust;
            }
            else
            {
                result = this.crudApi.FindEsrTrust(this.metaBase, reference);
                this.logger.WriteDebug(
                    string.Empty,
                     Convert.ToInt32(reference),
                    result.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.EsrOutbound,
                    0,
                    reference,
                    LogRecord.LogReasonType.None,
                    string.Format("{0} Caching Trust record for Vpd{1} {2}", "GetAccountFromVpd", reference, DateTime.Now),
                    "ApiRpc");
                cache.Add(
                    string.Format("TrustRecord_{0}", reference),
                    result,
                    new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
            }

            return new List<EsrTrust> { result };
        }

        public List<EsrTrust> Update(List<EsrTrust> entities)
        {
            return this.crudApi.UpdateEsrTrusts(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrTrust Delete(long entityId)
        {
            return this.crudApi.DeleteEsrTrust(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrTrust Delete(EsrTrust entity)
        {
            throw new NotImplementedException();
        }
    }
}