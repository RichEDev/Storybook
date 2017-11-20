using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cOdometerReadingObject
    {
        /// <summary>
        /// Blank odometer reading entry
        /// </summary>
        /// <returns>Odometer reading with minimal values set</returns>
        public static cOdometerReading BlankTemplate()
        {
            return new cOdometerReading(0, 0, DateTime.UtcNow, 0, 0, DateTime.UtcNow, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        /// Valid odometer reading entry
        /// </summary>
        /// <returns>Odometer reading with basic values set</returns>
        public static cOdometerReading ValidTemplate()
        {
            return new cOdometerReading(0, 0, DateTime.UtcNow, 100, 200, DateTime.UtcNow, cGlobalVariables.EmployeeID);
        }

        /// <summary>
        /// Invalid odometer reading entry
        /// </summary>
        /// <returns>Odometer reading with invalid values set</returns>
        public static cOdometerReading InvalidTemplate()
        {
            return new cOdometerReading(0, 0, DateTime.UtcNow, 100, -100, DateTime.UtcNow.AddYears(1), cGlobalVariables.EmployeeID);
        }

        /// <summary>
        /// Create an odometer reading to add to a car
        /// </summary>
        /// <param name="oCar">The car object to attach the odometer reading to</param>
        /// <param name="oOdometerReading">An odometer reading object, usually from one of the templates (cOdometerReadingObject.BlankTemplate for example)</param>
        /// <param name="nBusinessMiles">Were business miles incurred</param>
        /// <returns>The id of the odometer reading added</returns>
        public static int CreateOdometerReading(ref cCar oCar, cOdometerReading oOdometerReading, byte nBusinessMiles)
        {
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            int nOdometerReadingID;

            try
            {
                nOdometerReadingID = clsEmployees.saveOdometerReading(oOdometerReading.odometerid, oCar.employeeid, oCar.carid, oOdometerReading.datestamp, oOdometerReading.oldreading, oOdometerReading.newreading, nBusinessMiles);
            }
            catch (Exception e)
            {
                return -1;
            }

            return nOdometerReadingID;
        }

        /// <summary>
        /// Delete odometer readings from a car
        /// </summary>
        /// <param name="oCar">The car object to delete the odometer readings from</param>
        /// <returns>Boolean indicating success</returns>
        public static bool DeleteOdometerReadings(ref cCar oCar)
        {
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            try
            {
                foreach (cOdometerReading o in oCar.odometerreadings)
                {
                    clsEmployees.deleteOdometerReading(oCar.employeeid, oCar.carid, o.odometerid);
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
