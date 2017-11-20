using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cMileageCatObject
    {
        
        /// <summary>
        /// Create a mileage category object with associated date range and threshold
        /// </summary>
        /// <returns></returns>
        public static cMileageCat CreateObject()
        {
            cMileagecats clsMileageCats = new cMileagecats(cGlobalVariables.AccountID);

            int nMileageCatID = clsMileageCats.saveVehicleJourneyRate(GetMileageCatObject());
            int nMileageDateID = clsMileageCats.saveDateRange(GetMileageDateRangeObject(nMileageCatID));
            clsMileageCats.saveThreshold(GetMileageThreasholdObject(nMileageDateID));

            cMileageCat cat = clsMileageCats.GetMileageCatById(nMileageCatID);
            return cat;
        }

        /// <summary>
        /// Create a mileage category object with associated date range and threshold
        /// </summary>
        /// <returns></returns>
        public static cMileageCat CreateInvalidObject()
        {
            cMileagecats clsMileageCats = new cMileagecats(cGlobalVariables.AccountID);

            int nMileageCatID = clsMileageCats.saveVehicleJourneyRate(GetMileageCatObject());
            int nMileageDateID = clsMileageCats.saveDateRange(GetInvalidMileageDateRangeObject(nMileageCatID));
            clsMileageCats.saveThreshold(GetInvalidMileageThreasholdObject(nMileageDateID));

            cMileageCat cat = clsMileageCats.GetMileageCatById(nMileageCatID);
            return cat;
        }

        /// <summary>
        /// Geta a template mileage category 
        /// </summary>
        /// <returns></returns>
        private static cMileageCat GetMileageCatObject()
        {
            cCurrencies clsCurrencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            return new cMileageCat(0, "Unit Test " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Unit Tests", ThresholdType.Annual, true, new List<cMileageDaterange>(), MileageUOM.Mile, false, string.Empty, clsCurrencies.getCurrencyByAlphaCode("GBP").currencyid, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
        }

        /// <summary>
        /// Get a template mileage date range
        /// </summary>
        /// <param name="MileageID"></param>
        /// <returns></returns>
        private static cMileageDaterange GetMileageDateRangeObject(int MileageID)
        {
            return new cMileageDaterange(0, MileageID, null, null, new List<cMileageThreshold>(), DateRangeType.Any, DateTime.UtcNow, 0, null, 0);
        }

        /// <summary>
        /// Get an invalid template mileage date range
        /// </summary>
        /// <param name="MileageID"></param>
        /// <returns></returns>
        private static cMileageDaterange GetInvalidMileageDateRangeObject(int MileageID)
        {
            return new cMileageDaterange(0, MileageID, new DateTime(2009, 01, 01), new DateTime(2010, 01, 01), new List<cMileageThreshold>(), DateRangeType.Between, DateTime.UtcNow, 0, null, 0);
        }

        /// <summary>
        /// Get a template mileage threshold
        /// </summary>
        /// <param name="MileageDateID"></param>
        /// <returns></returns>
        private static cMileageThreshold GetMileageThreasholdObject(int MileageDateID)
        {
            return new cMileageThreshold(0, MileageDateID, null, null, RangeType.Any, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25,DateTime.UtcNow,0,null,0, (decimal)0.25);
        }

        /// <summary>
        /// Get a template mileage threshold
        /// </summary>
        /// <param name="MileageDateID"></param>
        /// <returns></returns>
        private static cMileageThreshold GetInvalidMileageThreasholdObject(int MileageDateID)
        {
            return new cMileageThreshold(0, MileageDateID, 100, null, RangeType.LessThan, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, (decimal)0.25, DateTime.UtcNow, 0, null, 0, (decimal)0.25);
        }
    }
}
