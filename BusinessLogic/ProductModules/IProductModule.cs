namespace BusinessLogic.ProductModules
{
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Define a product module.
    /// </summary>
    public interface IProductModule: IIdentifier<int>

    {
        /// <summary>
        /// Get the Name of the <see cref="IProductModule"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the Description of the <see cref="IProductModule"/>
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get the Brand Name of the <see cref="IProductModule"/>
        /// </summary>
        string BrandName { get; }
        
    }
}