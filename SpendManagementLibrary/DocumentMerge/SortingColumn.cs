using System;


namespace SpendManagementLibrary.DocumentMerge
{
    [Serializable]
    public class SortingColumn
    {
        public SortingColumn()
        {
        }

        public SortingColumn(string columnName, string sortType)
        {
            this.Name = columnName;
            this.DocumentSortType = sortType;
        }

        public string Name { get; set; }

        public string DocumentSortType { get; set; }
    }
}
