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
    public class EmployeeAccessRoles
    {
        private readonly Dictionary<int, List<int>> _backingCollection;

        private readonly int _accountID;

        private readonly int _employeeID;

        public const string CacheArea = "employeeAccessRoles";

        public EmployeeAccessRoles(int accountID, int employeeID)
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

        public Dictionary<int, List<int>> AllAccessRoles
        {
            get { return this._backingCollection; }
        }

        public List<int> GetBy(int subAccountID)
        {
            if (this._backingCollection.Count > 0)
            {
                if (this._backingCollection.ContainsKey(subAccountID))
                {
                    return this._backingCollection[subAccountID];
                }
            }
            return new List<int>();
        }

        public void Add(List<int> roles, int? subaccountid, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                foreach (int accessRoleID in roles)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@subaccountid", subaccountid);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeID);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleID);

                    if (currentUser != null)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                        if (currentUser.isDelegate)
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                        }
                        else
                        {
                            databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                        }
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }

                    databaseConnection.ExecuteProc("saveEmployeeAccessRoles");
                    databaseConnection.sqlexecute.Parameters.Clear();
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this._employeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);

                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                databaseConnection.ExecuteProc("recordEmployeeModified");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.CacheDelete();
        }

        public int Remove(int roleid, int? subaccountid, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int returnvalue;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@accessRoleID", roleid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@subAccountID", subaccountid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("deleteEmployeeAccessRole");
                returnvalue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.CacheDelete();

            return returnvalue;
        }

        private Dictionary<int, List<int>> Get(IDBConnection connection = null)
        {
            Dictionary<int, List<int>> retSubAccountAccessRoles;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountID)))
            {
                retSubAccountAccessRoles = this.CacheGet();

                if (retSubAccountAccessRoles == null)
                {
                    retSubAccountAccessRoles = new Dictionary<int, List<int>>();

                    const string SQL = "SELECT employeeID, accessRoleID, subAccountID , ExclusionType FROM dbo.employeeAccessRoles e inner join accessRoles a on e.accessRoleID = a.roleID  WHERE employeeID=@employeeID order by ExclusionType";
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", this._employeeID);
                    using (IDataReader reader = databaseConnection.GetReader(SQL))
                    {
                        while (reader.Read())
                        {
                            int subAccountID = reader.IsDBNull(reader.GetOrdinal("subAccountID"))
                                                   ? 0
                                                   : reader.GetInt32(reader.GetOrdinal("subAccountID"));
                            int accessRoleID = reader.GetInt32(reader.GetOrdinal("accessRoleID"));

                            List<int> lstAccessRoles = retSubAccountAccessRoles.ContainsKey(subAccountID)
                                                           ? retSubAccountAccessRoles[subAccountID]
                                                           : new List<int>();

                            if (!lstAccessRoles.Contains(accessRoleID))
                            {
                                lstAccessRoles.Add(accessRoleID);
                            }

                            if (retSubAccountAccessRoles.ContainsKey(subAccountID))
                            {
                                retSubAccountAccessRoles[subAccountID] = lstAccessRoles;
                            }
                            else
                            {
                                retSubAccountAccessRoles.Add(subAccountID, lstAccessRoles);
                            }
                        }

                        reader.Close();
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();

                    this.CacheAdd(retSubAccountAccessRoles);
                }
            }

            return retSubAccountAccessRoles;
        }

        private void CacheAdd(Dictionary<int, List<int>> accessRoles)
        {
            Cache cache = new Cache();
            cache.Add(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture), accessRoles);
        }

        private Dictionary<int, List<int>> CacheGet()
        {
            Cache cache = new Cache();
            return (Dictionary<int, List<int>>)cache.Get(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }

        private void CacheDelete()
        {
            Cache cache = new Cache();
            cache.Delete(this._accountID, CacheArea, this._employeeID.ToString(CultureInfo.InvariantCulture));
        }
    }
}
