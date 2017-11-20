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
    public class EmployeeEmailNotifications
    {
        private readonly List<int> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeEmailNotifications";
        
        public EmployeeEmailNotifications(int accountID, int employeeID)
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

        public List<int> EmailNotificationIDs
        {
            get
            {
                return this._backingCollection;
            }
        } 

        public bool Contains(int emailNotificationID)
        {
            return this._backingCollection.Contains(emailNotificationID);
        }

        public int this[int index]
        {
            get
            {
                return this._backingCollection[index];
            }
        }

        public void Remove(int key)
        {
            this._backingCollection.Remove(key);

            this.CacheDelete();
        }

        public void Clear()
        {
            this._backingCollection.Clear();

            this.CacheDelete();
        }

        private List<int> Get(IDBConnection connection = null)
        {
            List<int> lstEmailNotifications;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                lstEmailNotifications = this.CacheGet();

                if (lstEmailNotifications == null)
                {
                    lstEmailNotifications = new List<int>();

                    const string SQL = "SELECT emailNotificationID FROM emailNotificationLink WHERE employeeID=@employeeID";

                    databaseConnection.sqlexecute.Parameters.Clear();
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeID);

                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        while (reader.Read())
                        {
                            lstEmailNotifications.Add(reader.GetInt32(reader.GetOrdinal("emailNotificationID")));
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();

                    this.CacheAdd(lstEmailNotifications);
                }
            }

            return lstEmailNotifications;
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
