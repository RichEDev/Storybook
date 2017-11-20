using System;
using Common.Logging;
using Common.Logging.Converters;
using Common.Logging.Log4Net;

namespace ApproverReminder
{
    using APICallsHelper;

    /// <summary>
    /// Starts application for sending sending email reminders to approvers to notify them of pending claims.
    /// </summary>
     public class Program
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<Program>().GetLogger();

        /// <summary>
        /// An instance of <see cref="IExtraContext"/> for logging extra inforamtion
        /// </summary>
        private static readonly IExtraContext LoggingContent = new Log4NetContextAdapter();

        /// <summary>
        /// The Main method which sends out email reminder to approvers of pending claims.
        /// </summary>
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            LoggingContent["activityid"] = new TraceActivityId();

            Log.Debug("Claim reminder service has started.");

            var claimReminder = new ClaimReminder(new RequestHelper(), new ResponseHelper(), new LogFactory<ClaimReminder>().GetLogger());

            claimReminder.RemindApproversOfPendingClaims();

            claimReminder.RemindClaimantsOfCurrentClaims();
            
            Log.Debug("Claim reminder service has completed.");
        }

        /// <summary>
        /// Handles any errors that arent specifically handled elsewhere
        /// </summary>
        /// <param name="sender">Information on where the error has come from</param>
        /// <param name="e">Object containing the exception that occured</param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;

            if (Log.IsErrorEnabled)
            {
                Log.Error($"Unhandled exception occurred due to {exception.Message}", exception);
            }

            Environment.Exit(0);
        }
    }
}