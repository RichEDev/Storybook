namespace BusinessLogic.ProductModules
{
    /// <summary>
    /// An implenentaino of <see cref="IProductModule"/> which does nothing and has no module.
    /// </summary>
    public class NullProductModule: ProductModule
    {
        /// <summary>
        /// Create a blank <see cref="IProductModule"/> object
        /// </summary>
        /// <param name="id">The identifier for <see cref="IProductModule"/></param>
        /// <param name="name">The Name of the <see cref="IProductModule"/></param>
        /// <param name="description">The Description of the <see cref="IProductModule"/></param>
        /// <param name="brandName">The Brand Name of the <see cref="IProductModule"/></param>
        public NullProductModule(int id, string name, string description, string brandName)
            : base(id, name, description, brandName)
        {
        }
    }
}
