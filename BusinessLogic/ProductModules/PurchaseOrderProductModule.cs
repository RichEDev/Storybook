﻿namespace BusinessLogic.ProductModules
{
    using System;

    [Serializable]
    public class PurchaseOrderProductModule : ProductModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderProductModule"/> class.
        /// </summary>
        /// <param name="id">The identifier for <see cref="IProductModule"/></param>
        /// <param name="name">The Name of the <see cref="IProductModule"/></param>
        /// <param name="description">The Description of the <see cref="IProductModule"/></param>
        /// <param name="brandName">The Brand Name of the <see cref="IProductModule"/></param>
        /// <param name="brandNameHtml">The Brand Name Html of the <see cref="IProductModule"/></param>
        public PurchaseOrderProductModule(int id, string name, string description, string brandName, string brandNameHtml)
            : base(id, name, description, brandName, brandNameHtml)
        {
        }
    }
}