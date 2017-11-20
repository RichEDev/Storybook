using SpendManagementLibrary.Helpers;

namespace Spend_Management.shared.webServices
{
    using System.Web.Services;

    /// <summary>
    /// Summary description for svcErrors
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcErrors : WebService
    {
        /// <summary>
        /// Reports any JavaScript errors
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession=true)]
        public void ClientJavaScriptError(string errorMessage, string pageUrl, int lineNumber)
        {
            JavaScriptException exception = new JavaScriptException(errorMessage, pageUrl, lineNumber);
            ErrorHandlerWeb errorHandler = new ErrorHandlerWeb();
            errorHandler.SendError(cMisc.GetCurrentUser(), exception);
        }
    }
}
