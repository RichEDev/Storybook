namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public struct OnlineGlobalCountryInfo
    {
        public Dictionary<int, cGlobalCountry> OnlineGlobalCountries;
        public List<int> GlobalCountryIds;
    }
}