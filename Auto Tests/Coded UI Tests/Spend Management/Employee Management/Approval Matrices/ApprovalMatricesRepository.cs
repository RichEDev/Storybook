namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Approval_Matrices
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders;

    /// <summary>
    /// The Approval Matrix Repository
    /// </summary>
    class ApprovalMatricesRepository
    {
        internal static List<ApprovalMatrices> PopulateApprovalMatrices()
        {
            string strSQL = "SELECT approvalMatrixId, name, description, defaultApproverBudgetHolderId, defaultApproverEmployeeId, defaultApproverTeamId FROM approvalMatrices";

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            
            List<ApprovalMatrices> approvalMatrices = new List<ApprovalMatrices>();
            using (SqlDataReader reader = db.GetReader(strSQL))
            {
                int approvalMatrixIdOrdinal = reader.GetOrdinal("approvalMatrixId");
                int nameOrdinal = reader.GetOrdinal("name");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int defaultApproverBudgetHolderIdOrdinal = reader.GetOrdinal("defaultApproverBudgetHolderId");
                int defaultApproverEmployeeIdOrdinal = reader.GetOrdinal("defaultApproverEmployeeId");
                int defaultApproverTeamIdOrdinal = reader.GetOrdinal("defaultApproverTeamId");

                while (reader.Read())
                {
                    ApprovalMatrices approvalMatrix = new ApprovalMatrices();
                    approvalMatrix.ApprovalMatrixId = reader.GetInt32(approvalMatrixIdOrdinal);
                    approvalMatrix.Name = reader.GetString(nameOrdinal);
                    approvalMatrix.Description = reader.GetString(descriptionOrdinal);
                    approvalMatrix.DefaultApproverEmployeeId = reader.IsDBNull(defaultApproverEmployeeIdOrdinal) ? null : (int?)reader.GetInt32(defaultApproverEmployeeIdOrdinal);
                    approvalMatrix.DefaultApproverBudgetHolderId = reader.IsDBNull(defaultApproverBudgetHolderIdOrdinal) ? null : (int?)reader.GetInt32(defaultApproverBudgetHolderIdOrdinal);
                    approvalMatrix.DefaultApproverTeamId = reader.IsDBNull(defaultApproverTeamIdOrdinal) ? null : (int?)reader.GetInt32(defaultApproverTeamIdOrdinal);

                    if (approvalMatrix.DefaultApproverEmployeeId.HasValue)
                    {
                        approvalMatrix.DefaultEmployee = EmployeesRepository.PopulateEmployee(sqlToExecute: Employees.SqlItems, employeeId: approvalMatrix.DefaultApproverEmployeeId).FirstOrDefault();
                    }

                    if (approvalMatrix.DefaultApproverBudgetHolderId.HasValue)
                    {
                        approvalMatrix.DefaultHolder = BudgetHoldersRepository.PopulateBudgetHolders(sqlToExecute: BudgetHolders.SqlItems, budgetHolderId: approvalMatrix.DefaultApproverBudgetHolderId).FirstOrDefault();
                    }

                    if (approvalMatrix.DefaultApproverTeamId.HasValue)
                    {
                        approvalMatrix.DefaultTeam = TeamsRepository.PopulateTeams(sqlToExecute: Teams.SqlItems, teamId: approvalMatrix.DefaultApproverTeamId).FirstOrDefault();
                    }

                    approvalMatrix.ApprovalMatrixLevels = PopulateApprovalMatrixLevels(approvalMatrix.ApprovalMatrixId);
                    approvalMatrices.Add(approvalMatrix);
                }
            }
            return approvalMatrices;
        }

        internal static List<ApprovalMatrixLevel> PopulateApprovalMatrixLevels(int approvalMatrixId)
        {
            string strSQL = "select approvalMatrixLevelId, approvalLimit, approverEmployeeId, approverBudgetHolderId, approverTeamId from approvalMatrixLevels where approvalMatrixId = @approvalMatrixId";

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            
            List<ApprovalMatrixLevel> approvalMatrixLevels = new List<ApprovalMatrixLevel>();
            db.sqlexecute.Parameters.Add("@approvalMatrixId", approvalMatrixId);
            using (SqlDataReader reader = db.GetReader(strSQL))
            {
                int approvalMatrixLevelIdOrdinal = reader.GetOrdinal("approvalMatrixLevelId");
                int threshHoldAmountOrdinal = reader.GetOrdinal("approvalLimit");
                int approverEmployeeIdOrdinal = reader.GetOrdinal("approverEmployeeId");
                int approverHolderIdOrdinal = reader.GetOrdinal("approverBudgetHolderId");
                int approverTeamIdOrdinal = reader.GetOrdinal("approverTeamId");

                while (reader.Read())
                {
                    ApprovalMatrixLevel approvalMatrixLevel = new ApprovalMatrixLevel();
                    approvalMatrixLevel.ApprovalMatrixId = approvalMatrixId;
                    approvalMatrixLevel.ApprovalMatrixLevelId = reader.GetInt32(approvalMatrixLevelIdOrdinal);
                    approvalMatrixLevel.ThresholdAmount = reader.GetDecimal(threshHoldAmountOrdinal);
                    approvalMatrixLevel.ApproverEmployeeId = reader.IsDBNull(approverEmployeeIdOrdinal) ? null : (int?)reader.GetInt32(approverEmployeeIdOrdinal);
                    approvalMatrixLevel.ApproverBudgetHolderId = reader.IsDBNull(approverHolderIdOrdinal) ? null : (int?)reader.GetInt32(approverHolderIdOrdinal);
                    approvalMatrixLevel.ApproverTeamId = reader.IsDBNull(approverTeamIdOrdinal) ? null : (int?)reader.GetInt32(approverTeamIdOrdinal);

                    if (approvalMatrixLevel.ApproverEmployeeId.HasValue)
                    {
                        approvalMatrixLevel.LevelEmployee = EmployeesRepository.PopulateEmployee(sqlToExecute: Employees.SqlItems, employeeId: approvalMatrixLevel.ApproverEmployeeId).FirstOrDefault();
                    }

                    if (approvalMatrixLevel.ApproverBudgetHolderId.HasValue)
                    {
                        approvalMatrixLevel.LevelHolder = BudgetHoldersRepository.PopulateBudgetHolders(sqlToExecute: BudgetHolders.SqlItems, budgetHolderId: approvalMatrixLevel.ApproverBudgetHolderId).FirstOrDefault();
                    }

                    if (approvalMatrixLevel.ApproverTeamId.HasValue)
                    {
                        approvalMatrixLevel.LevelTeam = TeamsRepository.PopulateTeams(sqlToExecute: Teams.SqlItems, teamId: approvalMatrixLevel.ApproverTeamId).FirstOrDefault();
                    }

                    approvalMatrixLevels.Add(approvalMatrixLevel);
                }
            }

            return approvalMatrixLevels;
        }

        /// <summary>
        /// The Create Approval Matrix.
        /// </summary>
        /// <param name="approvalMatrixToSave">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CreateApprovalMatrix(ApprovalMatrices approvalMatrixToSave, ProductType executingProduct, int adminId)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            approvalMatrixToSave.ApprovalMatrixId = 0;
            expdata.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", approvalMatrixToSave.ApprovalMatrixId);
            expdata.sqlexecute.Parameters.AddWithValue("@name", approvalMatrixToSave.Name);
            expdata.sqlexecute.Parameters.AddWithValue("@description", approvalMatrixToSave.Description);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultApproverBudgetHolderId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultApproverEmployeeId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultApproverTeamId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", adminId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            if (approvalMatrixToSave.DefaultApproverBudgetHolderId != null)
            {
                expdata.sqlexecute.Parameters["@defaultApproverBudgetHolderId"].Value = approvalMatrixToSave.DefaultHolder.Employee.employeeID;
            }

            if (approvalMatrixToSave.DefaultApproverEmployeeId != null)
            {
                expdata.sqlexecute.Parameters["@defaultApproverEmployeeId"].Value = approvalMatrixToSave.DefaultEmployee.employeeID;
            }

            if (approvalMatrixToSave.DefaultApproverTeamId != null)
            {
                expdata.sqlexecute.Parameters["@defaultApproverTeamId"].Value = approvalMatrixToSave.DefaultTeam.TeamId;
            }

            expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveApprovalMatrix");

            approvalMatrixToSave.ApprovalMatrixId = (int)expdata.sqlexecute.Parameters["@id"].Value;
            expdata.sqlexecute.Parameters.Clear();

            //if (id > 0 && entity.ApprovalMatrixLevels != null)
            //{
            //    foreach (ApprovalMatrixLevel level in entity.ApprovalMatrixLevels)
            //    {
            //        this.SaveLevel(level);
            //    }
            //}

            return approvalMatrixToSave.ApprovalMatrixId;
        }

        /// <summary>
        /// The Delete Approval Matrix.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int DeleteApprovalMatrix(int id, ProductType executingProduct, int adminId)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct)); 
            
            expdata.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", id);
            expdata.sqlexecute.Parameters.AddWithValue("@auditEmployeeId", adminId);
            expdata.sqlexecute.Parameters.AddWithValue("@auditDelegateId", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteApprovalMatrix");

            var returnid = (int)expdata.sqlexecute.Parameters["@id"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return returnid;
        }

        /// <summary>
        /// Save a matrix level.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int SaveLevel(ApprovalMatrixLevel level, ProductType executingProduct, int adminId)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            level.ApprovalMatrixLevelId = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", level.ApprovalMatrixId);
            expdata.sqlexecute.Parameters.AddWithValue("@approvalmatrixlevelid", level.ApprovalMatrixLevelId);
            expdata.sqlexecute.Parameters.AddWithValue("@approvalLimit", level.ThresholdAmount);
            expdata.sqlexecute.Parameters.AddWithValue("@approverBudgetHolderId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@approverEmployeeId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@approverTeamId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", adminId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            if (level.ApproverBudgetHolderId != null)
            {
                expdata.sqlexecute.Parameters["@approverBudgetHolderId"].Value = level.LevelHolder.BudgetHolderId;
            }

            if (level.ApproverEmployeeId != null)
            {
                expdata.sqlexecute.Parameters["@approverEmployeeId"].Value = level.LevelEmployee.employeeID;
            }

            if (level.ApproverTeamId != null)
            {
                expdata.sqlexecute.Parameters["@approverTeamId"].Value = level.LevelTeam.TeamId;
            }

            expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveApprovalMatrixLevel");

            level.ApprovalMatrixLevelId = (int)expdata.sqlexecute.Parameters["@id"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return level.ApprovalMatrixLevelId;
        }

        /// <summary>
        /// The delete level.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <param name="matrixLevelId">
        /// The matrix level id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteLevel(int matrixId, int matrixLevelId, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));

            expdata.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", matrixId);
            expdata.sqlexecute.Parameters.AddWithValue("@approvalmatrixlevelid", matrixLevelId);
            expdata.sqlexecute.Parameters.AddWithValue("@auditEmployeeId", 0);
            expdata.sqlexecute.Parameters.AddWithValue("@auditDelegateId", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteApprovalMatrixLevel");

            var returnid = (int)expdata.sqlexecute.Parameters["@id"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return returnid;
        }
    }
}
