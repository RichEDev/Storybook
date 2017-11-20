using System;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.Employees;

namespace Spend_Management.shared.code.Mileage
{
    public class ExcessMileage
    {
        /// <summary>
        /// Minus the home to old work distance from the home to new work distance to get the excess mileage.
        /// If the distance is 0 or less than set as 0.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="car"></param>
        /// <param name="date"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static decimal GetRelocationMileage(Employee employee, cCar car, DateTime date, ICurrentUser currentUser)
        {
            var workAddresses = employee.GetWorkAddresses();
            var currentOffice = ExcessMileage.GetCurrentOffice(date, workAddresses, currentUser);
            if (currentOffice == null)
            {
                return 0;
            }

            var oldOffice = ExcessMileage.GetOldOfficeId(workAddresses, currentOffice);
            var homeLocationId = GetHomeLocationId(date, employee.GetHomeAddresses());

            if (homeLocationId > 0 && oldOffice > 0)
            {
                var subaccount = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId);
                var distance = ExcessMileage.CalculateRelocationMileageDistance(car, currentUser, homeLocationId, oldOffice, subaccount, currentOffice.LocationID);
                return distance;
            }

            return 0;
        }

        /// <summary>
        /// Return the current Office work location for the given date and esrLocation (if Esr GO 2)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="workAddresses"></param>
        /// <param name="currentUser"></param>
        /// <returns>The selected work location or null</returns>
        internal static cEmployeeWorkLocation GetCurrentOffice(DateTime date, EmployeeWorkAddresses workAddresses, ICurrentUserBase currentUser)
        {
            var currentOffice = workAddresses.GetBy(date);
           
            return currentOffice;
        }

        /// <summary>
        /// Return the current home location based on the date given.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="homeAdresses"></param>
        /// <returns></returns>
        internal static int GetHomeLocationId(DateTime date, EmployeeHomeAddresses homeAdresses)
        {
            var homeLocation = homeAdresses.GetBy(date);
            var homeLocationId = homeLocation != null ? homeLocation.LocationID : 0;
            return homeLocationId;
        }

        /// <summary>
        /// Return the Id of the previous work location, assuming that the location is a different Esr Location.
        /// </summary>
        /// <param name="workAddresses"></param>
        /// <param name="currentOffice"></param>
        /// <returns></returns>
        internal static int GetOldOfficeId(EmployeeWorkAddresses workAddresses, cEmployeeWorkLocation currentOffice)
        {
            if (currentOffice == null || currentOffice.StartDate == null)
            {
                return 0;
            }

            var oldOfficeLocation = workAddresses.GetPreviousWorkLocation(currentOffice);
            return oldOfficeLocation != null ? oldOfficeLocation.LocationID : 0;
        }

        /// <summary>
        /// Calculate Relocation (Excess) mileage
        /// </summary>
        /// <param name="car"></param>
        /// <param name="currentUser"></param>
        /// <param name="homeLocationId"></param>
        /// <param name="oldOffice"></param>
        /// <param name="subaccount"></param>
        /// <param name="currentOfficeId"></param>
        /// <returns></returns>
        internal static decimal CalculateRelocationMileageDistance(cCar car, ICurrentUser currentUser, int homeLocationId,
            int oldOffice, cAccountSubAccount subaccount, int currentOfficeId)
        {
            var distance1 =
                AddressDistance.GetRecommendedOrCustomDistance(homeLocationId, oldOffice, currentUser.AccountID, subaccount,
                    currentUser) ?? 0;
            var distance2 =
                AddressDistance.GetRecommendedOrCustomDistance(homeLocationId, currentOfficeId, currentUser.AccountID,
                    subaccount, currentUser) ?? 0;

            var distance = distance2 - distance1;

            if (distance <= 0)
            {
                {
                    return distance;
                }
            }

            if (car != null && car.defaultuom == MileageUOM.KM)
            {
                distance = cMileagecats.ConvertMilesToKilometres(distance);
            }

            return distance;
        }
    }
}
