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
    public class EmployeeGridSorts
    {
        private readonly IDictionary<Grid, cGridSort> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeGridSorts";

        public EmployeeGridSorts(int accountID, int employeeID)
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

        public cGridSort GetBy(Grid grid)
        {
            cGridSort gridSort;
            this._backingCollection.TryGetValue(grid, out gridSort);
            return gridSort;
        }

        public void Add(Grid grid, string columnname, byte direction)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this._accountID));

            string sql = "delete from default_sorts where gridid = @gridid and employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@gridid", (int)grid);
            expdata.ExecuteSQL(sql);


            sql = "insert into default_sorts (employeeid, gridid, columnname, defaultorder) values (@employeeid, @gridid, @columnname, @defaultorder)";
            expdata.sqlexecute.Parameters.AddWithValue("@columnname", columnname);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultorder", direction);
            expdata.ExecuteSQL(sql);
            expdata.sqlexecute.Parameters.Clear();

            this._backingCollection.Remove(grid);
            this._backingCollection.Add(grid, new cGridSort(grid, columnname, direction));

            this.CacheDelete();
        }

        public void Add(Grid grid, string columnname, string direction)
        {
            this.Add(grid, columnname, direction.ToLower() == "asc" ? (byte)1 : (byte)2);
        }

        private Dictionary<Grid, cGridSort> Get(IDBConnection connection = null)
        {
            Dictionary<Grid, cGridSort> lst;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                lst = this.CacheGet();

                if (lst == null)
                {
                    lst = new Dictionary<Grid, cGridSort>();
                    databaseConnection.sqlexecute.Parameters.Clear();
                
                    const string SQL =
                        "select gridid, columnname, defaultorder from default_sorts where employeeid = @employeeid";

                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        databaseConnection.sqlexecute.Parameters.Clear();
                        while (reader.Read())
                        {
                            Grid grid = (Grid)reader.GetInt32(reader.GetOrdinal("gridid"));
                            string columnname = reader.GetString(reader.GetOrdinal("columnname"));
                            byte sortorder = reader.GetByte(reader.GetOrdinal("defaultorder"));
                            cGridSort clssort = new cGridSort(grid, columnname, sortorder);
                            lst.Add(grid, clssort);
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();

                    this.CacheAdd(lst);
                }
            }

            return lst;
        }

        private void CacheAdd(Dictionary<Grid, cGridSort> gridSorts)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), gridSorts);
        }

        private Dictionary<Grid, cGridSort> CacheGet()
        {
            Cache cache = new Cache();
            return (Dictionary<Grid, cGridSort>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
