using System.Data;
using System.Globalization;

namespace Expenses_Reports.Formula
{
    public class StaticText : ICalculationPart
    {
        /// <summary>
        /// The value of the string
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Create an instance of <see cref="StaticText"/>
        /// </summary>
        /// <param name="value">The value of the string</param>
        public StaticText(string value)
        {
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
            return this.ReplacePositionFunctionsWithValues(this.Value, rowIndex, columnIndex);
        }

        /// <summary>
        /// Replace any instances of "ROW" and "COL" functions with the current rows and columns
        /// </summary>
        /// <param name="replacementValue">The function string containg and "ADDRESS"</param>
        /// <param name="row">The current Row index (zero based)</param>
        /// <param name="column">The current Column index(zero based)</param>
        /// <returns></returns>
        private string ReplacePositionFunctionsWithValues(string replacementValue, int row,
            int column)
        {
            if (replacementValue.Contains("ROW()"))
            {
                replacementValue = replacementValue
                    .Replace("ROW()", (row + 1).ToString(CultureInfo.InvariantCulture));
            }

            if (replacementValue.Contains("COLUMN()"))
            {
                replacementValue = replacementValue
                    .Replace("COLUMN()", (column + 1).ToString(CultureInfo.InvariantCulture));
            }

            if (replacementValue.Contains("COL()"))
            {
                replacementValue = replacementValue
                    .Replace("COL()", (column + 1).ToString(CultureInfo.InvariantCulture));
            }

            return replacementValue;
        }
    }
}