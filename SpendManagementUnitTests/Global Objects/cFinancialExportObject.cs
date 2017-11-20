using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cFinancialExportObject
    {
        /// <summary>
        /// Create a financial export global variable
        /// </summary>
        /// <returns></returns>
        public static cFinancialExport CreateFinancialExport()
        {
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cFinancialExports clsExports = new cFinancialExports(cGlobalVariables.AccountID);
            int tempExportID = clsExports.addFinancialExport(new cFinancialExport(0, cGlobalVariables.AccountID, FinancialApplication.ESR, new Guid("4a960142-c5b8-40fe-8eca-b2f7101f329c"), false, cGlobalVariables.EmployeeID, DateTime.UtcNow, 0, DateTime.Now, 4, cGlobalVariables.NHSTrustID));
            cGlobalVariables.ExportID = tempExportID;
            clsExports = new cFinancialExports(cGlobalVariables.AccountID);
            cFinancialExport export = clsExports.getExportById(tempExportID);
            return export;
        }

        /// <summary>
        /// Delete the global variable from the database
        /// </summary>
        public static void DeleteFinancialExport()
        {
            cFinancialExports clsExports = new cFinancialExports(cGlobalVariables.AccountID);
            clsExports.deleteFinancialExport(cGlobalVariables.ExportID);
        }
    }
}
