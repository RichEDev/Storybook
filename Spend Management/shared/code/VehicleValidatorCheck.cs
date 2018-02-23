namespace Spend_Management.shared.code
{
    using SpendManagementLibrary.Interfaces;
    using DutyOfCareAPI.DutyOfCare;
    using Bootstrap;
    using SpendManagementLibrary;
    using Spend_Management.shared.code.DVLA;

    /// <summary>
    /// An interface to define the operations allowed when validating a vehicle.
    /// </summary>
    public class VehicleValidatorCheck : IVehicleValidator
    {
        private readonly ICurrentUser _currentUser;
        private readonly cAccountProperties _accountProperties;
        private readonly IDutyOfCareApi _dutyOfCareApi;

        /// <summary>
        /// A new instance of <see cref="VehicleValidatorCheck"/>
        /// </summary>
        public VehicleValidatorCheck(ICurrentUser currentUser, cAccountProperties accountProperties)
        {
            this._currentUser = currentUser;
            this._accountProperties = accountProperties;
            this._dutyOfCareApi = BootstrapDvla.CreateNew();
        }

        /// <summary>
        /// Validate a car against an external service.
        /// </summary>
        /// <param name="vehicle">An instance of <see cref="cCar"/>to validate</param>
        /// <returns>An updated instance of <see cref="cCar"/></returns>
        public cCar ValidateCar(cCar vehicle)
        {
            var lookupResult = this._dutyOfCareApi.Lookup(vehicle.registration, BootstrapDvla.CreateLogger(this._currentUser));
            if (lookupResult.Code == "200")
            {
                if (lookupResult.Code != "200")
                {
                    return null;
                }

                vehicle.TaxExpiry = lookupResult.Vehicle.TaxExpiry;
                vehicle.IsTaxValid = lookupResult.Vehicle.TaxStatus == "Taxed";
                vehicle.MotExpiry = lookupResult.Vehicle.MotExpiry;
                vehicle.IsMotValid = lookupResult.Vehicle.MotStatus == "MOT";

                cEmployeeCars employeeCar = new cEmployeeCars(vehicle.accountid, vehicle.employeeid);
                employeeCar.SaveCar(vehicle);

                if (this._accountProperties.VehicleLookup)
                {
                    if (this._accountProperties.BlockTaxExpiry && vehicle.IsTaxValid)
                    {
                        var taxRepo = new TaxDocumentRepository(this._currentUser,
                            new cCustomEntities(this._currentUser),
                            new cFields(this._currentUser.AccountID),
                            new cTables(this._currentUser.AccountID));
                        taxRepo.Add(vehicle.TaxExpiry.Value, vehicle.carid);
                    }

                    if (this._accountProperties.BlockMOTExpiry && vehicle.IsMotValid)
                    {
                        var taxRepo = new MotDocumentRepository(this._currentUser,
                            new cCustomEntities(this._currentUser),
                            new cFields(this._currentUser.AccountID),
                            new cTables(this._currentUser.AccountID));
                        taxRepo.Add(vehicle.MotExpiry.Value, vehicle.carid);
                    }
                }
            }

            return vehicle;
        }
    }
}