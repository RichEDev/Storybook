namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders
{
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The budget holders repository.
    /// </summary>
    public class BudgetHoldersRepository
    {
        /// <summary>
        /// The populate budget holders.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<BudgetHolders> PopulateBudgetHolders(int? budgetHolderId = null, string sqlToExecute = "")
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT budgetholderid, budgetholder, description, employeeid FROM budgetholders";

            if (sqlToExecute == string.Empty)
            {
                sqlToExecute = strSQL;
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@budgetholderid", budgetHolderId);
            }
           List<BudgetHolders> budgetHolders = new List<BudgetHolders>();

            using (SqlDataReader reader = db.GetReader(strSQL))
            {
                #region set database columns
                int budgetholderIdOrdinal = reader.GetOrdinal("budgetholderid");
                int budgetholderOrdinal = reader.GetOrdinal("budgetholder");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int employeeidOrdinal = reader.GetOrdinal("employeeid");
                #endregion

                while (reader.Read())
                {
                    #region set values
                    BudgetHolders budgetHolder = new BudgetHolders();
                    budgetHolder.BudgetHolderId = reader.GetInt32(budgetholderIdOrdinal);
                    budgetHolder.BudgetHolder = reader.GetString(budgetholderOrdinal);
                    budgetHolder.Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal);
                    budgetHolder.EmployeeId = reader.GetInt32(employeeidOrdinal);
                    budgetHolder.Employee = EmployeesRepository.PopulateEmployee(employeeId: budgetHolder.EmployeeId, sqlToExecute: Employees.SqlItems).FirstOrDefault();
                    budgetHolders.Add(budgetHolder);
                    #endregion
                }
            }

            return budgetHolders;
        }

        /// <summary>
        /// The delete budget holder.
        /// </summary>
        /// <param name="budgeHolderId">
        /// The budge holder id.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        internal static void DeleteBudgetHolder(BudgetHolders budgeHolderToDelete, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));

            expdata.sqlexecute.Parameters.AddWithValue("@budgetholderid", budgeHolderToDelete.BudgetHolderId);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", budgeHolderToDelete.EmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteBudgetHolder");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The create budget holder.
        /// </summary>
        /// <param name="budgetHolders">
        /// The budget holders.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int CreateBudgetHolder(BudgetHolders budgetHolderToCreate, ProductType executingProduct)
        {            
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            budgetHolderToCreate.BudgetHolderId = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@budgetholderid", budgetHolderToCreate.BudgetHolderId);
            expdata.sqlexecute.Parameters.AddWithValue("@budgetholder", budgetHolderToCreate.BudgetHolder);
            expdata.sqlexecute.Parameters.AddWithValue("@userID", budgetHolderToCreate.EmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue(
                "@description",
                budgetHolderToCreate.Description.Length > 4000 ? budgetHolderToCreate.Description.Substring(0, 3999) : budgetHolderToCreate.Description);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", budgetHolderToCreate.Employee.employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("saveBudgetHolder");
            budgetHolderToCreate.BudgetHolderId = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            return budgetHolderToCreate.BudgetHolderId;
        }
    }
}
