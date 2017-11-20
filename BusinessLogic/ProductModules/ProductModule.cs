namespace BusinessLogic.ProductModules
{
    using System;

    /// <summary>
    /// An instance of <see cref="IProductModule"/>
    /// </summary>
    public abstract class ProductModule : IProductModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModule"/> class. 
        /// </summary>
        /// <param name="id">
        /// The identifier for <see cref="IProductModule"/>
        /// </param>
        /// <param name="name">
        /// The Name of the <see cref="IProductModule"/>
        /// </param>
        /// <param name="description">
        /// The Description of the <see cref="IProductModule"/>
        /// </param>
        /// <param name="brandName">
        /// The Brand Name of the <see cref="IProductModule"/>
        /// </param>
        protected ProductModule(int id, string name, string description, string brandName)
        {
            if (id <= 0)
            {
                throw new ArgumentException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (string.IsNullOrWhiteSpace(brandName))
            {
                throw new ArgumentNullException(nameof(brandName));
            }

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.BrandName = brandName;
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
    }
}
