using Common.Logging;
using Common.Logging.Converters;
using Common.Logging.Log4Net;

namespace SpendManagementApi.Repositories
{
    using Spend_Management;
    using Spend_Management.shared.code;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// The email notification repository
    /// </summary>
    public class EmailNotificationRepository
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<EmailNotificationRepository>().GetLogger();

        /// <summary>
        /// An instance of <see cref="IExtraContext"/> for logging extra inforamtion
        /// </summary>
        private static readonly IExtraContext LoggingContent = new Log4NetContextAdapter();

        /// <summary>
        /// Sends email notification to an email notification group about a Employee.
        /// </summary>
        /// <param name="employeeId">The Id of the Employee to send notifications about.</param>
        /// <param name="accountId">The Id of the Account.</param>
        public bool ExcessMileage(int accountId, int employeeId)
        {
            LoggingContent["activityid"] = new TraceActivityId();

            try
            {
                LoggingContent["accountid"] = accountId;
                LoggingContent["employeeid"] = employeeId;

                Log.Debug("Sending Excess Mileage notifications");

                new NotificationHelper(
                    new cEmployees(accountId).GetEmployeeById(employeeId)
                    ).ExcessMileage();

                Log.Debug("Sent Excess Mileage notifications");
            }
            catch
            {
                Log.Warn("Failed to send Excess Mileage notifications");
                throw new ApiException(ApiResources.ApiErrorEmailNotificationsFailed, ApiResources.ApiErrorEmailNotificationsFailed);
            }

            return true;
        }
    }
}