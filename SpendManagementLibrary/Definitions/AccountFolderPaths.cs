namespace SpendManagementLibrary.Definitions
{
    using System.Collections.Generic;
    using System.Linq;
    using System;

    using BusinessLogic.FilePath;

    /// <summary>
    /// Represents the storage folders for an account.
    /// These are taken from metabase.dbo.databases, and correspond to <see cref="FilePathType"/> members.
    /// Currently read-only.
    /// </summary>
    [Serializable]
    public class AccountFolderPaths
    {
        #region Private Fields
        
        private readonly Dictionary<FilePathType, string> _backingCollection;
        private readonly int _accountId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new AccountFolderPaths object. This should only really be called from GlobalFolderPaths for the moment.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="filePathsByType"></param>
        internal AccountFolderPaths(int accountId, Dictionary<FilePathType, string> filePathsByType)
        {
            _accountId = accountId;
            _backingCollection = filePathsByType;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the number of items in the backing collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _backingCollection.Count;
            }
        }

        /// <summary>
        /// Gets the Id of the account that these folder paths belong to.
        /// </summary>
        public int AccountId
        {
            get { return _accountId; }
        }

        /// <summary>
        /// Provides array style access for the underlying dictionary.
        /// </summary>
        /// <param name="index">The <see cref="FilePathType"/> to look up the path for.</param>
        /// <returns></returns>
        public string this[FilePathType index]
        {
            get
            {
                return _backingCollection[index];
            }
        }

        /// <summary>
        /// Indicates if the underlying dictionary contains an entry for the provided key.
        /// </summary>
        /// <param name="filePathType">The key.</param>
        /// <returns>A bool indicating whether an entry exists.</returns>
        public bool Contains(FilePathType filePathType)
        {
            return _backingCollection.Keys.Contains(filePathType);
        }

        /// <summary>
        /// Gets the enumerator for the underlying collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<FilePathType, string>> GetEnumerator()
        {
            return _backingCollection.GetEnumerator();
        }

        #endregion
    }
}
