namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using Attributes.Validation;
    using SpendManagementApi.Common;

    using Common;
    using Interfaces;
    using Employees;
    using Utilities;
    using SpendManagementLibrary;
    using Spend_Management;
    using SpendManagementApi.Common.Enums;


    /// <summary>
    /// Represents a Vehicle that can be owned either by a single <see cref="Employee">Employee</see>, or a pool-car style vehicle, which has mulitple 'owners'.
    /// If the vehicle's EmployeId property is set, then the car is not a poolcar, but belongs to an employee.
    /// </summary>
    public class Vehicle : BaseExternalType, IRequiresValidation, IEquatable<Vehicle>, IApiFrontForDbObject<cCar, Vehicle>
    {
        /// <summary>
        /// The unique ID of this vehicle.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The make of this vehicle.
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// The model of this vehicle.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The registration number of this vehicle.
        /// </summary>
        public string Registration { get; set; }

        /// <summary>
        /// The unit of measurement for distance of this vehicle.
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public MileageUOM UnitOfMeasure { get; set; }

        /// <summary>
        /// The fuel type of this vehicle.
        /// </summary>
        [Required]
        public int FuelType { get; set; }

        /// <summary>
        /// The engine size of this vehicle.
        /// </summary>
        public int EngineSize { get; set; }

        /// <summary>
        /// Whether the vehicle is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether the vehicle is exempt from Home to Location mileage.
        /// </summary>
        public bool IsExemptFromHomeToLocationMileage { get; set; }

        /// <summary>
        /// The car usage start date.
        /// </summary>
        public DateTime? CarUsageStartDate { get; set; }

        /// <summary>
        /// The car usage end date.
        /// </summary>
        public DateTime? CarUsageEndDate { get; set; }

        /// <summary>
        /// The financial year of mileage categories associated with this vehicle.
        /// </summary>
        public int? FinancialYearId { get; set; }

        /// <summary>
        /// The associated mileage category Ids for this vehicle.
        /// </summary>
        public List<int> MileageCategoryIds { get; set; }


        /// <summary>
        /// The odometer readings for this vehicle. 
        /// Must me populated if the OdometerReadingsRequired property is set to true.
        /// </summary>
        public OdometerReadings OdometerReadings { get; set; }

        /// <summary>
        /// Whether the vehicle has been approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Contains the Id of the owner, if this car is not a Pool Car.
        /// </summary>
        public new int? EmployeeId { get; set; }

        /// <summary>
        /// Contains the Ids of the Employees allowed to use this Car (if it is a Pool Car).
        /// </summary>
        public List<int> PoolCarUsers { get; set; }
        
        /// <summary>
        /// The User defined fields.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefined { get; set; }

        /// <summary>
        /// Contains the typeid of vehicle, like bicycle, car, motorcycle, moped 
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public VehicleType VehicleTypeId { get; set; }

        /// <summary>
        /// Get or Sets the vehcile description
        /// </summary>
        public string VehicleDescription { get; set; }

        /// <summary>
        /// Gets or Sets whether the vehicle is a pool vehicle
        /// </summary>
        public bool IsPoolVehicle { get; set; }

        /// <summary>
        /// Gets or sets the Id of the vehicle if the current vehicle is replacing another
        /// </summary>
        public int PreviousVehicleId { get; set; }

        /// <summary>
        /// Gets or sets the number of unapproved vehicles for the employee
        /// </summary>
        public int UnapprovedVehicleCount { get; set; }
    
        /// <summary>
        /// Gets or sets the Tax Expiry of this vehicle
        /// </summary>
        public DateTime TaxExpiry { get; set; }

        /// <summary>
        /// Gets or sets the tax status of this vehicle
        /// </summary>
        public string TaxStatus { get;set; }

        /// <summary>
        /// Gets or sets the MOT Expiry of this vehicle
        /// </summary>
        public DateTime MotExpiry { get; set; }

        /// <summary>
        /// Gets or sets the MOT status of this vehicle
        /// </summary>
        public string MotStatus { get;set; }

        /// <summary>
        /// Validates this Vehicle.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <exception cref="InvalidDataException">Data is invalid.</exception>
        /// <exception cref="ApiException">Api related errors.</exception>
        public void Validate(IActionContext actionContext)
        {
            if (string.IsNullOrEmpty(Make))
            {
                throw new InvalidDataException(ApiResources.Vehicles_MakeOfTheCarMustBeProvided);
            }

            if (string.IsNullOrEmpty(Model))
            {
                throw new InvalidDataException(ApiResources.Vehicles_ModelOfTheCarMustBeProvided);
            }

            if (!Enum.IsDefined(typeof(MileageUOM), UnitOfMeasure))
            {
                throw new InvalidDataException(ApiResources.Vehicles_UnitOfMeasureProvidedIsNotValid);
            }
            
            if (this.FuelType <= 0)
            {
                throw new InvalidDataException(ApiResources.Vehicles_FuelTypeProvidedIsNotValid);
            }
            
            if (MileageCategoryIds != null
                && MileageCategoryIds.Any(mc => actionContext.MileageCategories.GetMileageCatById(mc) == null))
            {
                throw new InvalidDataException(ApiResources.Vehicles_MileageCategoryDoesNotExist);
            }

            if (MileageCategoryIds != null
                && MileageCategoryIds.Any(mc => !actionContext.MileageCategories.GetMileageCatById(mc).catvalid))
                {
                        throw new InvalidDataException(ApiResources.Vehicles_MileageCategoryInvalid);
                }
                
            if (MileageCategoryIds != null
                && MileageCategoryIds.Any(
                    mc => actionContext.MileageCategories.GetMileageCatById(mc).FinancialYearID != this.FinancialYearId))
                {
                throw new InvalidDataException(ApiResources.Vehicles_MileageCategoryInvalidFinancialYearId);
            }

            if (MileageCategoryIds != null
                && MileageCategoryIds.Any(
                    mc => actionContext.MileageCategories.GetMileageCatById(mc).mileUom != this.UnitOfMeasure))
                    {
                throw new InvalidDataException(ApiResources.Vehicles_MileageCategoryInvalidUOM);
                    }
            
            // check that if the pool cars are there, that each one exists.
            if (PoolCarUsers != null && PoolCarUsers.Any())
            {
                if (EmployeeId != null)
                {
                    throw new InvalidDataException(ApiResources.Vehicles_CannotBeBothPoolCarAndOwned);
            }
            }

            this.UserDefined = UdfValidator.Validate(this.UserDefined, actionContext, "userdefinedCars");
            
            Helper.ValidateIfNotNull(this.OdometerReadings, actionContext, this.AccountId);

        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(Vehicle other)
        {
            if (other == null)
            {
                return false;
            }

            return this.AccountId.Equals(other.AccountId) && this.Approved.Equals(other.Approved)
                   && this.CarUsageEndDate.Equals(other.CarUsageEndDate)
                   && this.CarUsageStartDate.Equals(other.CarUsageStartDate)
                   && this.EngineSize.Equals(other.EngineSize)
                   && this.FuelType.Equals(other.FuelType)
                   && this.IsActive.Equals(other.IsActive)
                   && this.IsExemptFromHomeToLocationMileage.Equals(other.IsExemptFromHomeToLocationMileage)
                   && this.Make.Equals(other.Make)
                   && this.MileageCategoryIds.SequenceEqual(other.MileageCategoryIds)
                   && this.Model.Equals(other.Model)
                   && this.OdometerReadings.Equals(other.OdometerReadings)
                   && this.Registration.Equals(other.Registration)
                   && this.UnitOfMeasure.Equals(other.UnitOfMeasure);
    }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Vehicle);
        }

        /// <summary>
        /// The from.
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="Vehicle"/>.
        /// </returns>
        public Vehicle From(cCar dbType, IActionContext actionContext)
        {
            var employees = new cEmployees(dbType.accountid);
            var mCats = new cMileagecats(dbType.accountid);
            var pools = new cPoolCars(dbType.accountid);
            return dbType.Cast<Vehicle>(employees, mCats, pools);
        }

        /// <summary>
        /// The to.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="cCar"/>.
        /// </returns>
        public cCar To(IActionContext actionContext)
        {
            var employees = new cEmployees(AccountId.Value);
            var mCats = new cMileagecats(AccountId.Value);
            var pools = new cPoolCars(AccountId.Value);
            return this.Cast<cCar>(employees, mCats, pools);
        }

        /// <summary>
        /// The from.
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <returns>
        /// The <see cref="Vehicle"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public Vehicle From(cCar dbType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The to.
        /// </summary>
        /// <returns>
        /// The <see cref="cCar"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public cCar To()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a container for Odometer Readings, with a few extra pieces of data.
    /// </summary>
    public class OdometerReadings : IRequiresValidation, IEquatable<OdometerReadings>
    {
        /// <summary>
        /// Whether Odometer Readings are required to be kept for this Vehicle.
        /// If this is set to true, then the OdometerReadings property must be populated when claimants claim.
        /// </summary>
        public bool OdometerReadingRequired { get; set; }

        /// <summary>
        /// Gets or sets starting odometer reading
        /// </summary>
        public long StartOdometerReading { get; set; }

        /// <summary>
        /// Gets or sets ending odometer reading
        /// </summary>
        public int EndOdometerReading { get; set; }

        /// <summary>
        /// Gets or sets the list of individual odometer readings' Ids.
        /// </summary>
        public List<int> OdometerReadingList { get; internal set; }

        /// <summary>
        /// Creates a new OdometerReadings.
        /// </summary>
        public OdometerReadings()
        {
            OdometerReadingList = new List<int>();
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public void Validate(IActionContext actionContext)
        {
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(OdometerReadings other)
        {
            if (other == null)
            {
                return false;
            }
            return this.EndOdometerReading.Equals(other.EndOdometerReading)
                   && this.OdometerReadingList.SequenceEqual(other.OdometerReadingList)
                   && this.OdometerReadingRequired.Equals(other.OdometerReadingRequired)
                   && this.StartOdometerReading.Equals(other.StartOdometerReading);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as OdometerReadings);
        }
    }

    /// <summary>
    /// Represents an individual reading of a vehicles odometer. 
    /// An Odometer is the clock of how many miles or kilometres the vehicle has travelled in its lifetime.
    /// </summary>
    public class OdometerReading : DeletableBaseExternalType, IRequiresValidation, IEquatable<OdometerReading>
    {
        /// <summary>
        /// The unique Id of this odometer reading.
        /// </summary>
        public int OdometerReadingId { get; set; }

        /// <summary>
        /// The Id of the <see cref="Vehicle">Vehicle</see> this reading is attached to.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// The date this odometer reading was taken.
        /// </summary>
        public DateTime ReadingDate { get; set; }

        /// <summary>
        /// The value of the odometer at the last reading.
        /// </summary>
        public int OldReading { get; set; }

        /// <summary>
        /// The new value of the odometer at this reading.
        /// </summary>
        public int NewReading { get; set; }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <exception cref="InvalidDataException">
        /// </exception>
        public void Validate(IActionContext actionContext)
        {
            if (actionContext.PoolCars.GetCarByID(CarId) == null)
            {
                throw new InvalidDataException(ApiResources.Vehicles_NewReadingMustBeGreaterThanOldReading);
            }

 	        if (OldReading > NewReading)
 	        {
 	            throw new InvalidDataException(ApiResources.Vehicles_NewReadingMustBeGreaterThanOldReading);
 	        }
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(OdometerReading other)
        {
            if (other == null)
            {
                return false;
            }
            return this.NewReading.Equals(other.NewReading)
                   && this.OdometerReadingId.Equals(other.OdometerReadingId) && this.OldReading.Equals(other.OldReading)
                   && ReadingDate.Subtract(other.ReadingDate).Days == 0;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as OdometerReading);
        }
    }

    internal static class OdometerReadingConversion
    {
        /// <summary>
        /// The cast.
        /// </summary>
        /// <param name="odometerReading">
        /// The odometer reading.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        internal static TResult Cast<TResult>(this cOdometerReading odometerReading) where TResult : OdometerReading, new()
        {
            return new TResult
                       {
                           CarId = odometerReading.carid,
                           OdometerReadingId = odometerReading.odometerid,
                           NewReading = odometerReading.newreading,
                           OldReading = odometerReading.oldreading,
                           ReadingDate = odometerReading.datestamp,
                           CreatedById = odometerReading.createdby,
                           CreatedOn = odometerReading.createdon
                       };
        }

        internal static cOdometerReading Cast<TResult>(this OdometerReading odometerReading) where TResult : cOdometerReading, new()
        {
            return new cOdometerReading(
                odometerReading.OdometerReadingId,
                odometerReading.CarId,
                odometerReading.ReadingDate,
                odometerReading.OldReading,
                odometerReading.NewReading,
                odometerReading.CreatedOn,
                odometerReading.CreatedById);
        }
    }

    internal static class VehicleConversion
    {
        /// <summary>
        /// The cast.
        /// </summary>
        /// <param name="car">
        /// The car.
        /// </param>
        /// <param name="employees">
        /// The employees.
        /// </param>
        /// <param name="mileagecats">
        /// The mileagecats.
        /// </param>
        /// <param name="poolCars">
        /// The pool cars.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        internal static TResult Cast<TResult>(this cCar car, cEmployees employees, cMileagecats mileagecats, cPoolCars poolCars) where TResult : Vehicle, new()
        {
            if (car == null)
            {
                return null;
            }
            
            var userDefinedFields = poolCars.GetUserDefinedFieldsList(car.carid);
            var userDefinedFilFieldValues = userDefinedFields == null ? null : poolCars.GetUserDefinedFieldsList(car.carid).ToUserDefinedFieldValueList();
            var vehicle = new TResult
                       {
                           Id = car.carid,
                           AccountId = car.accountid,
                           IsActive = car.active,
                           CarUsageEndDate = car.enddate,
                           CarUsageStartDate = car.startdate,
                           CreatedById = car.createdby,
                           CreatedOn = car.createdon ?? DateTime.UtcNow,
                           EngineSize = car.EngineSize,
                           FuelType = car.VehicleEngineTypeId,
                           IsExemptFromHomeToLocationMileage = car.ExemptFromHomeToOffice,
                           Make = car.make,
                           MileageCategoryIds = (car.mileagecats ?? new List<int>()).ToList(),
                           FinancialYearId = (car.mileagecats != null && car.mileagecats.Any()) ? mileagecats.GetMileageCatById(car.mileagecats.First()).FinancialYearID : null,
                           Model = car.model,
                           ModifiedById = car.modifiedby,
                           ModifiedOn = car.modifiedon,
                           Registration = car.registration,
                           UnitOfMeasure = car.defaultuom,
                           UserDefined = userDefinedFilFieldValues,
                           Approved = car.Approved,
                           EmployeeId = car.employeeid == 0 ? (int?)null : car.employeeid,
                PoolCarUsers = car.employeeid == 0 ? poolCars.GetUsersPerPoolCar(car.carid) : null
                       };

            // do the odometer readings
                vehicle.OdometerReadings = new OdometerReadings
                {
                    StartOdometerReading = car.odometer,
                    EndOdometerReading = car.endodometer,
                    OdometerReadingRequired = car.fuelcard
                };

            return vehicle;
        }

        internal static cCar Cast<TResult>(this Vehicle vehicle, cEmployees employees, cMileagecats mileagecats, cPoolCars poolCars) where TResult : cCar, new()
        {
            vehicle.MileageCategoryIds = Helper.NullIf(vehicle.MileageCategoryIds);
            vehicle.OdometerReadings = Helper.NullIf(vehicle.OdometerReadings);
            
            cCar car = new cCar(
                vehicle.AccountId.Value, 
                vehicle.EmployeeId.HasValue ? vehicle.EmployeeId.Value : 0, 
                vehicle.Id, 
                vehicle.Make, 
                vehicle.Model, 
                vehicle.Registration, 
                vehicle.CarUsageStartDate, 
                vehicle.CarUsageEndDate, 
                vehicle.IsActive, 
                vehicle.MileageCategoryIds.ToList(), 
                (byte)vehicle.FuelType,
                vehicle.OdometerReadings.StartOdometerReading,
                vehicle.OdometerReadings.OdometerReadingRequired,
                vehicle.OdometerReadings.EndOdometerReading,
                vehicle.UnitOfMeasure,
                vehicle.EngineSize,
                vehicle.CreatedOn,
                vehicle.CreatedById,
                vehicle.ModifiedOn,
                vehicle.ModifiedById,
                vehicle.Approved,
                vehicle.IsExemptFromHomeToLocationMileage,
                (byte)vehicle.VehicleTypeId);
            return car;
        }
    }
}