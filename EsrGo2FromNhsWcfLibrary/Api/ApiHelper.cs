namespace EsrGo2FromNhsWcfLibrary.Api
{
    using APICallsHelper;

    /// <summary>
    /// A class to handle calls to the Spend Management API
    /// </summary>
    public class ApiHelper
    {
        /// <summary>
        /// A private instance of <see cref="ApiClientHelper.Client"/>
        /// </summary>
        private ApiClientHelper.Client apiClientHelper;

        /// <summary>
        /// The endpoint to use for the Email notifications
        /// </summary>
        private const string EndPoint = "/EmailNotifications/{0}/SendExcessMileageNotification/{1}"; 

        /// <summary>
        /// Create a new instance of <see cref="ApiHelper"/>
        /// </summary>
        public ApiHelper()
        {
            var apiUrlPath = new RequestHelper().GetHttpWebRequest(string.Empty).Address.AbsoluteUri;
            this.apiClientHelper = new ApiClientHelper.Client(apiUrlPath);
        }

        /// <summary>
        /// Send a new email notification for change of address when excess mileage is used.
        /// </summary>
        /// <param name="employeeId">The employee ID of the recipient of the email</param>
        /// <param name="accountId">The account ID of the recepient</param>
        public void SendEmailNotificationForExcessMileage(int employeeId, int accountId)
        {
            var accountEndpoint = string.Format(EndPoint, accountId, employeeId);
            this.apiClientHelper.SendEmailNotificationForExcessMileage(accountEndpoint);
        }

    }
}
