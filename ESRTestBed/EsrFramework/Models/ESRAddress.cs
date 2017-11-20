namespace EsrFramework.Models
{
    using System;

    public class EsrAddress
    {
        #region Public Properties

        public string AddressCountry { get; set; }
        public string AddressCounty { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressPostcode { get; set; }
        public string AddressStyle { get; set; }
        public string AddressTown { get; set; }
        public string AddressType { get; set; }
        public long EsrAddressId { get; set; }
        public DateTime? EsrLastUpdate { get; set; }
        public long? EsrPersonId { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public string PrimaryFlag { get; set; }

        #endregion
    }
}