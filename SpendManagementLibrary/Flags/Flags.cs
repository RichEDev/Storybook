namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The flags class.
    /// </summary>
    public class Flags
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Flags"/> class. 
        /// Initialises the flag class
        /// </summary>
        /// <param name="accountid">
        /// The account ID of the current user
        /// </param>
        public Flags(int accountid)
        {
            this.AccountID = accountid;
        }

        /// <summary>
        /// Deletes the receipt not attached flags from an expense
        /// </summary>
        /// <param name="expenseID">
        /// The expense to remove the flags from
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void DeleteReceiptNotAttachedFlag(int expenseID, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseID);
                databaseConnection.ExecuteProc("DeleteReceiptNotAttachedFlag");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        #region properties

        /// <summary>
        /// Gets the Account ID of the current user
        /// </summary>
        public int AccountID { get; private set; }

        #endregion

        /// <summary>
        /// Gets the expense information of expenses associated with a flagged item.
        /// </summary>
        /// <param name="associatedExpenses">
        /// The ids of the expenses to retrieve.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<AssociatedExpense> GetAssociatedExpenses(List<int> associatedExpenses, IDBConnection connection = null)
        {
            if (associatedExpenses.Count == 0)
            {
                return new List<AssociatedExpense>();
            }

            List<AssociatedExpense> expenses = new List<AssociatedExpense>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Add("@associatedExpenses", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in associatedExpenses)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@associatedExpenses"].Value = integers;

                using (
                    IDataReader reader =
                        databaseConnection.GetReader("GetFlagAssociatedExpenses", CommandType.StoredProcedure))
                {
                    int expenseIDOrd = reader.GetOrdinal("expenseid");
                    int nameOrd = reader.GetOrdinal("name");
                    int dateOrd = reader.GetOrdinal("date");
                    int totalOrd = reader.GetOrdinal("total");
                    int subcatOrd = reader.GetOrdinal("subcat");
                    int refnumOrd = reader.GetOrdinal("refnum");
                    int symbolOrd = reader.GetOrdinal("currencySymbol");
                    while (reader.Read())
                    {
                        string claimName = reader.GetString(nameOrd);
                        DateTime date = reader.GetDateTime(dateOrd);
                        string subcat = reader.GetString(subcatOrd);
                        string refnum = reader.GetString(refnumOrd);
                        string symbol = reader.GetString(symbolOrd);
                        decimal total = reader.GetDecimal(totalOrd);
                        int expenseid = reader.GetInt32(expenseIDOrd);

                        expenses.Add(new AssociatedExpense(claimName, date.ToShortDateString(), refnum, symbol + total.ToString("####,###,##0.00"), subcat, expenseid));
                    }

                    reader.Close();
                    databaseConnection.sqlexecute.Parameters.Clear();
                }
            }

            return expenses;
        }
    }
}
