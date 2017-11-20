using System.Data;

namespace Expenses_Reports.Formula
{
    public interface ICalculationPart
    {
        /// <summary>
        /// The value of the string
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Replace "ROW", "COL" and "[field]" references returning a formula string.
        /// </summary>
        /// <param name="row">The <see cref="DataRow"/>To use when generating the data</param>
        /// <param name="columnIndex">The current Column index (starting at 1)</param>
        /// <param name="rowIndex">The current Row Index (starting at 1)</param>
        /// <returns>The formatted formula string.</returns>
        string ToString(object[] row, int columnIndex, int rowIndex);
    }
}