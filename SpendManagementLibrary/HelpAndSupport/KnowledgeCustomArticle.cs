namespace SpendManagementLibrary.HelpAndSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Represents a "local" Knowledge article which is stored in the customer's database
    /// </summary>
    public class KnowledgeCustomArticle : KnowledgeArticle
    {
        /// <summary>
        /// Retrieves the total number of published <see cref="KnowledgeCustomArticle"/>
        /// </summary>
        /// <param name="accountIdentifier">The <see cref="cAccount"/> identifier</param>
        /// <returns>The count</returns>
        /// todo this might be obsolete now
        public static int Count(int accountIdentifier)
        {
            int result;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                const string Sql = "SELECT COUNT(KnowledgeCustomArticleId) AS ArticleCount FROM KnowledgeCustomArticles WHERE Published = 1;";
                result = databaseConnection.ExecuteScalar<int>(Sql);
            }

            return result;
        }
        
        /// <summary>
        /// Retrieves an instance of <see cref="KnowledgeCustomArticle"/> from storage
        /// </summary>
        /// <param name="accountIdentifier">The <see cref="cAccount"/> identifier</param>
        /// <param name="identifier">The <see cref="KnowledgeCustomArticle"/> identifier</param>
        /// <returns>The requested <see cref="KnowledgeCustomArticle"/> or null if none was found</returns>
        public static KnowledgeCustomArticle Get(int accountIdentifier, int identifier)
        {
            if (accountIdentifier <= 0)
            {
                throw new ArgumentOutOfRangeException("accountIdentifier", accountIdentifier, "The value of this argument must be greater than 0.");
            }

            KnowledgeCustomArticle article = null;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                const string Sql = "SELECT KnowledgeCustomArticleId, Title, ProductCategory, Summary, Body, Published, PublishedOn FROM KnowledgeCustomArticles WHERE KnowledgeCustomArticleId = @articleId;";
                databaseConnection.AddWithValue("@articleId", identifier);

                using (IDataReader reader = databaseConnection.GetReader(Sql))
                {
                    int identifierOrdinal = reader.GetOrdinal("KnowledgeCustomArticleId"),
                        titleOrdinal = reader.GetOrdinal("Title"),
                        productCategoryOrdinal = reader.GetOrdinal("ProductCategory"),
                        summaryOrdinal = reader.GetOrdinal("Summary"),
                        bodyOrdinal = reader.GetOrdinal("Body"),
                        publishedOrdinal = reader.GetOrdinal("Published"),
                        publishedOnOrdinal = reader.GetOrdinal("PublishedOn");

                    while (reader.Read())
                    {
                        article = new KnowledgeCustomArticle
                        {
                            Identifier = reader.GetInt32(identifierOrdinal),
                            Title = reader.GetString(titleOrdinal),
                            ProductCategoryValue = reader.GetString(productCategoryOrdinal),
                            Summary = reader.GetString(summaryOrdinal),
                            Body = reader.GetString(bodyOrdinal),
                            Published = reader.GetBoolean(publishedOrdinal),
                            PublishedOn = reader.IsDBNull(publishedOnOrdinal) ? DateTime.UtcNow : reader.GetDateTime(publishedOnOrdinal)
                        };
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return article;
        }

        /// <summary>
        /// Deletes a <see cref="KnowledgeCustomArticle"/> record from storage
        /// </summary>
        /// <param name="user">The <see cref="ICurrentUserBase"/></param>
        /// <param name="identifier">The <see cref="KnowledgeCustomArticle"/> identifier</param>
        /// <returns>0 to indicate the record has been deleted</returns>
        public static int Delete(ICurrentUserBase user, int identifier)
        {
            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                databaseConnection.AddWithValue("@KnowledgeCustomArticleId", identifier);
                databaseConnection.AddWithValue("@UserID", user.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);
                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = user.Delegate.EmployeeID;
                }

                databaseConnection.ExecuteProc("DeleteKnowledgeCustomArticle");
                databaseConnection.sqlexecute.Parameters.Clear();            
            }

            return 0;
        }

        /// <summary>
        /// Saves an instance of <see cref="KnowledgeCustomArticle"/> to storage
        /// </summary>
        /// <param name="user">The <see cref="ICurrentUserBase"/></param>
        /// <returns>The identifier of the <see cref="KnowledgeCustomArticle"/> that was saved</returns>
        public int Save(ICurrentUserBase user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "The value of this argument must not be null.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                databaseConnection.AddWithValue("@KnowledgeCustomArticleId", this.Identifier);
                databaseConnection.AddWithValue("@Title", this.Title, 255);
                databaseConnection.AddWithValue("@ProductCategory", this.ProductCategoryValue, 255);
                databaseConnection.AddWithValue("@Summary", this.Summary, 255);
                databaseConnection.AddWithValue("@Body", this.Body, -1);
                databaseConnection.AddWithValue("@UserID", user.EmployeeID);
                databaseConnection.AddWithValue("@DelegateID", DBNull.Value);

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = user.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("SaveKnowledgeCustomArticle");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }
        
        /// <summary>
        /// Retrieves a collection of <see cref="KnowledgeCustomArticle"/> that match a search term 
        /// </summary>
        /// <param name="accountIdentifier">The <see cref="cAccount"/></param>
        /// <param name="searchTerm">The search term</param>
        /// <param name="category">A <see cref="KnowledgeCustomArticle"/> ProductCategoryValue, can be an empty string</param>
        /// <returns>A list of <see cref="KnowledgeCustomArticle"/></returns>
        public static List<KnowledgeCustomArticle> Search(int accountIdentifier, string searchTerm, string category)
        {
            var results = new List<KnowledgeCustomArticle>();

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                // todo, these LIKEs are bad, see about FREETEXT() which will require FULLTEXT indexing on all customer databases
                const string Sql = "SELECT KnowledgeCustomArticleId, ProductCategory, Title, Summary, PublishedOn FROM KnowledgeCustomArticles WHERE Published = 1 AND (Title LIKE '%' + @searchTerm + '%' OR Summary LIKE '%' + @searchTerm + '%' OR Body LIKE '%' + @searchTerm + '%') AND (@category = '' OR ProductCategory = @category)"; //FREETEXT(KnowledgeCustomArticles.*, @searchTerm)";
                databaseConnection.AddWithValue("@searchTerm", searchTerm, 255);
                databaseConnection.AddWithValue("@category", category, 255);

                using (IDataReader reader = databaseConnection.GetReader(Sql))
                {
                    int identifierOrdinal = reader.GetOrdinal("KnowledgeCustomArticleId"),
                        productCategoryOrdinal = reader.GetOrdinal("ProductCategory"),
                        titleOrdinal = reader.GetOrdinal("Title"),
                        summaryOrdinal = reader.GetOrdinal("Summary"),
                        publishedOnOrdinal = reader.GetOrdinal("PublishedOn");

                    while (reader.Read())
                    {
                        var result = new KnowledgeCustomArticle
                        {
                            Identifier = reader.GetInt32(identifierOrdinal),
                            Title = reader.GetString(titleOrdinal),
                            ProductCategoryValue = reader.GetString(productCategoryOrdinal),
                            Summary = reader.GetString(summaryOrdinal),
                            PublishedOn = reader.IsDBNull(publishedOnOrdinal) ? DateTime.UtcNow : reader.GetDateTime(publishedOnOrdinal)
                        };

                        results.Add(result);
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return results;
        }

        /// <summary>
        /// Toggles the Published status of a <see cref="KnowledgeCustomArticle"/> in storage
        /// </summary>
        /// <param name="user">The <see cref="ICurrentUserBase"/></param>
        /// <param name="identifier">The identifier of the <see cref="KnowledgeCustomArticle"/> to publish or unpublish</param>
        /// <returns>The identifier of the <see cref="KnowledgeCustomArticle"/> that was updated</returns>
        public static int TogglePublished(ICurrentUserBase user, int identifier)
        {
            if (identifier <= 0)
            {
                throw new ArgumentOutOfRangeException("identifier", identifier, "The value of this argument must be greater than 0.");
            }

            int returnValue;

            using (IDBConnection databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@KnowledgeCustomArticleId", identifier);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@UserID", user.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@DelegateID", DBNull.Value);

                if (user.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters["@DelegateID"].Value = user.Delegate.EmployeeID;
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                databaseConnection.ExecuteProc("ToggleKnowledgeCustomArticlePublished");

                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return returnValue;
        }
    }
}
