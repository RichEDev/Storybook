namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Specialized;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Base class for a Knowledge Article
    /// </summary>
    public class KnowledgeArticle : IKnowledgeArticle
    {
        #region Properties

        /// <summary>
        /// The identifier
        /// </summary>
        public object Identifier { get; set; }

        /// <summary>
        /// The title / question
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The category, this should be the key of a SalesForce API ProductCategory
        /// </summary>
        public string ProductCategoryValue { get; set; }

        /// <summary>
        /// A short description of the article
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The body of the article
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// A flag indicating whether or not the article is active
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// When the article was published
        /// </summary>
        public DateTime PublishedOn { get; set; }

        /// <summary>
        /// The friendly name for the category
        /// </summary>
        public string ProductCategory
        {
            get
            {
                OrderedDictionary categories = Knowledge.GetArticleCategories();
                return categories[ProductCategoryValue].ToString();
            }
        }

        #endregion Properties

    }
}
