using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.Expenses.Base_Information.Cost_Codes
{
    /// <summary>
    /// The cost code repository
    /// </summary>
    public class CostCodesRepository
    {
        /// <summary>
        /// The populate costcode
        /// </summary>
        /// <param name="costcodeid"></param>
        /// <param name="sqlToExecute"></param>
        /// <returns></returns>
        public static List<CostCodes> PopulateCostCodes(int? costcodeid = null, string sqlToExecute = "")
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT costcodeid, costcode, description FROM costcodes";

            if (sqlToExecute == string.Empty)
            {
                sqlToExecute = strSQL;
            }
            else
            {
                db.sqlexecute.Parameters.Add("@costcodeid", costcodeid);
            }
            List<CostCodes> costCodes = new List<CostCodes>();
            using (SqlDataReader reader = db.GetReader(sqlToExecute))
            {
                int costCodeIdOrdinal = reader.GetOrdinal("costcodeid");
                int costCodeOrdinal = reader.GetOrdinal("costcode");
                int descriptionOrdinal = reader.GetOrdinal("description");
                // int teamLeaderOrdinal = reader.GetOrdinal("teamleaderid");

                while (reader.Read())
                {
                    CostCodes costCode = new CostCodes(reader.GetInt32(costCodeIdOrdinal), reader.GetString(costCodeOrdinal), reader.GetString(descriptionOrdinal));
                    costCodes.Add(costCode);
                }
            }
            return costCodes;
        }

        /// <summary>
        /// The delete cost code
        /// </summary>
        /// <param name="costCodeId"></param>
        /// <param name="employeeId"></param>
        /// <param name="executingProduct"></param>
        public static void DeleteCostcode(int costCodeId, int employeeId, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costCodeId);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@return", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@return"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("deletecostcode");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The create cost code
        /// </summary>
        /// <param name="costCodeToCreate"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static int CreateCostCode(CostCodes costCodeToCreate, int employeeId, ProductType executingProduct)
        {
            costCodeToCreate.CostCodeId = 0;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costCodeToCreate.CostCodeId);
            expdata.sqlexecute.Parameters.AddWithValue("@costcode", costCodeToCreate.CostCodeName);
            expdata.sqlexecute.Parameters.AddWithValue("@description", costCodeToCreate.Description);
            expdata.sqlexecute.Parameters.AddWithValue("@ownerEmployeeId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@ownerTeamId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@ownerBudgetHolderId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeId);

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", DBNull.Value);

            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@return", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@return"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCostcode");
            costCodeToCreate.CostCodeId = (int)expdata.sqlexecute.Parameters["@return"].Value;
            expdata.sqlexecute.Parameters.Clear();
            return costCodeToCreate.CostCodeId;
        }

        public static void ChangeStatus(int costcodeid, ProductType executingProduct, bool archive = true)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeid);
            string strsql = archive ? "update costcodes set archived = 1 where costcodeid = @costcodeid" : "update costcodes set archived = 0, where costcodeid = @costcodeid";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
    }
}
