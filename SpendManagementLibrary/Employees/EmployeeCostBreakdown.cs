using System;
using System.Globalization;

namespace SpendManagementLibrary.Employees
{
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    [Serializable]
    public class EmployeeCostBreakdown
    {
        private readonly IList<cDepCostItem> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeCostBreakdown";

        public EmployeeCostBreakdown(int accountID, int employeeID)
        {
            this._accountID = accountID;
            this._employeeID = employeeID;
            this._backingCollection = this.Get();
        }

        /// <summary>
        /// Override the default indexer for CostBreakdown.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="cDepCostItem"/> at the specified index.</returns>
        public cDepCostItem this[int index]
        {
            get
            {
                return this._backingCollection[index];
            }
        }

        public IList<cDepCostItem> Get(IDBConnection connection = null)
        {
            List<cDepCostItem> costBreakdown;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                costBreakdown = this.CacheGet();

                if (costBreakdown == null)
                {
                    costBreakdown = new List<cDepCostItem>();

                    databaseConnection.sqlexecute.Parameters.Clear();

                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);

                    const string SQL =
                        "select departmentid, costcodeid, percentused, projectcodeid from [employee_costcodes] where employeeid = @employeeid";

                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        while (reader.Read())
                        {
                            int departmentid = reader.IsDBNull(reader.GetOrdinal("departmentid")) == false
                                                   ? reader.GetInt32(reader.GetOrdinal("departmentid"))
                                                   : 0;
                            int costcodeid = reader.IsDBNull(reader.GetOrdinal("costcodeid")) == false
                                                 ? reader.GetInt32(reader.GetOrdinal("costcodeid"))
                                                 : 0;
                            int projectcodeid = reader.IsDBNull(reader.GetOrdinal("projectcodeid")) == false
                                                    ? reader.GetInt32(reader.GetOrdinal("projectcodeid"))
                                                    : 0;
                            int percentused = reader.GetInt32(reader.GetOrdinal("percentused"));
                            costBreakdown.Add(new cDepCostItem(departmentid, costcodeid, projectcodeid, percentused));
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();

                    this.CacheAdd(costBreakdown);

                }
            }

            return costBreakdown;
        }

        public void Add(bool newEmployee, cDepCostItem[] empbreakdown, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                if (this._backingCollection.Count == 0 && empbreakdown.Length == 0) 
                {
                    return;
                }

                if (empbreakdown.Length == 0)
                {
                    return;
                }

                // delete current breakdown
                if (newEmployee == false) 
                {
                    this.Clear();
                    this._backingCollection.Clear();
                }
            
                int i;

                cDepCostItem[] items = empbreakdown;

                for (i = 0; i < items.Length; i++)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);

                    if (items[i].departmentid == 0)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@departmentid", DBNull.Value);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@departmentid", items[i].departmentid);
                    }

                    if (items[i].costcodeid == 0)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@costcodeid", DBNull.Value);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@costcodeid", items[i].costcodeid);
                    }

                    if (items[i].projectcodeid == 0)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@projectcodeid", DBNull.Value);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@projectcodeid", items[i].projectcodeid);
                    }

                    databaseConnection.sqlexecute.Parameters.AddWithValue("@percentused", items[i].percentused);
                    const string SQL = "insert into [employee_costcodes] (employeeid, departmentid, costcodeid, percentused, projectcodeid) values (@employeeid,@departmentid,@costcodeid,@percentused,@projectcodeid)";

                    databaseConnection.ExecuteSQL(SQL);
                    databaseConnection.sqlexecute.Parameters.Clear();
                    if (!this._backingCollection.Contains(items[i]))
                    {
                        this._backingCollection.Add(items[i]);
                    }
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.CacheDelete();
        }

        public void Clear(IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                const string SQL = "delete from [employee_costcodes] where employeeid = @employeeid";
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.CacheDelete();
        }

        public IEnumerator<cDepCostItem> GetEnumerator()
        {
            return this._backingCollection.GetEnumerator();
        }

        public cDepCostItem[] ToArray()
        {
            return ((List<cDepCostItem>)this._backingCollection).ToArray();
        }

        private void CacheAdd(List<cDepCostItem> costBreakdown)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), costBreakdown);
        }

        private List<cDepCostItem> CacheGet()
        {
            Cache cache = new Cache();
            return (List<cDepCostItem>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
