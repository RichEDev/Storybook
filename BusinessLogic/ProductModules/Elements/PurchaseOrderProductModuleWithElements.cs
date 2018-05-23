﻿namespace BusinessLogic.ProductModules
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Elements;
    using BusinessLogic.ProductModules.Elements;

    [Serializable]
    public class PurchaseOrderProductModuleWithElements : ProductModuleWithElements
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderProductModule"/> class.
        /// </summary>
        /// <param name="id">The identifier for <see cref="IProductModule"/></param>
        /// <param name="name">The Name of the <see cref="IProductModule"/></param>
        /// <param name="description">The Description of the <see cref="IProductModule"/></param>
        /// <param name="brandName">The Brand Name of the <see cref="IProductModule"/></param>
        /// <param name="brandNameHtml">The Brand Name Html of the <see cref="IProductModule"/></param>
        /// <param name="elements">The elements of the <see cref="IProductModule"/></param>
        public PurchaseOrderProductModuleWithElements(int id, string name, string description, string brandName, string brandNameHtml, IList<IElement> elements)
            : base(id, name, description, brandName, brandNameHtml, elements)
        {
        }
    }
}
