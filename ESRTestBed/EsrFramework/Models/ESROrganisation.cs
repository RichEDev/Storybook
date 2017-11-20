namespace EsrFramework.Models
{
    using System;

    public class EsrOrganisation
    {
        #region Public Properties

        public string CostCentreDescription { get; set; }
        public string DefaultCostCentre { get; set; }
        public DateTime? EsrLastUpdateDate { get; set; }
        public long? EsrLocationId { get; set; }
        public long EsrOrganisationId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? HierarchyVersionFrom { get; set; }
        public long HierarchyVersionId { get; set; }
        public DateTime? HierarchyVersionTo { get; set; }
        public string NacsCode { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationType { get; set; }
        public long? ParentOrganisationId { get; set; }

        #endregion
    }
}