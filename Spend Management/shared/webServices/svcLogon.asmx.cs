namespace Spend_Management
{
    using System.Web.Services;

    using SpendManagementLibrary;

    /// <summary>
    /// The webservice for retrieving forgotten details.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcLogon : WebService
    {
        /// <summary>
        /// The request forgotten details.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address.
        /// </param>
        /// <param name="hostname">
        /// The hostname.
        /// </param>
        /// <returns>
        /// A ForgottenDetailsResponse object.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public ForgottenDetailsResponse RequestForgottenDetails(string emailAddress, string hostname)
        {
            return cEmployees.RequestForgottenDetails(emailAddress, HostManager.GetModule(hostname));
        }
    }
}
