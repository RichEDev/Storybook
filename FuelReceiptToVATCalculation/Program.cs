namespace FuelReceiptToVATCalculation
{
    using System;
    using Common.Logging.Converters;
    using Common.Logging.Log4Net;
    using APICallsHelper;
    using Common.Logging;

    /// <summary>
    /// Starts application for calculating the Fuel receipt to mileage allocation
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
        /// Entry point to the console application which process the Fuel receipt to mileage allocation calculation
        /// </summary>
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            LoggingContent["activityid"] = new TraceActivityId();

            Log.Debug("Fuel Receipt to VAT Calculation service has started.");

            new FuelReceiptToVATCalculation(new LogFactory<FuelReceiptToVATCalculation>().GetLogger()).AllocateFuelReceiptToMileage(new RequestHelper(), new ResponseHelper(), new AuthToken());

            Log.Debug("Fuel Receipt to VAT Calculation service has completed.");

            Environment.Exit(0);
        }

        /// <summary>
        /// Handles any errors that arent specifically handled elsewhere
        /// </summary>
        /// <param name="sender">Information on where the error has come from</param>
        /// <param name="e">Object containing the exception that occured</param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            if (Log.IsErrorEnabled)
            {
                Log.Error($"Unhandled exception occurred due to {exception.Message}", exception);
            }

            Environment.Exit(1);
        }
    }
}
