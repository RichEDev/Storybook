namespace EsrGo2FromNhs.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Caching;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;

    /// <summary>
    /// The ESR TRUST CRUD.
    /// </summary>
    public class EsrTrustCrud : EntityBase, IDataAccess<EsrTrust>
    {
        readonly MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrTrustCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrTrustCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public List<EsrTrust> Create(List<EsrTrust> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrTrust>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrTrust>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public List<EsrTrust> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrTrust>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public List<EsrTrust> ReadByEsrId(long esrId)
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
                result = this.EsrApiHandler.FindEsrTrust(reference);
                this.Logger.WriteDebug(
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
                this.cache.Add(
                    string.Format("TrustRecord_{0}", reference),
                    result,
                    new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
            }

            return new List<EsrTrust> { result };
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public List<EsrTrust> Update(List<EsrTrust> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrTrust>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        public EsrTrust Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrTrust>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        public EsrTrust Delete(EsrTrust entity)
        {
            throw new NotImplementedException();
        }
    }
}