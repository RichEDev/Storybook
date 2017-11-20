namespace Spend_Management.shared.code
{
    using System.Data;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using System;
    using Utilities.DistributedCaching;
    using System.Globalization;
    /// <summary>
    /// Get Authoriser Level Detail
    /// </summary>
    public class AuthoriserLevelDetail
    
    {

        /// <summary>
        /// current user
        /// </summary>
        CurrentUser user;
        /// <summary>
        /// AccountId
        /// </summary>
        private int accountid;
        public int AccountId
        {
            get { return accountid; }
        }
        /// <summary>
        /// AuthoriserLevelDetailId of AuthoriserLevelDetails
        /// </summary>
        public int AuthoriserLevelDetailId { get; set; }
        
        /// <summary>
        /// Amount of AuthoriserLevelDetails
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Description of AuthoriserLevelDetails
        /// </summary>
        public string Description { get; set; }

        public AuthoriserLevelDetail()
        {
            user = cMisc.GetCurrentUser();
        }

        /// <summary>
        /// Delete Authoriser Level Detail based on authoriserLevelDetailId
        /// </summary>
        /// <param name="authoriserLevelDetailId">authoriserLevelDetailId of AuthoriserLevelDetails</param>
        /// <param name="IsAssignAuthoriserLevel">IsAssignAuthoriserLevel is boolean</param>
        /// <returns></returns>
        public int DeleteAuthoriserLevelDetail(int authoriserLevelDetailId, bool IsAssignAuthoriserLevel)
        {
            int result = 1;
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", authoriserLevelDetailId);
                expdata.sqlexecute.Parameters.Add("@IsAuthoriserLevelAssigned", IsAssignAuthoriserLevel);
                expdata.sqlexecute.Parameters.Add("@employeeID", this.user.EmployeeID);
                if (this.user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", this.user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.Add("@RowCount", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@RowCount"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.DeleteAuthoriserLevelDetail");
                result = (int)expdata.sqlexecute.Parameters["@RowCount"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            return result;
        }

        /// <summary>
        /// Update Employee table For Default Authoriser based on employeeId
        /// </summary>
        /// <param name="employeeId">employeeId of Employees</param>
        /// <returns></returns>
        public int UpdateEmployeeForDefaultAuthoriser(int employeeId)
        {
            int result = 0;
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@EmployeeId", employeeId);
                expdata.sqlexecute.Parameters.Add("@RowCount", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@RowCount"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.UpdateEmployeeForDefaultAuthoriser");
                result = (int)expdata.sqlexecute.Parameters["@RowCount"].Value;
                if (result > 0)
                {
                    Cache caching = new Cache();
                    caching.Delete(user.AccountID, "employee", employeeId.ToString(CultureInfo.InvariantCulture));
                }
                expdata.sqlexecute.Parameters.Clear();
            }
           
            return result;
        }


        /// <summary>
        /// Save Authoriser Level Detail
        /// </summary>
        /// <param name="authoriserLevelDetailId">authoriserLevelDetailId of AuthoriserLevelDetails</param>
        /// <param name="amount">amount of AuthoriserLevelDetails</param>
        /// <param name="decription">decription of AuthoriserLevelDetails</param>
        /// <returns></returns>
        public int SaveAuthoriserLevel(int authoriserLevelDetailId, decimal amount, string decription)
        {
            int result = 0;
            amount = Math.Round(amount, 2);
            var flag = false;
            flag = CheckDuplicateAmount(amount, authoriserLevelDetailId);
            if (flag)
            {
                return -999;
            }
            flag = false;
           flag= CheckDuplicateAuthoriserLevelDescription(decription, authoriserLevelDetailId);
           if (flag)
           {
               return -998;
           }
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", authoriserLevelDetailId);
                expdata.sqlexecute.Parameters.Add("@Amount", amount);
                expdata.sqlexecute.Parameters.Add("@Description", decription);
                expdata.sqlexecute.Parameters.Add("@employeeID", this.user.EmployeeID);
                if (this.user.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", this.user.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.SaveAuthoriserLevelDetails");
                result = (int)expdata.sqlexecute.Parameters["@id"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            return result;
        }

        /// <summary>
        /// Get Authoriser Level Details based on authoriserLevelDetailId
        /// </summary>
        /// <param name="authoriserLevelDetailId">authoriserLevelDetailId of AuthoriserLevelDetails</param>
        /// <returns></returns>
        public void GetAuthoriserLevelDetail(int authoriserLevelDetailId)
        {
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", authoriserLevelDetailId);
                using (IDataReader reader = expdata.GetReader("GetAuthoriserLevelDetails", CommandType.StoredProcedure))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(reader.GetOrdinal("AuthoriserLevelDetailId"));
                            decimal amount = reader.GetDecimal(reader.GetOrdinal("Amount"));
                            string decscription = reader.GetString(reader.GetOrdinal("Description"));
                            amount = Math.Round(amount, 2);
                            this.AuthoriserLevelDetailId = id;
                            this.Amount = amount;
                            this.Description = decscription;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if any authoriser level with provided amount exists
        /// </summary>
        /// <param name="amount">amount of AuthoriserLevelDetails</param>
        /// <param name="id">id of AuthoriserLevelDetails</param>
        /// <returns></returns>
        public bool CheckDuplicateAmount(decimal amount, int id)
        {
            var result = false;
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@Amount", amount);
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", id);
                decimal? authoriserLevelAmount = expdata.ExecuteScalar<decimal?>("dbo.CheckDuplicateAuthoriserLevelAmount", CommandType.StoredProcedure);
                if (authoriserLevelAmount != null)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Check if any authoriser level with provided amount exists
        /// </summary>
        /// <param name="description">description of AuthoriserLevelDetails</param>
        /// <param name="id">id of AuthoriserLevelDetails</param>
        /// <returns>return true when record found </returns>
        public bool CheckDuplicateAuthoriserLevelDescription(string description, int id)
        {
            var result = false;
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", id);
                expdata.sqlexecute.Parameters.Add("@Description", description);
                string authoriserLevelDescription = expdata.ExecuteScalar<string>("dbo.CheckDuplicateAuthoriserLevelDescription", CommandType.StoredProcedure);
                if (string.IsNullOrWhiteSpace(authoriserLevelDescription) == false)
                {
                    result = true;
                   
                }
            }
            return result;
        }

        /// <summary>
        /// Check Authoriser Level Detail assign to employee
        /// </summary>
        /// <param name="authoriserLevelDetailId">authoriserLevelDetailId of AuthoriserLevelDetails</param>
        /// <returns>return true when record found</returns>
        public bool CheckIfAuthoriserLevelAssignedToEmployees(int authoriserLevelDetailId)
        {
            var result = false;
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@AuthoriserLevelDetailId", authoriserLevelDetailId);
                int? employeeId = expdata.ExecuteScalar<int?>("dbo.CheckIfAuthoriserLevelAssignedToEmployees", CommandType.StoredProcedure);
                if (employeeId!=null)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Get defult authoriser employee 
        /// </summary>
        /// <returns>Returns defult authoriser employee.</returns>
        public EmployeeWithNameAndId GetDefultAuthoriserEmployee()
        {
            var defaultEmployee = new EmployeeWithNameAndId();
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                using (IDataReader reader = expdata.GetReader("GetDefultAuthoriserEmployee", CommandType.StoredProcedure))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            defaultEmployee.EmployeeId = reader.GetInt32(reader.GetOrdinal("employeeid"));
                            defaultEmployee.FullName = reader.GetString(reader.GetOrdinal("EmployeeName"));
                        }
                    }
                }
            }
            return defaultEmployee;
        }

        /// <summary>
        /// Employee with full name, user name and id
        /// </summary>
        public struct EmployeeWithNameAndId
        {
            /// <summary>
            /// Gets or sets id of an employee
            /// </summary>
            public int EmployeeId { get; set; }

            /// <summary>
            /// Gets or sets full name of an employee
            /// </summary>
            public string FullName { get; set; }
        }
    }

    public class SaveExpensesItems
    {
        public int SavedExpensesId { get; set; }
    }
}