namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    
    using BusinessLogic.DataConnections;
    using BusinessLogic.P11DCategories;

    using Common.Actions;
    using DTO;
    using Security.Filters;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IP11DCategory"/> using the <see cref="P11DCategoryDto"/>.
    /// </summary>
    [JwtAuthentication]
    public class P11DCategoriesController : ApiController, ICrud<P11DCategoryDto, int>
    {
        private readonly Lazy<IDataFactory<IP11DCategory, int>> _p11DCategories = new Lazy<IDataFactory<IP11DCategory, int>>(() => WebApiApplication.container.GetInstance<IDataFactory<IP11DCategory, int>>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="P11DCategoryDto"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/P11DCategories">https://api.hostname/P11DCategories</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="P11DCategoryDto"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IP11DCategory> p11DCategories = this._p11DCategories.Value.Get();
            IEnumerable<P11DCategoryDto> p11DCategoriesDtoCollection = MapObjects.Map<IEnumerable<P11DCategoryDto>>(p11DCategories);

            return this.Json(p11DCategoriesDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="P11DCategoryDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/P11DCategories/{id}">https://api.hostname/P11DCategories/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="P11DCategoryDto"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            P11DCategoryDto p11DCategory = MapObjects.Map<P11DCategoryDto>(this._p11DCategories.Value[id]);

            return this.Json(p11DCategory);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="P11DCategoryDto"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/P11DCategories">https://api.hostname/P11DCategories</a>
        ///  Body: <see cref="P11DCategoryDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="P11DCategoryDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="P11DCategoryDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post([FromBody]P11DCategoryDto value)
        {
            IP11DCategory p11DCategory = MapObjects.Map<P11DCategory>(value);
            p11DCategory = this._p11DCategories.Value.Save(p11DCategory);
            value = MapObjects.Map<P11DCategoryDto>(p11DCategory);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="P11DCategoryDto"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/P11DCategories">https://api.hostname/P11DCategories</a>
        ///  Body: <see cref="P11DCategoryDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="P11DCategoryDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="P11DCategoryDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put([FromBody]P11DCategoryDto value)
        {
            IP11DCategory p11DCategory = MapObjects.Map<P11DCategory>(value);
            p11DCategory = this._p11DCategories.Value.Save(p11DCategory);
            value = MapObjects.Map<P11DCategoryDto>(p11DCategory);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to delete a specific <see cref="P11DCategoryDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/P11DCategories/{id}">https://api.hostname/P11DCategories/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An <see cref="int"/> code with the response of the delete.</returns>
        public IHttpActionResult Delete(int id)
        {
            return this.Json(this._p11DCategories.Value.Delete(id));
        }
    }
}