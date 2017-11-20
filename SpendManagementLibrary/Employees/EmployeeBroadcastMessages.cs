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
    public class EmployeeBroadcastMessages
    {
        private readonly IList<int> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeBroadcasts";

        public EmployeeBroadcastMessages(int accountID, int employeeID)
        {
            this._accountID = accountID;
            this._employeeID = employeeID;
            this._backingCollection = this.Get();
        }

        public int Count
        {
            get
            {
                return this._backingCollection.Count;
            }
        }

        public int this[int index]
        {
            get
            {
                return this._backingCollection[index];
            }
        }

        public bool Contains(int broadcastMessageID)
        {
            return this._backingCollection.Contains(broadcastMessageID);
        }

        public void Add(int broadcastMessageID, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                const string SQL = "insert into employee_readbroadcasts (employeeid, broadcastid, createdon, createdby) values (@employeeid, @broadcastid, @createdon, @userid)";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@broadcastid", broadcastMessageID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.UtcNow);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", this._employeeID);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }
            if (!this._backingCollection.Contains(broadcastMessageID))
            {
                this._backingCollection.Add(broadcastMessageID);
            }

            this.CacheDelete();
        }

        private IList<int> Get(IDBConnection connection = null)
        {
            List<int> temp;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                temp = this.CacheGet();

                if (temp == null)
                {
                    temp = new List<int>();

                    databaseConnection.sqlexecute.Parameters.Clear();

                    const string SQL = "select broadcastid from employee_readbroadcasts where employeeid = @employeeid";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {

                        while (reader.Read())
                        {
                            temp.Add(reader.GetInt32(0));
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();

                    this.CacheAdd(temp);
                }
            }

            return temp;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this._backingCollection.GetEnumerator();
        }

        private void CacheAdd(List<int> broadcastMessages)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), broadcastMessages);
        }

        private List<int> CacheGet()
        {
            Cache cache = new Cache();
            return (List<int>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
