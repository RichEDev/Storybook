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

        /// <summary>
        /// Get the Brand Name Html of the <see cref="IProductModule"/>
        /// </summary>
        string BrandNameHtml { get; }

        /// <summary>
        /// Get the HomePage of the <see cref="IProductModule"/>
        /// </summary>
        string HomePage { get; }
    }
}