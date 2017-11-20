namespace SpendManagementLibrary.Report
{
    public class ReportAborted : ReportResponse
    {
        /// <summary>
        /// An instance of <see cref="ReportResponse"/> used when the report instance has been aborted by the user.
        /// </summary>
        public ReportAborted() : base(ReportProgress.Aborted)
        {
        }
    }
}