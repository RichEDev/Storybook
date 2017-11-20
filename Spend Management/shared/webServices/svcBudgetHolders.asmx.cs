using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcBudgetHolders
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [System.Web.Script.Services.ScriptService]
    public class svcBudgetHolders : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public int deleteBudgetHolder(int budgetholderid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cBudgetholders clsholders = new cBudgetholders(user.AccountID);
            return clsholders.deleteBudgetHolder(budgetholderid);
        }
    }
}
