namespace SpendManagementLibrary.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    [Serializable]
    public class EmployeeSubCategories
    {
        private readonly List<int> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeSubCategories";

        public List<int> SubCategories
        {
            get
            {
                return this._backingCollection;
            }
        } 

        public EmployeeSubCategories(int accountID, int employeeID)
        {
            this._accountID = accountID;
            this._employeeID = employeeID;
            this._backingCollection = this.Get();
        }

        private List<int> Get(IDBConnection connection = null)
        {
            List<int> selectedSubcats;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                selectedSubcats = this.CacheGet();

                if (selectedSubcats == null)
                {
                    selectedSubcats = new List<int>();
                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string SQL = "SELECT subcatid FROM additems WHERE employeeID=@employeeID";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeID);

                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        while (reader.Read())
                        {
                            selectedSubcats.Add(reader.GetInt32(0));
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    this.CacheAdd(selectedSubcats);
                }
            }

            return selectedSubcats;
        }

        public bool Contains(int subCatID)
        {
            return this._backingCollection.Contains(subCatID);
        }

        public void Remove(int subCatID, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                const string SQL = "delete from additems where employeeid = @employeeid and subcatid = @subcatid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatid", subCatID);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }
            this._backingCollection.Remove(subCatID);
            this.CacheDelete();
        }

        public void Clear(bool clearCache, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                const string SQL = "delete from additems where employeeid = @employeeid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }
            this._backingCollection.Clear();

            if (clearCache)
            {
                this.CacheDelete();
            }
        }

        /// <summary>
        /// Add multiple selected sub categories at once.
        /// </summary>
        /// <param name="subCatIDs">An array of selected sub category ids.</param>
        public void AddMultiple(int[] subCatIDs)
        {
            this.Clear(false);

            foreach (int subcatid in subCatIDs)
            {
                this.Add(subcatid, false);
            }

            this.CacheDelete();
        }

        public void Add(int subCatID)
        {
            this.Add(subCatID, true);
        }

        private void Add(int subCatID, bool clearCache, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                if (this._backingCollection.Contains(subCatID) == false)
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string SQL = "insert into additems (employeeid, subcatid) values (@employeeid, @subcatid)";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatid", subCatID);
                    databaseConnection.ExecuteSQL(SQL);
                    databaseConnection.sqlexecute.Parameters.Clear();
                    if (!this._backingCollection.Contains(subCatID))
                    {
                        this._backingCollection.Add(subCatID);
                    }
                }
            }

            if (clearCache)
            {
                this.CacheDelete();
            }
        }

        private void CacheAdd(List<int> selectedSubCats)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), selectedSubCats);
        }

        private List<int> CacheGet()
        {
            Cache cache = new Cache();
            return (List<int>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        internal void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
