namespace BusinessLogic.ProductModules.Elements
{
    using System.Collections.Generic;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Elements;

    /// <summary>
    /// Defines a <see cref="IProductModuleWithElements"/> and all it's members
    /// </summary>
    public interface IProductModuleWithElements : IProductModule
    {
        /// <summary>
        /// Gets the <see cref="IElement"/> for this <see cref="IProductModuleWithElements"/>.
        /// </summary>
        IList<IElement> Elements { get; }
    }
}
