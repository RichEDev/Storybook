namespace SpendManagementLibrary.DocumentMerge
{
    public class TorchSummaryColumn
    {
        public string ColumnOrdinal { get; set; }

        public TorchSummaryColumnSortDirection ColumnSortDirection { get; set; }
    }

    public enum TorchSummaryColumnSortDirection
    {
        Ascending = 1,
        Descending = 2
    }
}
