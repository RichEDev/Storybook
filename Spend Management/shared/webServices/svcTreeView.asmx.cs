using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Text.RegularExpressions;
using System.Web.Script.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcTreeView
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcTreeView : System.Web.Services.WebService
    {
        private readonly Regex regGuid = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public List<cTreeNode> GetNodes(string baseTableID, bool isSubNode)
        {
            if (string.IsNullOrEmpty(baseTableID) == true || regGuid.IsMatch(baseTableID) == false)
            {
                throw new FormatException("baseTableID can not be converted to a Guid");
            }

            Guid gBaseTableID = new Guid(baseTableID);
            
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cReportTreeView clsTreeView = new cReportTreeView(currentUser.AccountID);

            return clsTreeView.GetNodes(gBaseTableID, isSubNode);
        }

        //[WebMethod(EnableSession=true)]
        //[ScriptMethod]
        //public List<sFieldBasics> GetFields(string viewGroupID)
        //{
        //    if (string.IsNullOrEmpty(viewGroupID) == true || regGuid.IsMatch(viewGroupID) == false)
        //    {
        //        throw new FormatException("viewGroupID can not be converted to a Guid");
        //    }

        //    CurrentUser currentUser = cMisc.GetCurrentUser();
        //    cTreeView clsTreeView = new cTreeView(currentUser.AccountID);

        //    Guid gViewGroupID = new Guid(viewGroupID);

        //    return clsTreeView.GetFields(gViewGroupID);
        //}
    }
}
