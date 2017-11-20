namespace SpendManagementLibrary.Report
{
    public enum AggregateFunction
    {
        None = 0,
        Count = 1,
        Max = 2,
        Min = 3,
        Average = 4,
        Sum = 5
    }

    /// <summary>
    /// A column included in a report
    /// </summary>
    public class ReportDataColumn
    {
        /// <summary>
        /// The field name for this column.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Is this field the primary key
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// The text to display for the header
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// The format to use (if necessary)
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The field type
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// The sort direction of this column
        /// </summary>
        public int SortDirection { get; private set; }

        /// <summary>
        /// The aggregate function for this column.
        /// </summary>
        public AggregateFunction AggregateFunction { get; private set; }

        /// <summary>
        /// Wether this column if grouped
        /// </summary>
        public bool GroupBy { get; private set; }

        /// <summary>
        /// If this column is hidden.
        /// </summary>
        public bool Hidden { get; private set; }

        /// <summary>
        /// Creaqte a new report column from a cReportColumn object
        /// </summary>
        /// <param name="column"></param>
        public ReportDataColumn(cReportColumn column)
        {
            if (column is cStandardColumn)
            {
                var standard = (cStandardColumn)column;
                var function = string.Empty;
                if (standard.funcsum)
                {
                    function = "_SUM";
                }
                else if (standard.funcavg)
                {
                    function = "_AVG";
                }
                else if (standard.funccount)
                {
                    function = "_COUNT";
                }
                else if (standard.funcmax)
                {
                    function = "_MAX";
                }
                else if (standard.funcmin)
                {
                    function = "_MIN";
                }

                this.FieldName = string.Format("{0}{1}", standard.order, function);
                this.HeaderText = string.IsNullOrEmpty(standard.DisplayName) ? standard.field.Description : standard.DisplayName;
                this.Format = standard.field.FieldType == "D" ? "{0:dd/MM/yyyy}" : null;
                this.FieldType = standard.field.FieldType;
                this.SortDirection = (int)standard.sort;
                if (standard.funcavg)
                {
                    this.AggregateFunction = AggregateFunction.Average;
                    this.HeaderText = "AVG of " + this.HeaderText;
                }

                if (standard.funccount)
                {
                    this.AggregateFunction = AggregateFunction.Count;
                    this.HeaderText = "COUNT of " + this.HeaderText;
                }

                if (standard.funcmax)
                {
                    this.AggregateFunction = AggregateFunction.Max;
                    this.HeaderText = "MAX of " + this.HeaderText;
                }

                if (standard.funcmin)
                {
                    this.AggregateFunction = AggregateFunction.Min;
                    this.HeaderText = "MIN of " + this.HeaderText;
                }

                if (standard.funcsum)
                {
                    this.AggregateFunction = AggregateFunction.Sum;
                    this.HeaderText = "SUM of " + this.HeaderText;
                }

                this.GroupBy = standard.groupby;
                this.Hidden = standard.hidden;
            }

            if (column is cStaticColumn)
            {
                this.FieldName = ((cStaticColumn)column).order.ToString();
                this.HeaderText = ((cStaticColumn)column).literalname;
                this.Format = null;
                this.FieldType = "Z";
            }

            if (column is cCalculatedColumn)
            {
                this.FieldName = ((cCalculatedColumn)column).order.ToString();
                this.HeaderText = ((cCalculatedColumn)column).columnname;
                this.Format = null;
                this.FieldType = "Z";
            }

        }
    }
}
