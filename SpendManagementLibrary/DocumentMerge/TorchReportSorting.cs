using System;
using System.Collections.Generic;

namespace SpendManagementLibrary.DocumentMerge
{
    [Serializable]
    public class TorchReportSorting
    {
        public TorchReportSorting()
        {
        }

        public TorchReportSorting(string reportName, List<TorchReportSortingColumn> torchReportSortingColumns)
        {
            ReportName = reportName;
            TorchReportSortingColumns = torchReportSortingColumns;
        }

        public string ReportName { get; set; }
        public List<TorchReportSortingColumn> TorchReportSortingColumns { get; set; }

    }
}
