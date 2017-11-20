namespace Spend_Management.shared.webServices
{
    using System.ComponentModel;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Summary description for svcContextualHelp
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcContextualHelp : WebService
    {

        /// <summary>
        /// Mark a contextual help item as "read" so that it is displayed
        /// </summary>
        /// <param name="contextualHelpId">The identifier of the contextual help item</param>
        /// <returns>true</returns>;
        [WebMethod(EnableSession = true)]
        public bool MarkAsRead(int contextualHelpId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employeeContextualHelp = new EmployeeContextualHelp(user.AccountID, user.EmployeeID);

            if (employeeContextualHelp.Show(contextualHelpId))
            {
                employeeContextualHelp.Add(contextualHelpId);
            }

            return true;
        }

        /// <summary>
        /// Mark a contextual help item as "unread" so that it is displayed
        /// </summary>
        /// <param name="contextualHelpId">The identifier of the contextual help item</param>
        /// <returns>true</returns>
        [WebMethod(EnableSession = true)]
        public bool MarkAsUnread(int contextualHelpId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employeeContextualHelp = new EmployeeContextualHelp(user.AccountID, user.EmployeeID);

            if (!employeeContextualHelp.Show(contextualHelpId))
            {
                employeeContextualHelp.Remove(contextualHelpId);
            }

            return true;
        }
    }
}
