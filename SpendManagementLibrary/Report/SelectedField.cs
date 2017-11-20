namespace Spend_Management.shared.reports
{
    using System;
    /// <summary>
    /// The selected fields from the report designer.
    /// </summary>
    public struct SelectedField
    {
        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string attributename { get; set;}

        /// <summary>
        /// Gets or sets the bread crumbs.
        /// </summary>
        public string crumbs { get; set;}

        /// <summary>
        /// Gets or sets the JOINVIAID.
        /// </summary>
        public int joinviaid { get; set;}

        /// <summary>
        /// Gets or sets the column ID.
        /// </summary>
        public string id { get; set;}

        /// <summary>
        /// Gets or sets the field ID.
        /// </summary>
        public Guid fieldid { get; set;}

        /// <summary>
        /// Gets or sets the sort. None, up or down.
        /// </summary>
        public int sort { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether to hide the column.
        /// </summary>
        public bool hide { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether count.
        /// </summary>
        public bool count { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether max.
        /// </summary>
        public bool max { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether min.
        /// </summary>
        public bool min { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether average.
        /// </summary>
        public bool average { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether sum.
        /// </summary>
        public bool sum { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether to group by this column.
        /// </summary>
        public bool groupby { get; set;}

        /// <summary>
        /// Gets or sets the COLUMNID.
        /// </summary>
        public string columnid { get; set;}

        /// <summary>
        /// Gets or sets the literal value.  
        /// The formula for calculated columns and the static text for static columns.
        /// </summary>
        public string literalValue { get; set;}

        /// <summary>
        /// Gets or sets the literalName.  The column name for Static and Calculated columns.
        /// </summary>
        public string literalName { get; set;}
    }
}