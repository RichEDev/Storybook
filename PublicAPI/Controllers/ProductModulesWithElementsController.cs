namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.ProductModules.Elements;

    using PublicAPI.Common.Actions;
    using PublicAPI.DTO;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IProductModuleWithElements"/> using the <see cref="IProductModuleWithElements"/>.
    /// </summary>
    public class ProductModulesWithElementsController : ApiController, IGet<int>
    {
        private readonly Lazy<IDataFactory<IProductModuleWithElements, int>> _productModuleWithElementsFactory = new Lazy<IDataFactory<IProductModuleWithElements, int>>(() => WebApiApplication.container.GetInstance<IDataFactory<IProductModuleWithElements, int>>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="ProductModuleWithElementsDTO"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/ProductModulesWithElements">https://api.hostname/ProductModulesWithElements</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="ProductModuleDTO"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IProductModuleWithElements> productModulesWithElements = this._productModuleWithElementsFactory.Value.Get();
            IEnumerable<ProductModuleWithElementsDTO> productModulesWithElementsDtoCollection = MapObjects.Map<IEnumerable<ProductModuleWithElementsDTO>>(productModulesWithElements);

            return this.Json(productModulesWithElementsDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="ProductModuleWithElementsDTO"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/ProductModulesWithElements/{id}">https://api.hostname/ProductModulesWithElements/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="ProductModuleWithElementsDTO"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            ProductModuleWithElementsDTO productModuleWithElements = MapObjects.Map<ProductModuleWithElementsDTO>(this._productModuleWithElementsFactory.Value[id]);

            return this.Json(productModuleWithElements);
        }
    }
}