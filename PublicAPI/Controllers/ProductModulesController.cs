namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.ProductModules;

    using PublicAPI.Common.Actions;
    using PublicAPI.DTO;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IProductModule"/> using the <see cref="IProductModule"/>.
    /// </summary>
    public class ProductModulesController : ApiController, IGet<int>
    {
        private readonly Lazy<IDataFactory<IProductModule, int>> _productModuleFactory = new Lazy<IDataFactory<IProductModule, int>>(() => WebApiApplication.container.GetInstance<IDataFactory<IProductModule, int>>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="ProductModuleDTO"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/ProductModules">https://api.hostname/ProductModules</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="ProductModuleDTO"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IProductModule> productModules = this._productModuleFactory.Value.Get();
            IEnumerable<ProductModuleDTO> productModulesDtoCollection = MapObjects.Map<IEnumerable<ProductModuleDTO>>(productModules);

            return this.Json(productModulesDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="ProductModuleDTO"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/ProductModules/{id}">https://api.hostname/ProductModules/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="ProductModuleDTO"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            ProductModuleDTO productModule = MapObjects.Map<ProductModuleDTO>(this._productModuleFactory.Value[id]);

            return this.Json(productModule);
        }
    }
}