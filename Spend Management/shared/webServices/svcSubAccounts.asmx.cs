using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcSubAccounts
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcSubAccounts : System.Web.Services.WebService
    {

        /// <summary>
        /// Switch the sub account ID in the session so that the home page redirection performed after this takes the user to the 
        /// the correct sub account with the correct access role privileges
        /// </summary>
        /// <param name="SubAccountID"></param>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public void SwitchSubAccount(int SubAccountID)
        {
            System.Web.HttpContext.Current.Session["SubAccountID"] = SubAccountID;
        }
    }
}
