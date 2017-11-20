namespace SpendManagementLibrary.Definitions
{
    using System;

    /// <summary>
    /// Simplified request information
    /// </summary>
    [Serializable]
    public class ReportRequestInformation
    {
        public int AccountId { get; set; }

        public DateTime CompletionTime { get; set; }

        public ExportType ExportType { get; set; }

        public int ProcessedRows { get; set; }

        public string ReportJoinSql { get; set; }

        public int ReportMaxRowLimit { get; set; }

        public string ReportName { get; set; }

        public ReportRunFrom ReportRunFrom { get; set; }

        public string ReportStaticSql { get; set; }

        public ReportType ReportType { get; set; }

        public int RowCount { get; set; }

        public bool SchedulerRequest { get; set; }

        public ReportRequestStatus Status { get; set; }

        public int SubAccountId { get; set; }
    }
}
