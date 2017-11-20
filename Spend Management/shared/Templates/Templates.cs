namespace Spend_Management.shared.Templates
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Runtime.Caching;
    using System.Web.Caching;

    using SpendManagementLibrary;

    /// <summary>
    /// The template for a collection class that manages library classes
    /// </summary>
    public class Templates
    {
        /// <summary>
        /// 
        /// </summary>
        private ICurrentUser _currentUser;

        /// <summary>
        /// 
        /// </summary>
        private List<Template> _templates;

        /// <summary>
        /// 
        /// </summary>
        private string _cacheKey;

        /// <summary>
        /// 
        /// </summary>
        private static string CacheKey(int accountId, int? subAccountId = null)
        {
            return (subAccountId.HasValue) ? string.Format("templates_{0}_{1}", accountId, subAccountId) : string.Format("templates_{0}", accountId);
        }

        /// <summary>
        /// The reference to the cache
        /// </summary>
        private Cache _cache = System.Web.HttpRuntime.Cache; // should this be Caching.cs Cache now?

        /// <summary>
        /// The sql to create a cache dependency for templates
        /// </summary>
        private const string CacheSQL = "SELECT [templateId], [cacheExpiry] FROM [dbo].[templates] WHERE [subAccountId] = @subAccountId AND @accountId = @accountId";

        /// <summary>
        /// The sql to fetch templates
        /// </summary>
        private const string SQL = "SELECT [templateId] FROM [dbo].[templates] WHERE [subAccountId] = @subAccountId";

        /// <summary>
        /// The sql to fetch templates
        /// </summary>
        private const string SQLSubs = "SELECT [templateSubId], [templateId] FROM [dbo].[templateSubs] INNER JOIN [templates] ON [templates].[templateId] = [templateSubs].[templateId] WHERE [template].[subAccountId] = @subAccountId";

        /// <summary>
        /// object used purely for locking
        /// </summary>
        private static readonly object CachingSyncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        public Templates(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _cacheKey = CacheKey(_currentUser.AccountID, _currentUser.CurrentSubAccountId);

            InitialiseCache();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitialiseCache()
        {
            _templates = _cache[_cacheKey] as List<Template> ?? PopulateCacheList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<Template> PopulateCacheList()
        {
            lock (CachingSyncRoot)
            {
                if (_templates == null)
                {
                    _templates = this.GetTemplates();
                }
            }

            return _templates;
        }

        /// <summary>
        /// Obtain a list of all templates
        /// </summary>
        /// <returns>The full list of templates</returns>
        private List<Template> GetTemplates()
        {
            DBConnection db = new DBConnection(_currentUser.Account.ConnectionString);
            List<Template> templates = new List<Template>();
            SortedList<int, List<TemplateSub>> templateSubs = this.GetTemplateSubs();  // prefetch sub items to avoid readers within readers

            db.sqlexecute.Parameters.AddWithValue("@subAccountId", _currentUser.CurrentSubAccountId);

            using (SqlDataReader reader = db.GetReader(SQL))
            {
                // todo: get ordinals

                while (reader.Read())
                {
                    // todo: get template from table
                    // todo: create template object
                    // todo: add to list
                    // todo: get sub items from list of templateSubs
                }

                // todo: insert list into cache, don't forget a dependency using CacheSQL

                db.sqlexecute.Parameters.Clear();
            }

            return templates;
        }

        /// <summary>
        /// Get an individual Template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Template GetById(int id)
        {
            // todo: fetch object from list, handle null entries
            return new Template();
        }

        /// <summary>
        /// Save or update a template object
        /// </summary>
        /// <param name="template">A template object, the id should be 0 for new templates</param>
        /// <returns>The template id or a negative status code</returns>
        public int Save(Template template)
        {
            // todo: save the template object (insert for 0 ids, update otherwise) and return id or negative status code
            // don't forget to update cacheExpiry and clear the cache entry for this key
            return 0;
        }

        #region TemplateSubs

        /// <summary>
        /// Get the list of templatesubs for the templates being cached
        /// </summary>
        /// <returns>Sorted list of lists of templatesubs indexed by their parent template id</returns>
        private SortedList<int, List<TemplateSub>> GetTemplateSubs()
        {
            DBConnection db = new DBConnection(_currentUser.Account.ConnectionString);
            SortedList<int, List<TemplateSub>> templateSubs = new SortedList<int, List<TemplateSub>>();

            db.sqlexecute.Parameters.AddWithValue("@subAccountId", _currentUser.CurrentSubAccountId);

            using (SqlDataReader reader = db.GetReader(SQLSubs))
            {
                // todo: get ordinals

                while (reader.Read())
                {
                    // todo: get templatesub items from table
                    // todo: create templatesub object
                    // todo: add to list indexed by template id
                }

                db.sqlexecute.Parameters.Clear();
            }

            return templateSubs;
        }

        #endregion TemplateSubs
    }
}