using System;
using System.Collections.Generic;

namespace SpendManagementLibrary.DocumentMerge
{
    /// <summary>
    /// Holds the grouping, filtering, sorting and report sorting information for a given grouping configuration id
    /// </summary>
    [Serializable]
    public class TorchGroupingConfiguration
    {
        #region Properties
        /// <summary>
        /// The grouping configuration id
        /// </summary>
        public int GroupingConfigurationId { get; set; }

        /// <summary>
        /// The merge project id
        /// </summary>
        public int MergeProjectId { get; set; }

        /// <summary>
        /// User selected grouping columns pre ordered in a list
        /// </summary>
        public List<string> GroupingColumnsList { get; set; }

        /// <summary>
        /// User selected sorting columns pre ordered in a list
        /// </summary>
        public List<SortingColumn> SortingColumnsList { get; set; }

        /// <summary>
        /// User selected filtering columns and information pre ordered in a list
        /// </summary>
        public List<TorchGroupingFieldFilter> FilteringColumns { get; set; }

        /// <summary>
        /// the source names selected to use for grouping
        /// </summary>
        public List<string> GroupingSources { get; set; }

        /// <summary>
        ///  The report sorting configurations
        /// </summary>
        public List<TorchReportSorting> ReportSortingConfigurations { get; set; }

        /// <summary>
        /// the label for the configuration
        /// </summary>
        public string ConfigurationLabel { get; set; }

        /// <summary>
        /// The description of the configuration
        /// </summary>
        public string ConfigurationDescription { get; set; }

        #endregion

        public TorchGroupingConfiguration()
        {
        }

        public TorchGroupingConfiguration(int projectConfigurationId, int projectId, List<string> groupingColumns, List<SortingColumn> sortingColumns, List<TorchGroupingFieldFilter> groupingFieldFilters, List<string> sources, List<TorchReportSorting> reportSortingConfigurations, string configurationLabel = "", string configurationDescription = "")
        {
            GroupingConfigurationId = projectConfigurationId;
            MergeProjectId = projectId;
            GroupingColumnsList = groupingColumns;
            SortingColumnsList = sortingColumns;
            FilteringColumns = groupingFieldFilters;
            GroupingSources = sources;
            ReportSortingConfigurations = reportSortingConfigurations;
            ConfigurationLabel = configurationLabel;
            ConfigurationDescription = configurationDescription;
        }

    }
}
