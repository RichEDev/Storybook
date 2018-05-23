namespace SpendManagementLibrary.HelpAndSupport
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    using BusinessLogic.Modules;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.SalesForceApi;
    using Utilities.DistributedCaching;

    public class Knowledge
    {
        /// <summary>
        /// Gets all tables and relationships for Support Questions and their Headings
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>A dataset containing tables for SupportQuestions and SupportQuestionHeadings</returns>
        public static DataSet GetHeadingsAndQuestions(ICurrentUserBase user)
        {
            string sql;

            var cache = new Cache();
            string cacheKey = string.Format("KnowledgeQuestionsAndHeadings_Module_{0}", (int)user.CurrentActiveModule);
            var questions = cache.Get(0, cacheKey, string.Empty) as DataSet;

            if (questions == null)
            {
                using (var databaseConnection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
                {
                    sql = string.Format("SELECT SupportQuestionHeadingId, Heading FROM SupportQuestionHeadings WHERE [ModuleId] = {0} OR [ModuleId] = @CurrentActiveModuleId ORDER BY [Order]; SELECT SupportQuestionId, Question, KnowledgeArticleUrl, SupportTicketSel, SupportTicketInternal, SupportQuestions.SupportQuestionHeadingId, 0 AS [CustomEntityId] FROM SupportQuestions INNER JOIN SupportQuestionHeadings ON SupportQuestionHeadings.SupportQuestionHeadingId = SupportQuestions.SupportQuestionHeadingId WHERE SupportQuestionHeadings.ModuleId = {0} OR SupportQuestionHeadings.ModuleId = @CurrentActiveModuleId ORDER BY [SupportQuestions].[Order];", (int)Modules.SpendManagement);
                    databaseConnection.AddWithValue("@CurrentActiveModuleId", (int)user.CurrentActiveModule);
                    questions = databaseConnection.GetDataSet(sql);

                    questions.Tables[0].TableName = "SupportQuestionHeadings";
                    questions.Tables[1].TableName = "SupportQuestions";
                    questions.Relations.Add("HeadingQuestionGroup", questions.Tables["SupportQuestionHeadings"].Columns["SupportQuestionHeadingId"], questions.Tables["SupportQuestions"].Columns["SupportQuestionHeadingId"]);
                }

                cache.Add(0, cacheKey, string.Empty, questions);
            }

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                sql = "SELECT entityId, supportQuestion FROM customEntities WHERE supportQuestion IS NOT NULL;";

                using (IDataReader reader = databaseConnection.GetReader(sql))
                {
                    if (reader.Read())
                    {
                        // add a new heading for Greenlights
                        DataRow row = questions.Tables["SupportQuestionHeadings"].NewRow();
                        row["SupportQuestionHeadingId"] = -1;
                        row["Heading"] = "I have a question about my online forms";

                        questions.Tables["SupportQuestionHeadings"].Rows.Add(row);

                        // add a question for each Greenlight that has support information defined
                        do
                        {
                            row = questions.Tables["SupportQuestions"].NewRow();
                            row["Question"] = reader.GetString(reader.GetOrdinal("supportQuestion"));
                            row["CustomEntityId"] = reader.GetInt32(reader.GetOrdinal("entityId"));
                            row["SupportTicketSel"] = false;
                            row["SupportTicketInternal"] = true;
                            row["SupportQuestionHeadingId"] = -1;

                            questions.Tables["SupportQuestions"].Rows.Add(row);
                        }
                        while (reader.Read());
                    }
                }
            }

            return questions;
        }

        /// <summary>
        /// Searches local knowledge articles
        /// </summary>
        /// <param name="accountIdentifier">The account ID</param>
        /// <param name="searchTerm">The string to search for</param>
        /// <param name="productCategory">An optional category to filter the results by</param>
        /// <param name="articleType">An optional article type to filter the results by</param>
        /// <returns>A list of knowledge articles</returns>
        public static List<IKnowledgeArticle> Search(int accountIdentifier, string searchTerm, string productCategory, string articleType)
        {
            var result = new List<IKnowledgeArticle>();

            List<KnowledgeCustomArticle> customArticles = KnowledgeCustomArticle.Search(accountIdentifier, searchTerm, productCategory);
            List<KnowledgeSalesForceArticle> salesForceArticles = KnowledgeSalesForceArticle.Search(searchTerm, productCategory, articleType);

            result.AddRange(customArticles);
            result.AddRange(salesForceArticles);

            return result;
        }

        /// <summary>
        /// Retrieves all knowledge article categories, either from cache or from the SalesForce API
        /// </summary>
        /// <param name="groupName">The root sales force article category group name, default is "Product_Categories"</param>
        /// <param name="withNone">Whether or not to return a "[None]" item with the list, default is true</param>
        /// <returns>A dictionary of pairs; category name/label</returns>
        public static OrderedDictionary GetArticleCategories(string groupName = "Product_Categories", bool withNone = true)
        {
            // attempt to retrieve the categories from cache
            var cache = new Cache();
            var categories = (OrderedDictionary)cache.Get(0, "KnowledgeCategories", groupName);

            if (categories == null)
            {
                SforceService binding = GetSalesForceBinding(true);

                // SalesForce data categories are retrieved in pairs, but we only need one
                var pair = new DataCategoryGroupSobjectTypePair
                            {
                                sobject = "KnowledgeArticleVersion", 
                                dataCategoryGroupName = groupName
                            };
                DataCategoryGroupSobjectTypePair[] pairs = { pair, null };

                // Get the list of top level categories using the "describe" call
                DescribeDataCategoryGroupStructureResult[] results = binding.describeDataCategoryGroupStructures(pairs, false);

                // Take the first (should be only) result and get full category hierarchy 
                foreach (DataCategory[] topCategories in results.Select(result => result.topCategories).Take(1))
                {
                    foreach (DataCategory topCategory in topCategories.Take(1))
                    {
                        categories = GetChildCategories(topCategory);
                    }
                }

                cache.Add(0, "KnowledgeCategories", groupName, categories);
            }

            if (categories != null && withNone)
            {
                categories.Insert(0, string.Empty, "[None]");
            }

            return categories;
        }

        /// <summary>
        /// Takes a single DataCategory and recursively retrieves all child DataCategories
        /// </summary>
        /// <param name="category">The <see cref="DataCategory"/> to retrieve child <see cref="DataCategory"/> from</param>
        /// <param name="categories">An optional Dictionary containing results that have already been retrieved</param>
        /// <returns>A dictionary of child categories</returns>
        private static OrderedDictionary GetChildCategories(DataCategory category, OrderedDictionary categories = null)
        {
            // create a new result set if one wasn't passed in
            if (categories == null)
            {
                categories = new OrderedDictionary();
            }

            // add each child for this category
            foreach (DataCategory childCategory in category.childCategories)
            {
                categories.Add(childCategory.name, childCategory.label);

                // if the child contains more children call this method again
                if (childCategory.childCategories != null)
                {
                    categories = GetChildCategories(childCategory, categories);
                }
            }

            return categories;
        }

        /// <summary>
        /// Retrieves a SalesForce binding object for accessing their API
        /// </summary>
        /// <param name="forceProduction">Optionally force SalesForce API into production mode regardless of the setting, needed because there is no sandbox for Knowledge</param>
        /// <returns>The <see cref="SforceService"/> binding</returns>
        public static SforceService GetSalesForceBinding(bool forceProduction = false)
        {
            bool sandbox;
            bool.TryParse(ConfigurationManager.AppSettings["SalesForceApiSandboxMode"], out sandbox);

            var binding = new SforceService();

            string username = ConfigurationManager.AppSettings["SalesForceApiUsername"];
            if (sandbox && !forceProduction)
            {
                username += ".sandbox";
                binding.Url = binding.Url.Replace("login.salesforce.com", "test.salesforce.com");
            }
            LoginResult loginResult = binding.login(username, ConfigurationManager.AppSettings["SalesForceApiPassword"]);

            binding.Url = loginResult.serverUrl;
            binding.SessionHeaderValue = new SessionHeader { sessionId = loginResult.sessionId };

            return binding;
        }

        /// <summary>
        /// Indicates whether or not a user has access to "Circle", the SEL Support Portal hosted by SalesForce
        /// SalesForce users have a "Portal User" flag, which is a custom property
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>True if a user with the same e-mail address has access to Circle, otherwise false</returns>
        public static bool CanAccessCircle(ICurrentUserBase user)
        {
            bool canAccess = false;

            // attempt to retrieve the value from cache
            var cache = new Cache();
            bool? cachedValue = cache.Get(user.AccountID, "HelpAndSupportCircleAccess", user.EmployeeID.ToString(CultureInfo.InvariantCulture)) as bool?;

            if (cachedValue.HasValue)
            {
                canAccess = cachedValue.Value;
            }
            else
            {
                SforceService binding = GetSalesForceBinding();
                QueryResult result = binding.query(string.Format("SELECT Id, Portal_User__c FROM Contact WHERE Contact.Email = '{0}'", user.Employee.EmailAddress.Replace("'", @"\'")));

                if (result.records != null)
                {
                    foreach (var searchResult in result.records)
                    {
                        canAccess = ((Contact)searchResult).Portal_User__c ?? false;

                        // cache the value
                        cache.Add(user.AccountID, "HelpAndSupportCircleAccess", user.EmployeeID.ToString(CultureInfo.InvariantCulture), canAccess, Cache.CacheTimeSpans.Short);

                        break;
                    }
                }
            }

            return canAccess;
        }
    }
}