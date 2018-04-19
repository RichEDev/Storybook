namespace BusinessLogic.GeneralOptions.Reports
{
    using System;

    /// <summary>
    /// Defines a <see cref="IReportOptions"/> and it's members
    /// </summary>
    public interface IReportOptions
    {
        /// <summary>
        /// Gets or sets the drilldown report
        /// </summary>
        Guid? DrilldownReport { get; set; }
    }
}
