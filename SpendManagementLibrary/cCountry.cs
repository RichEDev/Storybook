using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cCountry
    {
        public cCountry()
        {   
        }

        public cCountry(int countryId, int globalCountryId, bool archived, Dictionary<int, ForeignVatRate> vatRates, DateTime createdOn, int? createdBy)
        {
            this.CountryId = countryId;
            this.GlobalCountryId = globalCountryId;
            this.Archived = archived;
            this.CreatedOn = createdOn;
            this.CreatedBy = createdBy;
            this.VatRates = vatRates;
        }

        public int CountryId { get; private set; }

        public int GlobalCountryId { get; private set; }

        public bool Archived { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public int? CreatedBy { get; private set; }

        public Dictionary<int, ForeignVatRate> VatRates { get; private set; }
    }
}
