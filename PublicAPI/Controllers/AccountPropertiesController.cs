namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using PublicAPI.DTO;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IAccountProperty"/> using the <see cref="AccountPropertiesDTO"/>.
    /// </summary>
    public class AccountPropertiesController : ApiController
    {
        private readonly Lazy<IDataFactory<IAccountProperty, AccountPropertyCacheKey>> _accountPropertiesFactory = new Lazy<IDataFactory<IAccountProperty, AccountPropertyCacheKey>>(() => WebApiApplication.container.GetInstance<IDataFactory<IAccountProperty, AccountPropertyCacheKey>>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/AccountProperties">https://api.hostname/AccountProperties</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="AccountPropertiesDTO"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IAccountProperty> accountProperties = this._accountPropertiesFactory.Value.Get();
            IEnumerable<AccountPropertiesDTO> accountPropertiesDtoCollection = MapObjects.Map<IEnumerable<AccountPropertiesDTO>>(accountProperties);

            return this.Json(accountPropertiesDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="AccountPropertiesDTO"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/AccountProperties/{subAccountId}/{key}">https://api.hostname/AccountProperties/{subAccountId}/{key}</a>
        /// </remarks>
        /// <param name="subAccountId">The sub account id to match on</param>
        /// <param name="key">The key to match on</param>
        /// <returns>An instance of <see cref="AccountPropertiesDTO"/> with a matching id, of nothing is not matched.</returns>
        [Route("AccountProperties/{subAccountId}/{key}")]
        public IHttpActionResult Get(int subAccountId, string key)
        {
            AccountPropertiesDTO accountProperties = MapObjects.Map<AccountPropertiesDTO>(this._accountPropertiesFactory.Value[new AccountPropertyCacheKey(key, subAccountId.ToString())]);

            return this.Json(accountProperties);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/AccountProperties">https://api.hostname/AccountProperties</a>
        ///  Body: <see cref="P11DCategoryDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="AccountPropertiesDTO"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="AccountPropertiesDTO"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post(AccountPropertiesDTO value)
        {
            IAccountProperty accountProperty = MapObjects.Map<AccountProperty>(value);
            accountProperty = this._accountPropertiesFactory.Value.Save(accountProperty);
            value = MapObjects.Map<AccountPropertiesDTO>(accountProperty);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/AccountProperties">https://api.hostname/AccountProperties</a>
        ///  Body: <see cref="AccountPropertiesDTO"/>
        /// </remarks>
        /// <param name="value">The <see cref="AccountPropertiesDTO"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="AccountPropertiesDTO"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put(AccountPropertiesDTO value)
        {
            IAccountProperty accountProperty = MapObjects.Map<AccountProperty>(value);
            accountProperty = this._accountPropertiesFactory.Value.Save(accountProperty);
            value = MapObjects.Map<AccountPropertiesDTO>(accountProperty);

            return this.Json(value);
        }
    }
}