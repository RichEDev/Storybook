using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using SpendManagementLibrary.Employees;

namespace SpendManagementLibrary
{
	public class cFWAuditLog
	{
		public const char AUDIT_ADD = 'A';
		public const char AUDIT_DEL = 'D';
		public const char AUDIT_ARCH = 'X';
		public const char AUDIT_ERROR = '!';
		public const char AUDIT_UPDATE = 'E';
		public const char AUDIT_LOGON = 'I';
		public const char AUDIT_LOGOFF = 'O';
		public const int MAX_AUDITVAL_LEN = 2000;
		public ArrayList arrAuditRecs;
		private cFWSettings fws;
        private SpendManagementElement eElement;
        private int nSubAccountID;

		public cFWAuditLog(cFWSettings cFWS, SpendManagementElement element, int SubAccountID)
		{
			fws = cFWS;
			arrAuditRecs = new ArrayList();
            eElement = element;
            nSubAccountID = SubAccountID;
		}

		public int ARec_Count
		{
			get
			{
				if (arrAuditRecs != null)
				{
					return arrAuditRecs.Count;
				}
				else
				{
					return 0;
				}
			}
		}

        /// <summary>
        /// The associated spend management element
        /// </summary>
        public SpendManagementElement element
        {
            get { return eElement; }
        }

        /// <summary>
        /// Sub account ID
        /// </summary>
        public int SubAccountID
        {
            get { return nSubAccountID; }
        }

		public void AddAuditRec(cAuditRecord rec, bool StartNew)
		{
		if(StartNew || arrAuditRecs == null)
		{
			arrAuditRecs = new ArrayList();
		}

		if(rec.DataElementDesc == null)
		{
			rec.DataElementDesc = "";
		}
		if( rec.ElementDesc == null)
		{
			rec.ElementDesc = "";
		}
		if(rec.PostVal == null)
		{
			rec.PostVal = "";
		}
		if (rec.PreVal == null)
		{
			rec.PreVal = "";
		}
		if(rec.ContractNumber == null)
		{
			rec.ContractNumber = "";
		}
		arrAuditRecs.Add(rec);

		return;
		}

        public void CommitAuditLog(Employee employee, int EntityID)
		{
			// write record collection to the AuditLog db table
			cFWDBConnection fwdb = new cFWDBConnection();


            if (ARec_Count > 0)
			{
				fwdb.DBOpen(fws, false);

				foreach (object adutRecord in arrAuditRecs)
				{
				    cAuditRecord rec = (cAuditRecord)adutRecord;
				    fwdb.SetFieldValue("employeeID", employee.EmployeeID, "N", true);
				    fwdb.SetFieldValue("Datestamp", DateTime.Now, "D", false);
				    fwdb.SetFieldValue("employeeUsername", employee.Username , "S", false);

				    byte action = 0;

				    switch (rec.Action)
				    {
				        case 'A':
				            action = 1;
				            break;
				        case 'E':
				            action = 2;
				            break;
				        case 'D':
				            action = 3;
				            break;

				    }

				    fwdb.SetFieldValue("action", action , "S", false);

				    fwdb.SetFieldValue("elementID", element, "S", false);

				    string tmpStr;
				    if (rec.Action == 'E')
				    {
				        if (rec.ContractNumber != string.Empty)
				        {
				            if (!rec.ElementDesc.Contains(rec.ContractNumber))
				            {
				                tmpStr = rec.ElementDesc + " (" + rec.ContractNumber + ")";
				            }
				            else
				            {
				                tmpStr = rec.ElementDesc;
				            }
				            rec.ElementDesc = tmpStr;
				        }                        					
				    }

				    tmpStr = rec.ElementDesc.Length > 250 ? rec.ElementDesc.Substring(0, 250) : rec.ElementDesc;

				    if (tmpStr == "")
				    {
				        tmpStr = "n/a";
				    }
				    fwdb.SetFieldValue("recordTitle", tmpStr, "S", false);
                    
				    if (action == 2)
				    {
				        tmpStr = rec.PreVal.Length > MAX_AUDITVAL_LEN ? rec.PreVal.Substring(0, MAX_AUDITVAL_LEN) : rec.PreVal;
                    
				        fwdb.SetFieldValue("oldvalue", tmpStr, "S", false);

				        if (rec.Action == 'A')
				        {
				            if (rec.PostVal.Contains(rec.ContractNumber))
				            {
				                tmpStr = rec.PostVal + " (" + rec.ContractNumber + ")";
				                rec.PostVal = tmpStr;
				            }                            
				        }

				        tmpStr = rec.PostVal.Length > MAX_AUDITVAL_LEN ? rec.PostVal.Substring(0, MAX_AUDITVAL_LEN) : rec.PostVal;

				        fwdb.SetFieldValue("newvalue", tmpStr, "S", false);
				    }

				    fwdb.SetFieldValue("subAccountId", SubAccountID, "N", false);
				    fwdb.SetFieldValue("entityid", EntityID, "N", false);
				    //fwdb.SetFieldValue("Location Id", usrInfo.ActiveLocation, "N", false);
				    fwdb.FWDb("W", "auditLog", "", "", "", "", "", "", "", "", "", "", "", "");
				}

				// Clear array ready for re-use
				arrAuditRecs.Clear();

				fwdb.DBClose();
			}

			return;
		}
	}

	public class cAuditRecord
	{
		public char Action;
		public string DataElementDesc;
		public string ContractNumber;
		public string ElementDesc;
		public string PreVal;
		public string PostVal;

		public cAuditRecord()
		{
		}
	}
}
