using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cCarObject
    {
        /// <summary>
        /// Blank car template
        /// </summary>
        /// <returns>Car with minimal values set</returns>
        public static cCar BlankTemplate()
        {
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, "Unit Test", " Unit Test", "NoReg", null, null, true, new List<int>(), 1, 0, false, 0, null, null, 0, string.Empty, null, null, 0, string.Empty, null, null, 0, null, null, 0, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 0, null, 0, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Valid car template
        /// </summary>
        /// <returns>Car with basic valid values set</returns>
        public static cCar ValidTemplate()
        {
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, "Unit Test Make", "Unit Test Model", "X11 ABC " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), true, new List<int>(), 1, 0, false, 0, null, null, 0, string.Empty, null, null, 0, string.Empty, null, null, 0, null, null, 0, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Inactive car template
        /// </summary>
        /// <returns>Car with basic values set, but not activated</returns>
        public static cCar InactiveTemplate()
        {
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, "Unit Test Make", "Unit Test Model", "X11 ABC " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), false, new List<int>(), 1, 0, false, 0, null, null, 0, string.Empty, null, null, 0, string.Empty, null, null, 0, null, null, 0, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Car template that should fail duty of care tests
        /// </summary>
        /// <returns>Car with basic values set, expired tax/mot/insurance</returns>
        public static cCar FailsDutyOfCareTemplate(bool active = true)
        {
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, "Unit Test Make", "Unit Test Model", "X11 ABC " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), active, new List<int>(), 1, 0, false, 0, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, "123456789", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, "123456789", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Car template that should pass duty of care date tests
        /// </summary>
        /// <returns>Car with basic values set, in-date tax/mot/insurance</returns>
        public static cCar PassesDutyOfCareTemplate(bool active = true)
        {
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, "Unit Test Make", "Unit Test Model", "X11 ABC " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), active, new List<int>(), 1, 0, false, 0, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, "123456789", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, "123456789", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Car template that should fail duty of care tests
        /// </summary>
        /// <returns>Car with basic values set, expired tax/mot/insurance</returns>
        public static cCar FailsDutyOfCareTemplate(bool passesLicenceExpiry, bool passesMOT, bool passesInsurance, bool passesService)
        {
            DateTime MOTExpiryDate = DateTime.Now.AddDays(-10);
            DateTime taxExpiryDate = DateTime.Now.AddDays(-10);
            DateTime insuranceExpiryDate = DateTime.Now.AddDays(-10);
            DateTime serviceExpiryDate = DateTime.Now.AddDays(-10);

            if (!passesInsurance)
            {

            }

            if (!passesLicenceExpiry)
            {

            }
            if (!passesMOT)
            {

            }
            if (!passesService)
            {

            }
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, "Unit Test Make", "Unit Test Model", "X11 ABC " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), true, new List<int>(), 1, 0, false, 0, taxExpiryDate, DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, "123456789", MOTExpiryDate, DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, "123456789", insuranceExpiryDate, DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, serviceExpiryDate, DateTime.UtcNow.AddYears(-1), cGlobalVariables.DelegateID, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Invalid car template
        /// </summary>
        /// <returns>Car with some incorrect values set</returns>
        public static cCar InvalidTemplate()
        {
            return new cCar(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID, 0, string.Empty, string.Empty, string.Empty, null, null, true, new List<int>(), 0, 0, true, 0, null, null, 0, string.Empty, null, null, 0, string.Empty, null, null, 0, null, null, 0, new SortedList<int, object>(), new cOdometerReading[1] { cOdometerReadingObject.InvalidTemplate() }, string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Blank pool car template
        /// </summary>
        /// <returns>Pool car with minimal values set</returns>
        public static cCar ValidPoolCarTemplate()
        {
            return new cCar(cGlobalVariables.AccountID, 0, 0, "Unit Test Make", "Unit Test Model", "X11 ABC " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7), true, new List<int>(), 1, 0, false, 0, null, null, 0, string.Empty, null, null, 0, string.Empty, null, null, 0, null, null, 0, new SortedList<int, object>(), new cOdometerReading[0], string.Empty, string.Empty, string.Empty, string.Empty, MileageUOM.Mile, 1000, DateTime.UtcNow, 0, null, null, true, false, null, null, null, null);
        }

        /// <summary>
        /// Create and return a saved car id
        /// </summary>
        /// <param name="oCar">Either a full cCar object or from one of the templates (cCarObject.BlankTemplate() for example)</param>
        /// <returns>Saved Car ID</returns>
        public static int CreateID(cCar oCar, int instanciateUsingEmployeeID = 0)
        {
            cCarsBase clsCars = null;

            if (instanciateUsingEmployeeID > 0)
            {
                clsCars = new cEmployeeCars(cGlobalVariables.AccountID, cGlobalVariables.EmployeeID);
            }
            else
            {
                clsCars = new cPoolCars(cGlobalVariables.AccountID);
            }

            int carID;

            try
            {
                carID = clsCars.SaveCar(oCar);
            }
            catch (Exception e)
            {
                return -1;
            } 
            
            return carID;
        }

        /// <summary>
        /// Create and return a saved car object
        /// </summary>
        /// <param name="oCar">Either a full cCar object or from one of the templates (cCarObject.BlankTemplate() for example)</param>
        /// <returns>Saved Car Object</returns>
        public static cCar CreateObject(cCar oCar, int? instanciateUsingEmployeeID = null, bool isPoolCar = false)
        {
            cCarsBase clsCars = null;

            if (isPoolCar == false)
            {
                if (instanciateUsingEmployeeID.HasValue == false)
                {
                    instanciateUsingEmployeeID = cGlobalVariables.EmployeeID;
                }
                clsCars = new cEmployeeCars(cGlobalVariables.AccountID, instanciateUsingEmployeeID.Value);
            }
            else
            {
                clsCars = new cPoolCars(cGlobalVariables.AccountID);
            }
            
            try
            {
                oCar.carid = clsCars.SaveCar(oCar);
            }
            catch (Exception e)
            {
                return null;
            } 

            return oCar;
        }

        /// <summary>
        /// Delete a saved car
        /// </summary>
        /// <param name="oCar">The cCar to delete</param>
        /// <returns>Boolean indicating success</returns>
        public static bool DeleteCar(cCar oCar, int? instanciateUsingEmployeeID = null, bool isPoolCar = false)
        {
            cCarsBase clsCars = null;

            if (isPoolCar == false)
            {
                if (instanciateUsingEmployeeID.HasValue == false)
                {
                    instanciateUsingEmployeeID = cGlobalVariables.EmployeeID;
                }
                clsCars = new cEmployeeCars(cGlobalVariables.AccountID, instanciateUsingEmployeeID.Value);
            }
            else
            {
                clsCars = new cPoolCars(cGlobalVariables.AccountID);
            }

            try
            {
                if (oCar.employeeid == 0)
                {
                    ((cPoolCars)clsCars).DeletePoolCarUsers(oCar.carid);
                }

                clsCars.DeleteCar(oCar.carid);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
