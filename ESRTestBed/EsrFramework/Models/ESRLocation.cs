namespace EsrFramework.Models
{
    using System;

    public class EsrLocation
    {
        #region Public Properties

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string Description { get; set; }
        public DateTime EsrLastUpdate { get; set; }
        public long EsrLocationId { get; set; }
        public string Fax { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string LocationCode { get; set; }
        public string PayslipDeliveryPoint { get; set; }
        public string Postcode { get; set; }
        public string SiteCode { get; set; }
        public string Telephone { get; set; }
        public string Town { get; set; }
        public string WelshAddress1 { get; set; }
        public string WelshAddress2 { get; set; }
        public string WelshAddress3 { get; set; }
        public string WelshLocationTranslation { get; set; }
        public string WelshTownTranslation { get; set; }

        #endregion
    }
}