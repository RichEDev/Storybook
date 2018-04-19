namespace BusinessLogic.GeneralOptions.Reports
{
    using System;

    /// <summary>
    /// Defines a <see cref="ReportOptions"/> and it's members
    /// </summary>
    public class ReportOptions : IReportOptions
    {
        /// <summary>
        /// Gets or sets the drilldown report
        /// </summary>
        public Guid? DrilldownReport { get; set; }
    }
}
