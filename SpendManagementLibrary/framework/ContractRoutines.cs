using System;
using System.Collections.Generic;
using SpendManagementLibrary.Employees;

namespace SpendManagementLibrary
{
	public class ContractRoutines
	{
		public static void AddContractHistory(int accountid, string connStr, Employee employee, SummaryTabs ContractTab, int contractId, cAuditRecord ARec)
		{
			string tmpAction = "";
			DBConnection db = new DBConnection(connStr);
			const string SQL = "insert into contract_history ([contractId], SummaryTab, Action, [modifierName], [employeeId], [changeField], PreVal, PostVal) values (@contractid, @tab, @action, @modifier, @userid, @changefield, @preval, @postval)";

			db.sqlexecute.Parameters.AddWithValue("@contractid", contractId);
			db.sqlexecute.Parameters.AddWithValue("@tab", ContractTab);
			
			switch (ARec.Action)
			{
				case cFWAuditLog.AUDIT_DEL:
					tmpAction = "DELETE";
					break;
				case cFWAuditLog.AUDIT_UPDATE:
					tmpAction = "UPDATE";
					break;
				case cFWAuditLog.AUDIT_ADD:
					tmpAction = "INSERT";
					break;
				default:
					tmpAction = "UNKNOWN";
					break;
			}
			db.sqlexecute.Parameters.AddWithValue("@action", tmpAction);
			db.sqlexecute.Parameters.AddWithValue("@modifier", employee.FullName);
			db.sqlexecute.Parameters.AddWithValue("@userid", employee.EmployeeID);

            if (ARec.ElementDesc == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@changefield", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@changefield", ARec.ElementDesc);
            }
            if (ARec.PreVal == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@preval", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@preval", ARec.PreVal);
            }

            if (ARec.PostVal == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@postval", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@postval", ARec.PostVal);
            }
			db.ExecuteSQL(SQL);
		}

		public static void AddContractHistory(int accountid, string connStr, Employee employee, SummaryTabs ContractTab, int contractId, List<cAuditRecord> AuditRecArrayList)
		{
		    if (AuditRecArrayList.Count <= 0)
		    {
		        return;
		    }

		    foreach (cAuditRecord auditRecord in AuditRecArrayList)
		    {
		        AddContractHistory(accountid, connStr, employee, ContractTab, contractId, auditRecord);
		    }
		}

	    public static bool CheckAudienceAccess(int accountid, Employee employee, string connStr, string dbTable, int entityId, int subAccountId)
		{
			bool retVal = true;
			string sql = "";

			switch (dbTable)
			{
				case "contract_audience":
					sql = "SELECT dbo.CheckContractAccess(@userId, @entityId, @subAccountId) AS [PermitVal]";
					break;
				case "attachment_audience":
					sql = "SELECT dbo.CheckAudienceAccess(@userId, @entityId) AS [PermitVal]";
					break;
				default:
					return retVal;
			}

			DBConnection db = new DBConnection(connStr);
			db.sqlexecute.Parameters.AddWithValue("@userId", employee.EmployeeID);
			db.sqlexecute.Parameters.AddWithValue("@entityId", entityId);

	        if (dbTable == "contract_audience")
	        {
                db.sqlexecute.Parameters.AddWithValue("@subAccountId", subAccountId);
	        }

	        int permitVal = db.getcount(sql);

			if (permitVal == 0)
			{
				retVal = false;
			}

			return retVal;
		}
	}
}
