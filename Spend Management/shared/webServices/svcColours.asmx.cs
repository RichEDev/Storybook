using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcColours
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [System.Web.Script.Services.ScriptService]
    public class svcColours : System.Web.Services.WebService
    {

        /// <summary>
        /// Restores the default colours
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void Restore()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cColours clscolours = new cColours(curUser.AccountID, curUser.CurrentSubAccountId, curUser.CurrentActiveModule);
            clscolours.RestoreDefaults();
        }
    }
}
