using System.Collections.Generic;

namespace SpendManagementLibrary.DocumentMerge
{
    class TorchSummaryReport
    {
        public string ReportName { get; set; }

        public List<TorchSummaryColumn> ReportSortingColumns { get; set; }

        TorchSummaryReport()
        {
            ReportSortingColumns = new List<TorchSummaryColumn>();
        }
    }
}
