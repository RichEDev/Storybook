namespace Expenses_Reports.Formula
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the parts of a calculation for Reports etc.
    /// </summary>
    public class Calculation : IEnumerable<ICalculationPart>
    {
        /// <summary>
        /// Create a new instance of <see cref="Calculation"/>
        /// </summary>
        public Calculation()
        {
            this._parts = new List<ICalculationPart>();
        }

        /// <summary>
        /// A private list of <see cref="ICalculationPart"/>
        /// </summary>
        private readonly List<ICalculationPart> _parts;

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ICalculationPart> GetEnumerator()
        {
            return this._parts.GetEnumerator();
        }

        /// <summary>
        /// Add an instance of <see cref="ICalculationPart"/> to the Enumerable list
        /// </summary>
        /// <param name="part"></param>
        public void Add(ICalculationPart part)
        {
            this._parts.Add(part);
        }

        /// <summary>
        /// Output the Calculation with functions and references replaced where possible
        /// </summary>
        /// <param name="row">An instance of <see cref="DataRow"/>to get data from</param>
        /// <param name="columnIndex">The current Column Index (starting at 1)</param>
        /// <param name="rowIndex">The current Row Index (starting at 1)</param>
        /// <returns>A string of the current calculation</returns>
        public string ToString(object[] row, int columnIndex, int rowIndex)
        {
            var result = new StringBuilder();
            foreach (ICalculationPart calculationPart in this._parts)
            {
                result.Append(calculationPart.ToString(row, columnIndex, rowIndex));
            }

            return result.ToString();
        }

        
    }
}
