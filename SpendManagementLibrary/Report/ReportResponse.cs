namespace SpendManagementLibrary.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public enum ReportProgress
    {
        InProgress = 0,
        Complete = 1,
        Error = 2,
        Queued = 3,
        Aborted = 4

    }

    /// <summary>
    /// A base class for the Report Response.
    /// </summary>
    public class ReportResponse
    {
        public ReportResponse(ReportProgress progress)
        {
            this.Progress = progress;
        }

        public int RequestNumber { get; set; }

        public ReportProgress Progress { get; set; }

    }
}
