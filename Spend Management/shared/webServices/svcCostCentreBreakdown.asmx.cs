using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.Script.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcCostCentreBreakdown
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcCostCentreBreakdown : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="ccbArray"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<cCcbItem> TestArray(object[] ccbArray)
        {
            cCcbItemArray costCentres = new cCcbItemArray(ccbArray);
            
            return costCentres.itemArray;
        }
    }
}
