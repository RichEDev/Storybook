using System.Data;
using SpendManagementLibrary;

namespace Expenses_Reports.Formula
{
    /// <summary>
    /// A class holind ginformation mapping a <see cref="cReportColumn"/> to an <seealso cref="DataColumn"/>
    /// </summary>
    public class ColumnLookup
    {
        /// <summary>
        /// The instance of <see cref="DataColumn"/> for this column
        /// </summary>
        public DataColumn DataColumn { get; set; }

        /// <summary>
        /// The instance of <see cref="cReportColumn"/> for this column
        /// </summary>
        public cReportColumn ReportColumn { get; set; }

        /// <summary>
        /// The Name of the column 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the column including any SQL functions e.g. "SUM of Total"
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// The instance of <see cref="Calculation"/> derived from the <see cref="DataColumn"/> and <see cref="cCalculatedColumn"/>
        /// </summary>
        public Calculation Calculation { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="ColumnLookup"/>
        /// </summary>
        /// <param name="name">The name of the column</param>
        /// <param name="dataColumn">The instance of <see cref="DataColumn"/>for this column</param>
        /// <param name="reportColumn">The instance of <see cref="cReportColumn"/>for this column</param>
        public ColumnLookup(string name,  DataColumn dataColumn, cReportColumn reportColumn)
        {
            this.Name = name;
            this.SetFunctionName(name, reportColumn);
            this.DataColumn = dataColumn;
            this.ReportColumn = reportColumn;
            this.Calculation = null;
        }

        /// <summary>
        /// Set the Functon name property based on the current name plus any function set on the <seealso cref="cReportColumn"/>
        /// </summary>
        /// <param name="name">The base name</param>
        /// <param name="reportColumn">The <see cref="cReportColumn"/>for this column</param>
        private void SetFunctionName(string name, cReportColumn reportColumn)
        {
            if (reportColumn is cStandardColumn)
            {
                var standard = (cStandardColumn) reportColumn;
                if (standard.funcavg)
                {
                    this.FunctionName = "AVG of " + this.Name;
                }
                if (standard.funccount)
                {
                    this.FunctionName = "COUNT of " + this.Name;
                }
                if (standard.funcmax)
                {
                    this.FunctionName = "MAX of " + this.Name;
                }
                if (standard.funcmin)
                {
                    this.FunctionName = "MIN of " + this.Name;
                }
                if (standard.funcsum)
                {
                    this.FunctionName = "SUM of " + this.Name;
                }
            }
            this.FunctionName = name;

        }
    }
}