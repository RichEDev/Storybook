namespace EsrNhsHubFileTransferService.Code.Helpers
{
    public static class GlobalVariables
    {
        #region Public Properties

        /// <summary>
        ///     The number of days to delay the deletion of a successfully transferred file.
        /// </summary>
        /// <remarks>
        ///     The ESR specification asks that files are deleted after they are transferred,
        ///     a buffer period may be preferable to keep the file available for re-runs/verification after it has been transferred
        ///     rather than the customer having to raise an SR with the central team to have the file reproduced
        /// </remarks>
        /// <value>Values between 0 and 30 are sensible, defaults to 7 if not set from App.config</value>
        public static int DeleteFilesXDaysAfterProcessed { get; set; }

        /// <summary>
        ///     Enable greater detail in the event logs for the service by setting to true
        /// </summary>
        /// <value>false if not set from App.config</value>
        public static bool ExtendedLogging { get; set; }

        /// <summary>
        ///     How frequently to check for files to transfer
        /// </summary>
        /// <value>Values from 600000 (10 minutes) upwards are sensible, defaults to 1800000 (30 minutes) if not set from App.config</value>
        public static double PollIntervalMilliseconds { get; set; }

        /// <summary>
        ///     Run as soon as the service starts (otherwise waits for the timer to expire)
        /// </summary>
        /// <value>false if not set from App.config</value>
        public static bool StartImmediately { get; set; }

        #endregion
    }
}