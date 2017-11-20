using Spend_Management.shared.code;
using Spend_Management.shared.code.Logon;
using System.Web.Services;

namespace Spend_Management.shared.webServices
{
    /// <summary>
    /// Summary description for svcLogonMessages
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)] 
    [System.Web.Script.Services.ScriptService]
    public class svcLogonMessages : System.Web.Services.WebService
    {

        /// <summary>
        /// Delete Logon message through grid request
        /// </summary>
        /// <param name="messageId">Logon message ID which needs to deleted</param>
        [WebMethod(EnableSession = true)]
        public void DeleteLogonMessage(int messageId)
        {
            var logonMessages = new LogonMessages();
            logonMessages.DeleteLogonMessage(messageId);
        }

        /// <summary>
        /// Archive/un-archive Logon message
        /// </summary>
        /// <param name="messageId">Logon message ID which needs to be archieved</param>
        /// <param name="activeStatus">Status of archieve</param>
        [WebMethod(EnableSession = true)]
        public string ChangeStatus(int messageId, int activeStatus)
        {
            var logonMessages = new LogonMessages();
            return logonMessages.ChangeLogonMessagesStatus(messageId, activeStatus);
         }

        /// <summary>
        /// to save logon messags
        /// </summary>
        /// <param name="logonmessages">Object of logon message</param>
        /// <returns>Confirmation of type interger</returns>
        [WebMethod]
        public int SaveLogonMessage(LogonMessage logonmessages)
        {           
            var currentUser = cMisc.GetCurrentUser();
            var logonMessages = new LogonMessages();
            return logonMessages.AddOrUpdateLogonMessage(logonmessages, currentUser.EmployeeID);
        }
    }
}
