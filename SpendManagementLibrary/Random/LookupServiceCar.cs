using System.Collections.Generic;

namespace SpendManagementLibrary.Random
{
    using System;

    /// <summary>
    /// A class inherited from <seealso cref="cCar"/> which also has properties for Tax and MOT expiry.
    /// </summary>
    public class LookupServiceCar : cCar
    {
        /// <summary>
        /// Create a new instance of <see cref="LookupServiceCar"/>
        /// </summary>
        /// <param name="accountid">the ID of the account for this vehicle</param>
        /// <param name="employeeid">The ID of the employee (0 for a pool car)</param>
        /// <param name="carid">The ID of the car (0 for a new car)</param>
        /// <param name="make">The make of the vehicle</param>
        /// <param name="model">The model of the vehicle</param>
        /// <param name="regno">The registration number</param>
        /// <param name="startdate">The start date of usage of the vehicle</param>
        /// <param name="enddate">The end date of usage of the vehicle</param>
        /// <param name="active">True if this vehicle is active (depending on the duty of care beinf off)</param>
        /// <param name="mileagecats">A list of <seealso cref="cMileageCat"/>used for this vehicle</param>
        /// <param name="vehicleEngineTypeId">The engine type of the vehicle</param>
        /// <param name="odometer">Start odometer reading</param>
        /// <param name="fuelcard">True if a fuel card is used for this vehicle</param>
        /// <param name="endodometer">The end odometer reading</param>
        /// <param name="defaultuom">Default Units</param>
        /// <param name="engineSize">The size of the engine in cc</param>
        /// <param name="createdon">The Date created</param>
        /// <param name="createdby">The ID of the employee who created the vehicle</param>
        /// <param name="modifiedon">The modified date (if any)</param>
        /// <param name="modifiedby">The modifier (if any)</param>
        /// <param name="approved">True if the vehicle is approved for use</param>
        /// <param name="exemptfromhometooffice">True if the vehicle is exempt from any hoe to office rules</param>
        /// <param name="VehicleTypeId">The type of vehicle </param>
        /// <param name="taxExpiry">The date of the Tax expiry</param>
        /// <param name="isTaxValid">The tax status "Taxed" or not</param>
        /// <param name="motExpiry">The date of the MOT</param>
        /// <param name="isMotValid">The MOT status "MOT" or not</param>
        public LookupServiceCar(int accountid, int employeeid, int carid, string make, string model, string regno, DateTime? startdate, DateTime? enddate, bool active, List<int> mileagecats, int vehicleEngineTypeId, long odometer, bool fuelcard, int endodometer, MileageUOM defaultuom, int engineSize, DateTime? createdon, int createdby, DateTime? modifiedon, int? modifiedby, bool approved, bool exemptfromhometooffice, byte? VehicleTypeId, DateTime? taxExpiry, bool isTaxValid, DateTime? motExpiry, bool isMotValid) : base(accountid, employeeid, carid, make, model, regno, startdate, enddate, active, mileagecats, vehicleEngineTypeId, odometer, fuelcard, endodometer, defaultuom, engineSize, createdon, createdby, modifiedon, modifiedby, approved, exemptfromhometooffice, VehicleTypeId, taxExpiry, isTaxValid, motExpiry, isMotValid)
        {
        }
    }
}
