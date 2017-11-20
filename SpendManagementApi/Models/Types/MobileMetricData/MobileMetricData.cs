namespace SpendManagementApi.Models.Types.MobileMetricData
{

    using System.Collections.Generic;

    /// <summary>
    /// The mobile metric data class.
    /// </summary>
    public class MobileMetricData : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the Metric Data.
        /// </summary>
        public IList<KeyValuePair<string, string>> MetricData { get; set; }
    }
}