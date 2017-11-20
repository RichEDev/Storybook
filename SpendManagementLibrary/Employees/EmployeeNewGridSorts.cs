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
    public class EmployeeNewGridSorts
    {
        private readonly IDictionary<string, cNewGridSort> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeNewGridSorts";

        public EmployeeNewGridSorts(int accountID, int employeeID)
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

        public cNewGridSort GetBy(string gridName)
        {
            cNewGridSort gridSort;
            this._backingCollection.TryGetValue(gridName, out gridSort);
            return gridSort;
        }

        /// <summary>
        /// Update a grid sort for an employee
        /// </summary>
        /// <param name="gridName"></param>
        /// <param name="sort"></param>
        public void Add(string gridName, cNewGridSort sort)
        {
            if (this._backingCollection.ContainsKey(gridName))
            {
                this._backingCollection[gridName] = sort;
            }
            else
            {
                this._backingCollection.Add(gridName, sort);
            }

            this.CacheDelete();
        }

        private IDictionary<string, cNewGridSort> Get(IDBConnection connection = null)
        {
            Dictionary<string, cNewGridSort> lst;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                lst = this.CacheGet();

                if (lst == null)
                {
                    lst = new Dictionary<string, cNewGridSort>();
                    databaseConnection.sqlexecute.Parameters.Clear();
                    const string SQL = "select gridid, sortedcolumn, sortorder, sortjoinviaid from employeeGridSortOrders where employeeid = @employeeid";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        int grididOrd = reader.GetOrdinal("gridid");
                        int sortedcolOrd = reader.GetOrdinal("sortedcolumn");
                        int sortorderOrd = reader.GetOrdinal("sortorder");
                        int joinViaIDOrd = reader.GetOrdinal("sortJoinViaID");

                        while (reader.Read())
                        {
                            string gridid = reader.GetString(grididOrd);
                            Guid sortedcolumn = reader.GetGuid(sortedcolOrd);
                            SortDirection sortdirection = (SortDirection)reader.GetByte(sortorderOrd);
                            int sortJoinViaID = 0;
                            if (!reader.IsDBNull(joinViaIDOrd))
                            {
                                sortJoinViaID = reader.GetInt32(joinViaIDOrd);
                            }
                            cNewGridSort clssort = new cNewGridSort(gridid, sortedcolumn, sortdirection, sortJoinViaID);
                            lst.Add(gridid, clssort);
                        }
                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    this.CacheAdd(lst);
                }
            }

            return lst;
        }

        private void CacheAdd(Dictionary<string, cNewGridSort> gridSorts)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), gridSorts);
        }

        private Dictionary<string, cNewGridSort> CacheGet()
        {
            Cache cache = new Cache();
            return (Dictionary<string, cNewGridSort>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
