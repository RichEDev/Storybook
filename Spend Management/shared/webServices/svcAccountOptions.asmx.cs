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
    /// Summary description for svcAccountOptions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcAccountOptions : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cFieldToDisplay GetFieldToDisplay(string code)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cMisc clsMisc = new cMisc(currentUser.AccountID);
            cFieldToDisplay reqFieldToDisplay = clsMisc.GetGeneralFieldByCode(code);

            return reqFieldToDisplay;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void SaveFieldToDisplay(cFieldToDisplay field)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cMisc clsMisc = new cMisc(currentUser.AccountID);
            List<cFieldToDisplay> lstFieldsToDisplay = new List<cFieldToDisplay>();
            lstFieldsToDisplay.Add(field);

            clsMisc.UpdateFieldsToDisplay(lstFieldsToDisplay);

        }

        [WebMethod(EnableSession = true)]
        public bool checkForPNGAttachmentType()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cGlobalMimeTypes gMimeTypes = new cGlobalMimeTypes(currentUser.AccountID);
            
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            if(mimeTypes.GetMimeTypeByGlobalID(gMimeTypes.getMimeTypeByExtension("PNG").GlobalMimeID) == null)
                return false;
            
            return true;

        }
    }
}
