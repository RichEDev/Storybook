using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Class for concurrent user management
    /// </summary>
    public class cConcurrentUsers
    {
        private int nAccountID;
        private int nEmployeeID;
        private struct accessManagementElement
        {
            public Guid manageID;
            public int employeeID;
            public DateTime lastActivityDate;
        }
        private List<accessManagementElement> CUList;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="accountID">Metabase account ID</param>
        /// <param name="employeeID">Employee ID</param>
        public cConcurrentUsers(int accountID, int employeeID)
        {
            nAccountID = accountID;
            nEmployeeID = employeeID;
        }

        /// <summary>
        /// Get a ManageID for a given employee
        /// </summary>
        /// <param name="employeeID">Employee ID to obtain an ID for</param>
        /// <returns>Returns unique identifier if exists or empty if not present</returns>
        public Guid getManageID(int employeeID)
        {
            if (CUList == null)
            {
                CUList = LoadCUsers();
            }

            Guid retID = Guid.Empty;
            foreach (accessManagementElement ame in CUList)
            {
                if (ame.employeeID == employeeID)
                {
                    retID = ame.manageID;
                    break;
                }
            }

            return retID;
        }

        /// <summary>
        /// Does an employee exist in the current concurrent users list
        /// </summary>
        /// <param name="employeeID">EmployeeID to check existence of</param>
        /// <returns>TRUE if present, otherwise FALSE</returns>
        public bool Exists(int employeeID)
        {
            if (CUList == null)
            {
                CUList = LoadCUsers();
            }

            bool ret = false;
            foreach (accessManagementElement ame in CUList)
            {
                if (ame.employeeID == employeeID)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Loads the users current logged into a collection
        /// </summary>
        /// <returns></returns>
        private List<accessManagementElement> LoadCUsers()
        {
            List<accessManagementElement> retList = new List<accessManagementElement>();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(nAccountID));

            //                      0           1           2
            string sql = "select manageID, employeeID, lastActivity from accessManagement";
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    accessManagementElement me = new accessManagementElement();
                    me.manageID = reader.GetGuid(0);
                    me.employeeID = reader.GetInt32(1);
                    me.lastActivityDate = DateTime.Now;
                    if (!reader.IsDBNull(2))
                    {
                        me.lastActivityDate = reader.GetDateTime(2);
                    }
                    retList.Add(me);
                }
                reader.Close();
            }

            return retList;
        }

        /// <summary>
        /// Register a user in the concurrent user management
        /// </summary>
        /// <returns>Unique management ID for the concurrent user</returns>
        public Guid LogonUser()
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(nAccountID));
            cAccounts accs = new cAccounts();
            cAccount acc = accs.GetAccountByID(nAccountID);

            int maxConcurrentUsers = acc.LicencedUsers;
            int curCUCount = Count;
            Guid retManageID;
            if (Exists(nEmployeeID))
            {
                retManageID = getManageID(nEmployeeID);
            }
            else
            {
                if (curCUCount >= maxConcurrentUsers)
                    return Guid.Empty;

                retManageID = Guid.NewGuid();
            }

            db.sqlexecute.Parameters.Clear();
            db.sqlexecute.Parameters.AddWithValue("@manageID", retManageID);
            db.sqlexecute.Parameters.AddWithValue("@employeeID", nEmployeeID);
            db.ExecuteProc("saveCUActivity");

            CUList = null;
            return retManageID;
        }

        /// <summary>
        /// Removes a user from the managed concurrent users
        /// </summary>
        /// <param name="accessManageID">Concurrent user management ID to remove</param>
        /// <param name="accountID">Metabase account ID</param>
        public static void LogoffUser(Guid accessManageID, int accountID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountID));
            db.sqlexecute.Parameters.AddWithValue("@manageID", accessManageID);
            db.ExecuteProc("deleteCU");
            
            return;
        }

        /// <summary>
        /// Logs activity against a managed concurrent user record
        /// </summary>
        /// <param name="accountID">Metabase account ID</param>
        /// <param name="employeeID">Employee ID to log the activity against</param>
        /// <param name="previousLastActivity">The last time this employee's activity was logged CurrentUserBase.LastActivity</param>
        public static bool CUActivityHit(int accountID, int employeeID, DateTime? previousLastActivity)
        {
            if (previousLastActivity.HasValue == false || (DateTime.Now - previousLastActivity.Value).TotalSeconds >= 60)
            {
                cAccounts clsAccounts = new cAccounts();
                cAccount account = clsAccounts.GetAccountByID(accountID);

                if (account.LicencedUsers > 0)
                {
                    DBConnection db = new DBConnection(cAccounts.getConnectionString(accountID));
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
                    db.ExecuteProc("CUActivityHit");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the current number of concurrent users logged on to the application
        /// </summary>
        public int Count
        {
            get 
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(nAccountID));
                int cuCount = 0;

                string sql = "select count(manageID) from accessManagement where lastActivity > DATEADD(mi, -60, GETDATE());";
                cuCount = db.getcount(sql);
                
                return cuCount;
            }
        }
    }
}
