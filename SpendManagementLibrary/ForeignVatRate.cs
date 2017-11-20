namespace SpendManagementLibrary
{
    using System;

    [Serializable()]
    public struct ForeignVatRate
    {
        public int CountryId { get; set; }
        public int SubcatId { get; set; }
        public double Vat { get; set; }
        public double VatPercent { get; set; }
    }
}