namespace BusinessLogic.ProductModules
{
    public class GreenLightWorkForceProductModule:ProductModule
    {
        /// <summary>
        /// Create a new instance of <see cref="GreenLightWorkForceProductModule"/>
        /// </summary>
        /// <param name="id">The identifier for <see cref="IProductModule"/></param>
        /// <param name="name">The Name of the <see cref="IProductModule"/></param>
        /// <param name="description">The Description of the <see cref="IProductModule"/></param>
        /// <param name="brandName">The Brand Name of the <see cref="IProductModule"/></param>
        public GreenLightWorkForceProductModule(int id, string name, string description, string brandName)
            : base(id, name, description, brandName)
        {
        }
    }
}
