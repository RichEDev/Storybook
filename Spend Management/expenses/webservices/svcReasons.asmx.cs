using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcReasons
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcReasons : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReasonID"></param>
        /// <param name="ReasonName"></param>
        /// <param name="Description"></param>
        /// <param name="CodeWithVAT"></param>
        /// <param name="CodeWithoutVAT"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveReason(int ReasonID, string ReasonName, string Description, string CodeWithVAT, string CodeWithoutVAT, bool Archived)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int returncode;
            cReasons clsReasons = new cReasons(currentUser.AccountID);
            cReason clsReason;


            if (ReasonID > 0) //update
            {
                cReason oldreason = clsReasons.getReasonById(ReasonID);
                clsReason = new cReason(currentUser.AccountID, ReasonID, ReasonName, Description, CodeWithVAT, CodeWithoutVAT, oldreason.createdon, oldreason.createdby, DateTime.Now, currentUser.EmployeeID, Archived);
            }
            else
            {
                clsReason = new cReason(currentUser.AccountID, ReasonID, ReasonName, Description, CodeWithVAT, CodeWithoutVAT, DateTime.Now, currentUser.EmployeeID, null, null, Archived);

            }
            returncode = clsReasons.saveReason(clsReason);
            return returncode;
        }
    }
}
