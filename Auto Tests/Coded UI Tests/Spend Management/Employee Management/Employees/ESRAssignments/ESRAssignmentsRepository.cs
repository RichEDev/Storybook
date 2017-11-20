namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.ESRAssignments
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The esr assignments repository.
    /// </summary>
    public class ESRAssignmentsRepository
    {
        /// <summary>
        /// The save assignment.
        /// </summary>
        /// <param name="assignmentToAdd">
        /// The assignment to add.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CreateEsrAssignment(ESRAssignments assignmentToAdd, ProductType executingProduct)
        {
            assignmentToAdd.AssignmentId = 0;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@esrAssignID", assignmentToAdd.AssignmentId);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", assignmentToAdd.EmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@assignmentNumber", assignmentToAdd.AssignmentNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(assignmentToAdd.Active));
            expdata.sqlexecute.Parameters.AddWithValue("@primaryAssignment", Convert.ToByte(assignmentToAdd.PrimaryAssignment));
            expdata.sqlexecute.Parameters.AddWithValue("@earliestassignmentstartdate", assignmentToAdd.EarliestAssignmentStartDate);

            if (assignmentToAdd.FinalAssignmentEndDate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@finalassignmentenddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@finalassignmentenddate", assignmentToAdd.FinalAssignmentEndDate);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@userid", assignmentToAdd.EmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@supervisorAssignmentNumber", assignmentToAdd.EmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", -1);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveESRAssignmentNumber");
            assignmentToAdd.AssignmentId = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return assignmentToAdd.AssignmentId;
        }

        /// <summary>
        /// The delete assignment.
        /// </summary>
        /// <param name="assignmentToDelete">
        /// The assignment to delete.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        public void DeleteEsrAssignment(ESRAssignments assignmentToDelete, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@esrAssignID", assignmentToDelete.AssignmentId);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", assignmentToDelete.EmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.ExecuteProc("deleteESRAssignment");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The populate employee.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<ESRAssignments> PopulateESRAssigtnment()
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            List<ESRAssignments> esrAssignments = new List<ESRAssignments>();
            using (SqlDataReader reader = db.GetStoredProcReader("getEsrAssignments"))
            {
                #region set database columns
                int employeeidOrdinal = reader.GetOrdinal("employeeId");
                int assignmentIdOrdinal = reader.GetOrdinal("AssignmentID");
                int assignmentNumberOrdinal = reader.GetOrdinal("AssignmentNumber");
                int earliestAssignmentStartDateOrdinal = reader.GetOrdinal("EarliestAssignmentStartDate");
                int finalAssignmentEndDateOrdinal = reader.GetOrdinal("FinalAssignmentEndDate");
                int assignmentStatusOrdinal = reader.GetOrdinal("Active");
                int primaryAssignmentOrdinal = reader.GetOrdinal("PrimaryAssignment");
                #endregion

                while (reader.Read())
                {
                    ESRAssignments esrAssignment = new ESRAssignments();
                    #region set values
                    esrAssignment.AssignmentId = 0;
                    esrAssignment.EmployeeId = reader.GetInt32(employeeidOrdinal);
                    esrAssignment.AssignmentNumber = reader.GetString(assignmentNumberOrdinal);
                    esrAssignment.EarliestAssignmentStartDate = reader.GetDateTime(earliestAssignmentStartDateOrdinal);
                    esrAssignment.FinalAssignmentEndDate = reader.IsDBNull(finalAssignmentEndDateOrdinal) ? null : (DateTime?)reader.GetDateTime(finalAssignmentEndDateOrdinal);
                    esrAssignment.Active = reader.GetBoolean(assignmentStatusOrdinal);
                    esrAssignment.PrimaryAssignment = reader.GetBoolean(primaryAssignmentOrdinal);
                    #endregion
                    esrAssignments.Add(esrAssignment);
                }

                reader.Close();
                db.sqlexecute.Parameters.Clear();
            }

            return esrAssignments;
        }
    }
}
