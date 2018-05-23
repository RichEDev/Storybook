namespace BusinessLogic.ProductModules
{
    using System;

    /// <summary>
    /// An instance of <see cref="IProductModule"/>
    /// </summary>
    [Serializable]
    public abstract class ProductModule : IProductModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModule"/> class. 
        /// </summary>
        /// <param name="id">
        /// The identifier for <see cref="ProductModule"/>
        /// </param>
        /// <param name="name">
        /// The Name of the <see cref="ProductModule"/>
        /// </param>
        /// <param name="description">
        /// The Description of the <see cref="ProductModule"/>
        /// </param>
        /// <param name="brandName">
        /// The Brand Name of the <see cref="ProductModule"/>
        /// </param>
        /// <param name="brandNameHtml">
        /// The Brand Name html of the <see cref="ProductModule"/>
        /// </param>
        protected ProductModule(int id, string name, string description, string brandName, string brandNameHtml)
        {
            if (id <= 0)
            {
                throw new ArgumentException(nameof(id));
            }

            Guard.ThrowIfNullOrWhiteSpace(name, nameof(name));
            Guard.ThrowIfNull(description, nameof(description));

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.BrandName = brandName;
            this.BrandNameHtml = brandNameHtml;
        }

        /// <summary>
        /// Gets or sets the identifier for <see cref="IProductModule"/>
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the Name of the <see cref="IProductModule"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Description of the <see cref="IProductModule"/>
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the Brand Name of the <see cref="IProductModule"/>
        /// </summary>
        public string BrandName { get; }

        /// <summary>
        /// Gets the Brand Name html of the <see cref="IProductModule"/>
        /// </summary>
        public string BrandNameHtml { get; }

        /// <summary>
        /// Get the HomePage of the <see cref="IProductModule"/>
        /// </summary>
        public string HomePage => "~/home.aspx";
    }
}
