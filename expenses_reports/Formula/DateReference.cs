using System;
using System.Data;

namespace Expenses_Reports.Formula
{
    public class DateReference : ICalculationPart
    {
        public string Value { get; }

        /// <summary>
        /// The <see cref="DataTable"/> Column Ordinal
        /// </summary>
        public DataColumn Column { get; set; }

        /// <summary>
        /// The name of the column (as used in the Formula)
        /// </summary>
        public string ColumnBaseName { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="ColumnReference"/>
        /// </summary>
        /// <param name="column">The <see cref="DataColumn"/> of this reference</param>
        /// <param name="columnBaseName">The Column name of this reference (Used as [ColumnName])</param>
        /// <param name="value">The original value of the string.</param>
        public DateReference(DataColumn column, string columnBaseName, string value)
        {
            this.Column = column;
            this.ColumnBaseName = columnBaseName;
            this.Value = value;
        }

        /// <summary>
        /// Replace "ROW", "COL" and "[field]" references returning a formula string.
        /// </summary>
        /// <param name="row">The <see cref="DataRow"/>To use when generating the data</param>
        /// <param name="columnIndex">The current Column index (starting at 1)</param>
        /// <param name="rowIndex">The current Row Index (starting at 1)</param>
        /// <returns>The formatted formula string.</returns>
        public string ToString(object[] row, int columnIndex, int rowIndex)
        {
            var resultColumn = this.Column;
            var result = row[this.Column.Ordinal];
            if (resultColumn.DataType == typeof(string) || resultColumn.DataType == typeof(decimal))
            {
                return "\"" + result + "\"";
            }

            if (result == DBNull.Value && resultColumn.DataType == typeof(decimal))
            {
                return "0";
            }

            if (resultColumn.DataType == typeof(DateTime))
            {
                var dateValue = (DateTime)result;
                return $"'{dateValue.ToString("dd/MM/yyyy")}'";
            }

            if (resultColumn.DataType == typeof(bool))
            {
                if (bool.Parse(result.ToString()))
                {
                    return "TRUE()";
                }

                return "FALSE()";
            }

            return result.ToString();
        }
    }
}