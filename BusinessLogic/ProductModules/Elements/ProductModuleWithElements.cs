namespace BusinessLogic.ProductModules.Elements
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Elements;

    /// <summary>
    /// Defines a <see cref="ProductModuleWithElements"/> and all it's members
    /// </summary>
    [Serializable]
    public abstract class ProductModuleWithElements : IProductModuleWithElements
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModuleWithElements"/> class.
        /// </summary>
        /// <param name="id">
        /// The identifier for <see cref="ProductModuleWithElements"/>
        /// </param>
        /// <param name="name">
        /// The Name of the <see cref="ProductModuleWithElements"/>
        /// </param>
        /// <param name="description">
        /// The Description of the <see cref="ProductModuleWithElements"/>
        /// </param>
        /// <param name="brandName">
        /// The Brand Name of the <see cref="ProductModuleWithElements"/>
        /// </param>
        /// <param name="brandNameHtml">
        /// The Brand Name html of the <see cref="ProductModuleWithElements"/>
        /// </param>
        /// <param name="elements">
        /// A <see cref="IList{T}"/> of <see cref="IElement"/> for the <see cref="ProductModuleWithElements"/>
        /// </param>
        protected ProductModuleWithElements(int id, string name, string description, string brandName, string brandNameHtml, IList<IElement> elements)
        {
            if (id <= 0)
            {
                throw new ArgumentException(nameof(id));
            }

            Guard.ThrowIfNullOrWhiteSpace(name, nameof(name));
            Guard.ThrowIfNull(description, nameof(description));
            Guard.ThrowIfNull(elements, nameof(elements));

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.BrandName = brandName;
            this.BrandNameHtml = brandNameHtml;
            this.Elements = elements;
        }

        /// <summary>
        /// Gets or sets the identifier for <see cref="ProductModuleWithElements"/>
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the Name of the <see cref="ProductModuleWithElements"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Description of the <see cref="ProductModuleWithElements"/>
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the Brand Name of the <see cref="ProductModuleWithElements"/>
        /// </summary>
        public string BrandName { get; }

        /// <summary>
        /// Gets the Brand Name Html of the <see cref="ProductModuleWithElements"/>
        /// </summary>
        public string BrandNameHtml { get; }

        /// <summary>
        /// Gets the <see cref="IList{IElement}"/> of the <see cref="ProductModuleWithElements"/>
        /// </summary>
        public IList<IElement> Elements { get; }

        /// <summary>
        /// Get the HomePage of the <see cref="ProductModuleWithElements"/>
        /// </summary>
        public string HomePage => "~/home.aspx";
    }
}
