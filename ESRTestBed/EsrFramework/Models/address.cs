namespace EsrFramework.Models
{
    using System;

    public class Address
    {
        #region Public Properties

        public bool AccountWideFavourite { get; set; }
        public int AddressId { get; set; }
        public string AddressName { get; set; }
        public string AddressNameLookup { get; set; }
        public bool Archived { get; set; }
        public string City { get; set; }
        public string CityLookup { get; set; }
        public int? Country { get; set; }
        public string County { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreationMethod { get; set; }
        public string GlobalIdentifier { get; set; }
        public string Latitude { get; set; }
        public string Line1 { get; set; }
        public string Line1Lookup { get; set; }
        public string Line2 { get; set; }
        public string Line2Lookup { get; set; }
        public string Line3 { get; set; }
        public string Longitude { get; set; }
        public DateTime? LookupDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool Obsolete { get; set; }
        public string Postcode { get; set; }
        public string PostcodeLookup { get; set; }
        public int? SubAccountId { get; set; }
        public int Udprn { get; set; }

        #endregion
    }
}