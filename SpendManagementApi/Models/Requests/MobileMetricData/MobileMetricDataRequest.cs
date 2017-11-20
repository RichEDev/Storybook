namespace SpendManagementApi.Models.Requests.MobileMetricData
{
    using SpendManagementApi.Models.Common;
    using System.Collections.Generic;

    /// <summary>
    /// The mobile metric data request.
    /// </summary>
    public class MobileMetricDataRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the metric data.
        /// </summary>
        public IList<KeyValuePair<string, string>> MetricData { get; set; }
    }
}