namespace SpendManagementLibrary.Interfaces
{
    using System;

    /// <summary>
    /// Common interface for Knowledge Articles
    /// </summary>
    public interface IKnowledgeArticle
    {
        #region Properties

        /// <summary>
        /// The identifier
        /// </summary>
        object Identifier { get; set; }

        /// <summary>
        /// The title / question
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The category, this should be the key of a SalesForce API ProductCategory
        /// </summary>
        string ProductCategoryValue { get; set; }

        /// <summary>
        /// A short description of the article
        /// </summary>
        string Summary { get; set; }

        /// <summary>
        /// The body of the article
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// When the article was published
        /// </summary>
        DateTime PublishedOn { get; set; }

        #endregion Properties
    }
}
