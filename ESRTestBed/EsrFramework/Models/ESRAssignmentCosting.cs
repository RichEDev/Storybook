namespace EsrFramework.Models
{
    using System;

    public class EsrAssignmentCosting
    {
        #region Public Properties

        public string Analysis1 { get; set; }
        public string Analysis2 { get; set; }
        public string CharitableIndicator { get; set; }
        public string CostCentre { get; set; }
        public int? EsrAssignId { get; set; }
        public long EsrAssignmentId { get; set; }
        public long EsrCostingAllocationId { get; set; }
        public DateTime? EsrLastUpdate { get; set; }
        public long EsrPersonId { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public int? ElementNumber { get; set; }
        public string EntityCode { get; set; }
        public decimal? PercentageSplit { get; set; }
        public string SpareSegment { get; set; }
        public string Subjective { get; set; }

        #endregion
    }
}