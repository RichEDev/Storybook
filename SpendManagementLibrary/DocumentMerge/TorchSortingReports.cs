namespace SpendManagementLibrary.DocumentMerge
{
    /// <summary>
    /// 
    /// </summary>
    public class TorchSortingReports
    {
        #region Properties

        public int ReportId { get; set; }
        public string ReportName { get; set; }

        #endregion

        public TorchSortingReports (int reportId,string reportName)
        {
            ReportId = reportId;
            ReportName = reportName;
        }

    }
}
