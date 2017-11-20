namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using Definitions;
    using Helpers;
    using Interfaces;
    using Utilities.DistributedCaching;

    /// <summary>
    /// Manages a dictionary of <see cref="AccountFolderPaths"/> objects, one for each account. 
    /// Each <see cref="AccountFolderPaths"/> is created using the metabase.dbo.databases table's paths.
    /// </summary>
    public class GlobalFolderPaths
    {
        #region Static Fields

        /// <summary>
        /// Private backing collection.
        /// </summary>
        private static readonly ConcurrentDictionary<int, AccountFolderPaths> BackingCollection = new ConcurrentDictionary<int, AccountFolderPaths>();
        private static readonly object CacheLock = new object();

        /// <summary>The name used for the couchbase area key</summary>
        public const string CacheArea = "FolderPath";

        /// <summary>The name used for the couchbase item key</summary>
        public const string CacheKey = "0";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new GlobalFolderPaths object, optionally with the supplied db connection.
        /// </summary>
        /// <param name="connection"></param>
        public GlobalFolderPaths(IDBConnection connection = null)
        {
            MetabaseConnectionString = GlobalVariables.MetabaseConnectionString;
            Get(connection);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// [readonly] Gets the Metabase connection string. This is grabbed from GlobalVariables at the start.
        /// </summary>
        public string MetabaseConnectionString
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the <see cref="AccountFolderPaths"/> for the given accountId. 
        /// </summary>
        /// <param name="accountId">The accountId.</param>
        /// <returns>An <see cref="AccountFolderPaths"/> object containing the paths for the account.</returns>
        public AccountFolderPaths GetBy(int accountId)
        {
            AccountFolderPaths folderPath;
            BackingCollection.TryGetValue(accountId, out folderPath);
            return folderPath;
        }

        /// <summary>
        /// Gets a sinlge folder path, given the accountId and <see cref="FilePathType"/>.
        /// </summary>
        /// <param name="accountId">The Id of the account to look in.</param>
        /// <param name="filePathType">The type of the folder path you want.</param>
        /// <returns></returns>
        public string GetSingleFolderPath(int accountId, FilePathType filePathType)
        {
            var accountFolderPaths = GetBy(accountId);
            if (accountFolderPaths == null)
            {
                throw new ArgumentOutOfRangeException("accountId", "The accountId you provided did not exist or have any paths assigned.");
            }

            return accountFolderPaths[filePathType];
        }

        #endregion

        #region Private Members


        /// <summary>
        /// Gets the data for the concurrent dictionary either from cache or the DB.
        /// If there is no cached data, then the DB will be accessed and the cache will be populated.
        /// </summary>
        /// <param name="connection"></param>
        private void Get(IDBConnection connection = null)
        {
            lock (CacheLock)
            {
                // sort the db results as they are added.
                IList<AccountFolderPaths> paths = CacheGet();
                
                // there is nothing in cache
                if (paths == null)
                {
                    // get from DB
                    using (var databaseConnection = connection ?? new DatabaseConnection(MetabaseConnectionString))
                    {
                        const string sql = "SELECT accountid, receiptpath, cardtemplatepath, offlineupdatepath, policyfilepath, cardocumentpath, logopath, attachmentspath FROM dbo.databases JOIN dbo.registeredusers on databaseID = dbserver";
                        paths = new List<AccountFolderPaths>();

                        databaseConnection.sqlexecute.Parameters.Clear();
                        using (IDataReader reader = databaseConnection.GetReader(sql))
                        {
                            // read contents
                            while (reader.Read())
                            {
                                int accountId = reader.GetInt32(reader.GetOrdinal("accountid"));
                                var dictionary = new Dictionary<FilePathType, string>
                                {
                                    { FilePathType.Receipt, PadFolderPathWithTrailingSlashIfNotNull(reader, "receiptpath") },
                                    { FilePathType.CardTemplate, PadFolderPathWithTrailingSlashIfNotNull(reader, "cardtemplatepath") },
                                    { FilePathType.OfflineUpdate, PadFolderPathWithTrailingSlashIfNotNull(reader, "offlineupdatepath") },
                                    { FilePathType.PolicyFile, PadFolderPathWithTrailingSlashIfNotNull(reader, "policyfilepath") },
                                    { FilePathType.CarDocument, PadFolderPathWithTrailingSlashIfNotNull(reader, "cardocumentpath") },
                                    { FilePathType.Logo, PadFolderPathWithTrailingSlashIfNotNull(reader, "logopath") },
                                    { FilePathType.Attachments, PadFolderPathWithTrailingSlashIfNotNull(reader, "attachmentspath") }
                                };

                                // add to the list
                                paths.Add(new AccountFolderPaths(accountId, dictionary));
                            }

                            // clean up
                            reader.Close();
                        }

                        // clean up
                        databaseConnection.sqlexecute.Parameters.Clear();
                        
                        // cache
                        CacheAdd(paths);
                    }

                }

                // add the items to the concurrent dictionary
                foreach (var accountFolderPaths in paths)
                {
                    BackingCollection.TryAdd(accountFolderPaths.AccountId, accountFolderPaths);
                }
            }
        }

        /// <summary>
        /// DRY method for grabbing the value out of a column and padding it with a trailing slash.
        /// </summary>
        /// <param name="reader">The reader that has access to the column.</param>
        /// <param name="columnName">The name of the column to grab the value from.</param>
        /// <returns>A string with a trailing slash, or null.</returns>
        private static string PadFolderPathWithTrailingSlashIfNotNull(IDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            var folderPath = reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            return !string.IsNullOrEmpty(folderPath) && folderPath.EndsWith("\\") ? folderPath : folderPath + "\\";
        }

        /// <summary>
        /// Attempts to grab the data from the cache.
        /// </summary>
        /// <returns>A sorted list.</returns>
        private IList<AccountFolderPaths> CacheGet()
        {
            var cache = new Cache();
            return (List<AccountFolderPaths>)cache.Get(0, CacheArea, CacheKey);
        }

        /// <summary>
        /// Adds the data to the cache.
        /// </summary>
        /// <param name="items">The sorted list.</param>
        private void CacheAdd(IList<AccountFolderPaths> items)
        {
            var cache = new Cache();
            cache.Add(0, CacheArea, CacheKey, items);
        }

        #endregion
    }
}
