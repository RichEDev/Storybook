namespace SpendManagementLibrary.Report
{
    using System.Collections.Generic;
    /// <summary>
    /// Returned when a report is complete, this contains the report data and columns.
    /// </summary>
    public class ReportResponseComplete :ReportResponse
    {
        public ReportResponseComplete():base(ReportProgress.Complete)
        {
        }

        /// <summary>
        /// The columns for this report.
        /// </summary>
        public List<ReportDataColumn> Columns { get; set; }

        /// <summary>
        /// The data for this report.
        /// </summary>
        public List<Dictionary<string, object>> Data { get; set; }

        /// <summary>
        /// Gets or sets the Path of the Chart image (if any).
        /// </summary>
        public string ChartPath { get; set; }
    }
}
