namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public struct OnlineCountryInfo
    {
        public Dictionary<int, cCountry> OnlineCountries;
        public Dictionary<int, ForeignVatRate[]> OnlineVatRates;
        public List<int> CountryIds;
    }
}