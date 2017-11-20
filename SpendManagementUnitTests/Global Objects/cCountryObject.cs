using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    class cCountryObject
    {
        public static cCountry CreateCountry()
        {
            cGlobalCountries clsGlobalCountries = new cGlobalCountries();
            cGlobalCountry tempGlobalCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("GB");

            cCountries clsCountries = new cCountries(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCountry tempCountry = clsCountries.getCountryByGlobalCountryId(tempGlobalCountry.globalcountryid);
            if (tempCountry == null)
            {
                int tempCountryId = clsCountries.saveCountry(new cCountry(0, tempGlobalCountry.globalcountryid, false, null, DateTime.Now, cGlobalVariables.EmployeeID));
                clsCountries = new cCountries(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
                tempCountry = clsCountries.getCountryById(tempCountryId);
            }

            return tempCountry;
        }
    }
}
