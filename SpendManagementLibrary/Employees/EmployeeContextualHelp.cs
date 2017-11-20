namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    using Helpers;
    using Interfaces;

    using Utilities.DistributedCaching;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EmployeeContextualHelp
    {
        /// <summary>
        /// The key that this instance should prefix for its cache key
        /// </summary>
        public const string CacheArea = "employeeContextualHelp";

        /// <summary>
        /// The account ID to which this instance's <see cref="_employeeId"/> belongs
        /// </summary>
        private readonly int _accountId;
        
        /// <summary>
        /// A collection of contextual help IDs that should not be shown.
        /// </summary>
        private readonly List<int> _backingCollection;

        /// <summary>
        /// The employeeId for the <see cref="Employee"/> this instance is related to.
        /// </summary>
        private readonly int _employeeId;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeContextualHelp"/> class.
        /// </summary>
        /// <param name="accountId">The account ID this employee belongs too.</param>
        /// <param name="employeeId">The employee ID.</param>
        public EmployeeContextualHelp(int accountId, int employeeId)
        {
            this._accountId = accountId;
            this._employeeId = employeeId;
            this._backingCollection = this.Get();
        }

        /// <summary>
        /// Reset all contextual help to show.
        /// </summary>
        /// <param name="connection">The connection to use to access the database.</param>
        public void Clear(IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                const string SQL = "DELETE FROM EmployeeContextualHelp WHERE EmployeeId = @employeeId";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", this._employeeId);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this._backingCollection.Clear();
            this.CacheDelete();
        }

        /// <summary>
        /// Removes an entry to mark a contextual help to not display.
        /// </summary>
        /// <param name="contextualHelpId">The contextual help ID to remove.</param>
        /// <param name="connection">The connection to use to access the database.</param>
        public void Remove(int contextualHelpId, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                const string SQL = "DELETE FROM EmployeeContextualHelp WHERE EmployeeId = @employeeId AND ContextualHelpId = @contextualHelpId";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", this._employeeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@contextualHelpId", contextualHelpId);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this._backingCollection.Remove(contextualHelpId);
            this.CacheDelete();
        }

        /// <summary>
        /// Whether or not to show the contextual help for the specified <paramref name="contextualHelpId"/>.
        /// </summary>
        /// <param name="contextualHelpId">The contextual help ID to check for.</param>
        /// <returns>True if the help should be shown</returns>
        public bool Show(int contextualHelpId)
        {
            return !this.Contains(contextualHelpId);
        }

        /// <summary>
        /// Add a contextual help ID to the database to prevent it being shown to this <see cref="Employee"/>.
        /// </summary>
        /// <param name="contextualHelpId">The ID to insert into the database.</param>
        /// <param name="connection">The connection to use to access the database.</param>
        public void Add(int contextualHelpId, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                if (this._backingCollection.Contains(contextualHelpId) == false)
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string SQL = "INSERT INTO EmployeeContextualHelp (EmployeeId, ContextualHelpID) VALUES (@employeeId, @contextualHelpID)";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", this._employeeId);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@contextualHelpID", contextualHelpId);
                    databaseConnection.ExecuteSQL(SQL);
                    databaseConnection.sqlexecute.Parameters.Clear();

                    if (!this._backingCollection.Contains(contextualHelpId))
                    {
                        this._backingCollection.Add(contextualHelpId);
                    }
                }
            }

            this.CacheDelete();
        }

        /// <summary>
        /// Add the collection to cache.
        /// </summary>
        /// <param name="contextualHelpIDs">A collection of which contextual help IDs this <see cref="Employee"/> does not wish to be shown.</param>
        private void CacheAdd(List<int> contextualHelpIDs)
        {
            Cache cache = new Cache();
            cache.Add(this._accountId, CacheArea, this._employeeId.ToString(CultureInfo.InvariantCulture), contextualHelpIDs);
        }

        /// <summary>
        /// Remove the collection from cache.
        /// </summary>
        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountId, CacheArea, this._employeeId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Retrieve the collection from cache.
        /// </summary>
        /// <returns>A collection of which contextual help IDs have been selected to not show for this <see cref="Employee"/></returns>
        private List<int> CacheGet()
        {
            Cache cache = new Cache();
            return (List<int>)cache.Get(this._accountId, CacheArea, this._employeeId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Checks if this employee has previously stated they do not wish to see contextual help for <paramref name="contextualHelpId"/>.
        /// </summary>
        /// <param name="contextualHelpId">The contextual help ID to check for.</param>
        /// <returns>true if the specified <paramref name="contextualHelpId"/>.</returns>
        private bool Contains(int contextualHelpId)
        {
            return this._backingCollection.Contains(contextualHelpId);
        }

        /// <summary>
        /// Retrieves a of contextual help IDs which should not be displayed to the <see cref="Employee"/>.
        /// </summary>
        /// <param name="connection">The connection to use to access the database.</param>
        /// <returns>A collection of contextual help IDs which should not be displayed to the <see cref="Employee"/>.</returns>
        private List<int> Get(IDBConnection connection = null)
        {
            List<int> hiddenContextualHelpIDs;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                hiddenContextualHelpIDs = this.CacheGet();

                if (hiddenContextualHelpIDs == null)
                {
                    hiddenContextualHelpIDs = new List<int>();
                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string SQL = "SELECT contextualHelpID FROM EmployeeContextualHelp WHERE EmployeeId = @employeeId";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", this._employeeId);

                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        while (reader.Read())
                        {
                            hiddenContextualHelpIDs.Add(reader.GetInt32(0));
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    this.CacheAdd(hiddenContextualHelpIDs);
                }
            }

            return hiddenContextualHelpIDs;
        }
    }
}