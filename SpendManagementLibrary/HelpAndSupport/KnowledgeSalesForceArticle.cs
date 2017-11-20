using System.Configuration;

namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using SpendManagementLibrary.SalesForceApi;

    /// <summary>
    /// Represents a SalesForce Knowledge Article
    /// </summary>
    public class KnowledgeSalesForceArticle : KnowledgeArticle
    {
        /// <summary>
        /// A URL to the article on knowledge.software-europe.com
        /// </summary>
        public string Url;

        /// <summary>
        /// Retrieves a collection of <see cref="KnowledgeSalesForceArticle"/> that match a search term 
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <param name="productCategory">A <see cref="KnowledgeCustomArticle"/> ProductCategoryValue to filter the search by, can be an empty string</param>
        /// <param name="articleType">A SalesForce article type to filter the search by, can be an empty string</param>
        /// <returns>A list of <see cref="KnowledgeSalesForceArticle"/></returns>
        public static List<KnowledgeSalesForceArticle> Search(string searchTerm, string productCategory, string articleType)
        {
            var results = new List<KnowledgeSalesForceArticle>();

            // escape any non aplhanumeric characters
            searchTerm = Regex.Replace(searchTerm, "[^0-9a-zA-Z ]+", string.Empty);

            if (searchTerm.Length >= 3)
            {
                var binding = Knowledge.GetSalesForceBinding(true);

                string productNames = ConfigurationManager.AppSettings["SalesForceKnowledgeArticleProducts"];

                // build the SalesForce SOSL string, adding clauses as necessary 
                var searchQuery = string.Format("FIND {{{0}}} RETURNING KnowledgeArticleVersion(ArticleNumber, ArticleType, Title, UrlName, Summary, LastPublishedDate WHERE PublishStatus = 'Online' AND Language = 'en_US' LIMIT 20) WITH DATA CATEGORY Choose_your_Product__c AT ({1})", searchTerm, productNames);

                if (!string.IsNullOrEmpty(productCategory) || !string.IsNullOrEmpty(articleType))
                {
                    searchQuery += " AND ";
                }

                if (!string.IsNullOrEmpty(productCategory))
                {
                    searchQuery += string.Format("Product_Categories__c BELOW {0}__c", productCategory);

                    if (!string.IsNullOrEmpty(articleType))
                    {
                        searchQuery += " AND ";
                    }
                }

                if (!string.IsNullOrEmpty(articleType))
                {
                    searchQuery += string.Format("Document_Type__c AT {0}__c", articleType);
                }

                // perform the search
                SearchResult searchResults = binding.search(searchQuery);

                // construct a KnowledgeSalesForceArticle object for each search result
                if (searchResults.searchRecords != null)
                {
                    foreach (var searchResult in searchResults.searchRecords)
                    {
                        var salesForceArticle = (KnowledgeArticleVersion)searchResult.record;
                        var article = new KnowledgeSalesForceArticle
                        {
                            Title = salesForceArticle.Title,
                            Body = string.Empty,
                            Identifier = salesForceArticle.ArticleNumber,
                            Summary = salesForceArticle.Summary,
                            ProductCategoryValue = salesForceArticle.ArticleType,
                            Published = true,
                            PublishedOn = salesForceArticle.LastPublishedDate ?? DateTime.UtcNow,
                            Url = string.Format("/articles/{0}/{1}", salesForceArticle.ArticleType.Replace("__kav", string.Empty), salesForceArticle.UrlName)
                        };

                        results.Add(article);
                    }
                }
            }
            
            return results;
        }
    }
}
