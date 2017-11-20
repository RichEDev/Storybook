namespace EsrFramework.Models
{
    using System;

    public class EsrPhone
    {
        #region Public Properties

        public DateTime? EsrLastUpdate { get; set; }
        public long EsrPersonId { get; set; }
        public long EsrPhoneId { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneType { get; set; }

        #endregion
    }
}