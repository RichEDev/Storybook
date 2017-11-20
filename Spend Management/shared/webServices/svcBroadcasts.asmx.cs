using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcBroadcasts
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcBroadcasts : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public cBroadcastMessages GetMessages()
        {
            CurrentUser currentUser = cMisc.getCurrentUser();
            cBroadcastMessages clsBroadcastMessages = new cBroadcastMessages(currentUser.Employee.employeeid);


            return clsBroadcastMessages;
        }
    }
}
