namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The account property CRUD.
    /// </summary>
    public class AccountPropertyCrud : EntityBase, IDataAccess<AccountProperty>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AccountPropertyCrud"/> class.
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
        public AccountPropertyCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.AccountPropertyApi == null)
            {
                this.DataSources.AccountPropertyApi = new AccountPropertyApi(metabase, accountid);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<AccountProperty> Create(List<AccountProperty> entities)
        {
            return this.DataSources.AccountPropertyApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Read(int entityId)
        {
            return this.DataSources.AccountPropertyApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Read(long entityId)
        {
            return this.DataSources.AccountPropertyApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<AccountProperty> ReadAll()
        {
            return this.DataSources.AccountPropertyApi.ReadAll();
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<AccountProperty> ReadByEsrId(long personId)
        {
            return this.DataSources.AccountPropertyApi.ReadByEsrId(personId);
        }

        /// <summary>
        /// The read special.  Not implimented
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<AccountProperty> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<AccountProperty> Update(List<AccountProperty> entities)
        {
            return this.DataSources.AccountPropertyApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Delete(int entityId)
        {
            return this.DataSources.AccountPropertyApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Delete(long entityId)
        {
            return this.DataSources.AccountPropertyApi.Delete(entityId);
        }

        public AccountProperty Delete(AccountProperty entity)
        {
            throw new NotImplementedException();
        }
    }
}