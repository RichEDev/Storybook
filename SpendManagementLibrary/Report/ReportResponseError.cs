namespace SpendManagementLibrary.Report
{

    /// <summary>
    /// Returned if there is a report error.
    /// </summary>
    public class ReportResponseError: ReportResponse
    {
        public ReportResponseError(): base(ReportProgress.Error)
        {
        }
    }
}
