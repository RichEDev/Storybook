namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;

    using BusinessLogic.Modules;

    using SpendManagementLibrary;

    /// <summary>
    /// Web service to obtain broadcast message information
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class SvcBroadcastMessage : WebService
    {
        /// <summary>
        /// Retrieves all messages for the current user
        /// </summary>
        /// <param name="includeNotes">
        /// The include Notes.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The cBroadcastMessages
        /// </returns>
        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<cBroadcastMessage> GetMessages(bool includeNotes = false, string page = "")
        {
            var user = cMisc.GetCurrentUser();
            var messages = new cBroadcastMessages(user.AccountID);
            broadcastLocation location;

            switch (page)
            {
                case "home.aspx":
                    location = broadcastLocation.HomePage;
                    break;
                case "submitclaim.aspx":
                    location = broadcastLocation.SubmitClaim; 
                    break;
                default:
                    location = broadcastLocation.notSet;
                    break;
            }
            
            var messageList = messages.GetMessages(user.Employee, user.AccountID, location);

            // if we're greenlight we want to remove the expenses related messages.
            if (user.CurrentActiveModule == Modules.Greenlight || user.CurrentActiveModule == Modules.GreenlightWorkforce)
            {
                var expensesMessageList = messageList.Where(message => message.location == broadcastLocation.SubmitClaim);
                foreach (var message in expensesMessageList)
                {
                    messageList.Remove(message);
                }
            }

            // once per-session broadcast messages need to be marked
            foreach (var message in messageList.Where(message => message.oncepersession))
            {
                this.Session["broadcast" + message.broadcastid] = 1;
            }

            if (includeNotes)
            {
                var employees = new cEmployees(user.AccountID);
                DataTable notesTable = employees.getNotes(user.EmployeeID);
                if (notesTable != null && notesTable.Rows != null && notesTable.Rows.Count > 0)
                {
                    foreach (DataRow noteRow in notesTable.Rows)
                    {
                        var noteId = (int)noteRow["noteid"];
                        var date = (DateTime)noteRow["datestamp"];
                        var title = string.Format("Note - Sent: {0} {1}", date.ToShortDateString(), date.ToString("HH:mm:ss"));
                        string note = noteRow["note"].ToString();
                        var message = new cBroadcastMessage(0, title, note, DateTime.Now, DateTime.Now, true, broadcastLocation.HomePage, DateTime.Now, true, DateTime.Now, user.EmployeeID, DateTime.Now, user.EmployeeID);
                        messageList.Add(message);
                        employees.markNoteAsRead(noteId);
                    }
                }
            }

            return messageList;
        }
    }
}
