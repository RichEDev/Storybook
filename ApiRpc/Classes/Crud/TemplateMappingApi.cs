namespace ApiRpc.Classes.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.ESR;

    using global::ApiCrud;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.Code;
    using global::ApiRpc.Interfaces;
    public class TemplateMappingApi : IDataAccess<TemplateMapping>
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
        public TemplateMappingApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<TemplateMapping> Create(List<TemplateMapping> entities)
        {
            return this.crudApi.CreateTemplateMappings(this.metaBase, this.accountId.ToString(), entities);
        }

        public TemplateMapping Read(int entityId)
        {
            return this.crudApi.ReadTemplateMapping(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public TemplateMapping Read(long entityId)
        {
            return this.crudApi.ReadTemplateMapping(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<TemplateMapping> ReadAll()
        {
            return this.crudApi.ReadAllTemplateMapping(this.metaBase, this.accountId.ToString());
        }

        public List<TemplateMapping> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get templates where VPD = reference
        /// </summary>
        /// <param name="reference">The VPD to read.</param>
        /// <returns></returns>
        public List<TemplateMapping> ReadSpecial(string reference)
        {
            var cache = MemoryCache.Default;
            if (!cache.Contains(string.Format("Trust_{0}", reference)))
            {
                var allRecords = this.ReadAll();
                if (allRecords != null)
                {
                    var result = !string.IsNullOrEmpty(reference) ? allRecords.Where(templateMapping => templateMapping.trustVPD == reference).ToList() : allRecords;
                    cache.Add(
                        string.Format("Trust_{0}", reference),
                        result,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
                    return result;
                }

            }
            else
            {
                return cache.Get(string.Format("Trust_{0}", reference)) as List<TemplateMapping>;
            }

            return new List<TemplateMapping>();
        }

        public List<TemplateMapping> Update(List<TemplateMapping> entities)
        {
            return this.crudApi.UpdateTemplateMappings(this.metaBase, this.accountId.ToString(), entities);
        }

        public TemplateMapping Delete(long entityId)
        {
            return this.crudApi.DeleteTemplateMapping(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public TemplateMapping Delete(TemplateMapping entity)
        {
            throw new System.NotImplementedException();
        }
    }
}