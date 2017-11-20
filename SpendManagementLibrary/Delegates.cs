namespace SpendManagementLibrary
{
    using System.Data;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    public class Delegates
    {
        private readonly ICurrentUserBase currentUser;

        public Delegates(ICurrentUserBase currentUser)
        {
            this.currentUser = currentUser;
        }

        public DataSet getProxies(int employeeid, IDBConnection connection = null)
        {
            DataSet ds;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                const string SQL = "select employees.employeeid, username, title + ' ' + firstname + ' ' + surname as [empname] from employees inner join employee_proxies on employees.employeeid = employee_proxies.proxyid where employee_proxies.employeeid = @employeeid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                ds = databaseConnection.GetDataSet(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return ds;
        }

        public void assignProxy(int assigningid, int employeeid, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", assigningid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);

                string sql = "select count(*) from employee_proxies where employeeid = @employeeid and proxyid = @proxyid";
                int count = databaseConnection.ExecuteScalar<int>(sql);
                if (count != 0)
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    return;
                }

                sql = "insert into employee_proxies (employeeid, proxyid) values (@employeeid, @proxyid)";

                databaseConnection.ExecuteSQL(sql);
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        public void removeProxy(int removingid, int employeeid, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                const string SQL = "delete from employee_proxies where employeeid = @employeeid and proxyid = @proxyid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", removingid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
                databaseConnection.ExecuteSQL(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        public bool isProxy(int employeeid, IDBConnection connection = null)
        {
            int count;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                const string SQL = "select count(*) from employee_proxies where proxyid = @proxyid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);
                count = databaseConnection.ExecuteScalar<int>(SQL);
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return count != 0;
        }

        public ListItem[] createProxyDropDown(int employeeid, IDBConnection connection = null)
        {
            ListItem[] tempitems;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.currentUser.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                databaseConnection.sqlexecute.Parameters.AddWithValue("@proxyid", employeeid);

                string strsql = "select count(*) from employees where archived = 0 and " +
                                " employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";
                int count = databaseConnection.ExecuteScalar<int>(strsql);

                tempitems = new ListItem[count];
                strsql = "select employeeid, [surname] + ', ' + [title] + ' ' + firstname as empname from employees where archived = 0 and " +
                         " employeeid in (select employeeid from employee_proxies where proxyid = @proxyid)";

                using (IDataReader empreader = databaseConnection.GetReader(strsql))
                {
                    int i = 0;
                    while (empreader.Read())
                    {
                        tempitems[i] = new ListItem { Text = empreader.GetString(empreader.GetOrdinal("empname")), Value = empreader.GetInt32(empreader.GetOrdinal("employeeid")).ToString(CultureInfo.InvariantCulture) };
                        if (empreader.GetInt32(empreader.GetOrdinal("employeeid")) == employeeid)
                        {
                            tempitems[i].Selected = true;
                        }

                        i++;
                    }

                    empreader.Close();
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return tempitems;
        }
    }
}
