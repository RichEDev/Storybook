using System;

namespace SpendManagementLibrary.DocumentMerge
{
    /// <summary>
    /// Holds the column name and sort direction information for a sorted report column
    /// </summary>
    [Serializable]
    public class TorchReportSortingColumn
    {
        #region Properties

        /// <summary>
        /// The column name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The sorting order
        /// </summary>
        public TorchSummaryColumnSortDirection SortingOrder { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public TorchReportSortingColumn() { }

        public TorchReportSortingColumn(string columnName, TorchSummaryColumnSortDirection sortingOrder)
        {
            ColumnName = columnName;
            SortingOrder = sortingOrder;
        }
    }
}


