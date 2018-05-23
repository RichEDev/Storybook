namespace PublicAPI.DTO
{
    using System.Collections.Generic;

    using BusinessLogic.Elements;

    public class ProductModuleWithElementsDTO
    {
        /// <summary>
        /// Get the Name of the <see cref="ProductModuleWithElementsDTO"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the Description of the <see cref="ProductModuleWithElementsDTO"/>
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Get the Brand Name of the <see cref="ProductModuleWithElementsDTO"/>
        /// </summary>
        public string BrandName { get; }

        /// <summary>
        /// Get the Brand Name Html of the <see cref="ProductModuleWithElementsDTO"/>
        /// </summary>
        public string BrandNameHtml { get; }

        /// <summary>
        /// Get the HomePage of the <see cref="ProductModuleWithElementsDTO"/>
        /// </summary>
        public string HomePage { get; }

        /// <summary>
        /// Gets the <see cref="IElement"/> for this <see cref="ProductModuleWithElementsDTO"/>.
        /// </summary>
        public IList<IElement> Elements { get; }
    }
}