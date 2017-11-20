namespace EsrFramework.Models
{
    public class AddressEsrAllocation
    {
        #region Public Properties

        public int AddressEsrAllocationId { get; set; }
        public int AddressId { get; set; }
        public long? EsrAddressId { get; set; }
        public long? EsrLocationId { get; set; }
        public int? Companyid { get; set; }

        #endregion
    }
}