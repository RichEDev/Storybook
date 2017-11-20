namespace SpendManagementLibrary.Report
{
    /// <summary>
    /// Returned if the report is in progress or queued.
    /// </summary>
    public class ReportResponseInProgress : ReportResponse
    {
        public ReportResponseInProgress():base(ReportProgress.InProgress)
        {
        }

        /// <summary>
        /// The percentage processed.
        /// </summary>
        public int PercentageProcessed { get; set; }

        /// <summary>
        /// The row count (total).
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// The export type.
        /// </summary>
        public ExportType Exporttype { get; set; }
    }
}
