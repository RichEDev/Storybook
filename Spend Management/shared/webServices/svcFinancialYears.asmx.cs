using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary.FinancialYears;

namespace Spend_Management.shared.webServices
{
    /// <summary>
    /// Service interface for FinancialYears class in Spend Management Library.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcFinancialYears : System.Web.Services.WebService
    {

        /// <summary>
        /// Save new or update existing Financial Year.
        /// </summary>
        /// <param name="financialYearID"></param>
        /// <param name="Description"></param>
        /// <param name="YearStart"></param>
        /// <param name="YearEnd"></param>
        /// <param name="Active"></param>
        /// <param name="Primary"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int Save(int financialYearID, string description, string yearStart, string yearEnd, bool active, bool primary)
        {
            var financialYear = new FinancialYear(financialYearID, description, yearStart, yearEnd, active, primary);
            return financialYear.Save(cMisc.GetCurrentUser());
        }

        [WebMethod(EnableSession=true)]
        public FinancialYear Get(int id)
        {
            return FinancialYear.Get(id, cMisc.GetCurrentUser());
        }

        [WebMethod(EnableSession = true)]
        public int Delete(int id)
        {
            return FinancialYear.Delete(id, cMisc.GetCurrentUser());
        }
    }
}
