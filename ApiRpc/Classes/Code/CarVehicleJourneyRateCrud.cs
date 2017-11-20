namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The car vehicle journey rate crud.
    /// </summary>
    public class CarVehicleJourneyRateCrud : EntityBase, IDataAccess<CarVehicleJourneyRate>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CarVehicleJourneyRateCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API collection.
        /// </param>
        public CarVehicleJourneyRateCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.CarAssignmentNumberApi == null)
            {
                this.DataSources.CarAssignmentNumberApi = new CarAssignmentNumberApi(metabase, accountid);
            }

            if (this.DataSources.CarVehicleJourneyRateApi == null)
            {
                this.DataSources.CarVehicleJourneyRateApi = new CarVehicleJourneyRateApi(metabase, accountid);
            }

            if (this.DataSources.CarApi == null)
            {
                this.DataSources.CarApi = new CarApi(metabase, accountid);
            }

            if (this.DataSources.EsrVehicleApi == null)
            {
                this.DataSources.EsrVehicleApi = new VehicleApi(metabase, accountid);
            }
        }

        #region IDataAccess

        public List<CarVehicleJourneyRate> Create(List<CarVehicleJourneyRate> entities)
        {
            throw new System.NotImplementedException();
        }

        public CarVehicleJourneyRate Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public CarVehicleJourneyRate Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<CarVehicleJourneyRate> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<CarVehicleJourneyRate> ReadByEsrId(long personId)
        {
            throw new System.NotImplementedException();
        }

        public List<CarVehicleJourneyRate> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<CarVehicleJourneyRate> Update(List<CarVehicleJourneyRate> entities)
        {
            throw new System.NotImplementedException();
        }

        public CarVehicleJourneyRate Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public CarVehicleJourneyRate Delete(CarVehicleJourneyRate entity)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the mileage rates associated to the cars of the imported persons. 
        /// </summary>
        /// <param name="esrPersons">
        /// <param name="trustVPD">The current VPD that is being imported against.
        /// </param>
        /// The ESR Persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> UpdateMileageRates(List<EsrPersonRecord> esrPersons, string trustVPD)
        {
            var result = new List<EsrPersonRecord>();
            var journeyRates = MemoryCache.Default.Get(string.Format("VJR_{0}", this.AccountId)) as List<VehicleJourneyRate>;
            if (journeyRates == null)
            {
                var vjr = new VehicleJourneyRateCrud(string.Empty, this.MetaBase, this.AccountId, new ApiCrudList());
                journeyRates = vjr.ReadAll();
            }

            if (journeyRates != null)
            {
                foreach (EsrPersonRecord person in esrPersons)
                {
                    var cars = this.DataSources.CarApi.ReadByEsrId(person.EmployeeId);
                    var vehicles = this.DataSources.EsrVehicleApi.ReadByEsrId(person.ESRPersonId);
                    foreach (Car car in cars)
                    {
                        var carVehicleRates = this.DataSources.CarVehicleJourneyRateApi.ReadSpecial(car.carid.ToString());
                        var allocations = this.DataSources.CarAssignmentNumberApi.ReadByEsrId(car.carid);
                        List<EsrVehicle> carVehicles = this.GetVehiclesForCurrentCar(car, vehicles, allocations);
                        if (carVehicles.Count > 0 && this.ImportElementType(TemplateMapping.ImportElementType.Vehicle, trustVPD))
                        {
                            var rates = new List<CarVehicleJourneyRate>();
                            var message = this.FindNewVehicleJourneyRates(person.EmployeeId, car.carid, carVehicles, journeyRates, ref rates);

                            if (!string.IsNullOrEmpty(message))
                            {
                                person.ActionResult.Message = person.ActionResult.Message + message;
                                person.ActionResult.Result = ApiActionResult.PartialSuccess;
                            }

                            var changesFound = this.FindChangesInCarRates(carVehicleRates, rates);

                            if (changesFound)
                            {
                                foreach (CarVehicleJourneyRate carVehicleRate in carVehicleRates)
                                {
                                    carVehicleRate.Action = Action.Delete;
                                    this.DataSources.CarVehicleJourneyRateApi.Delete(carVehicleRate);
                                }

                                this.DataSources.CarVehicleJourneyRateApi.Create(rates);
                            }
                        }
                    }
                }
            }

            return esrPersons;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get list of vehicles for current car based on car allocation numbers.
        /// </summary>
        /// <param name="currentCar">
        /// The current car.
        /// </param>
        /// <param name="vehicles">
        /// The vehicles.
        /// </param>
        /// <param name="allocations">
        /// The allocations.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<EsrVehicle> GetVehiclesForCurrentCar(Car currentCar, List<EsrVehicle> vehicles, IEnumerable<CarAssignmentNumberAllocation> allocations)
        {
            var result = new List<EsrVehicle>();
            foreach (CarAssignmentNumberAllocation allocation in allocations.Where(allocation => allocation.CarId == currentCar.carid))
            {
                result.AddRange(vehicles.Where(vehicle => vehicle.ESRVehicleAllocationId == allocation.ESRVehicleAllocationId));
            }

            return result;
        }

        /// <summary>
        /// Find changes in car rates.
        /// </summary>
        /// <param name="carVehicleRates">
        /// The car vehicle rates.
        /// </param>
        /// <param name="rates">
        /// The rates.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FindChangesInCarRates(List<CarVehicleJourneyRate> carVehicleRates, List<CarVehicleJourneyRate> rates)
        {
            if (carVehicleRates.Count == 0 || carVehicleRates.Count != rates.Count)
            {
                return true;
            }

            return carVehicleRates.Select(carVehicleJourneyRate => rates.Any(rate => rate.MileageId == carVehicleJourneyRate.MileageId)).Any(found => !found);
        }

        /// <summary>
        /// Create list of new vehicle journey rates.
        /// </summary>
        /// <param name="employeeId">
        /// Employee id
        /// </param>
        /// <param name="carId">
        /// The car Id.
        /// </param>
        /// <param name="carVehicles">
        /// The car vehicles.
        /// </param>
        /// <param name="journeyRates">
        /// The journey rates.
        /// </param>
        /// <param name="rates">
        /// The rates.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FindNewVehicleJourneyRates(int employeeId, int carId, IEnumerable<EsrVehicle> carVehicles, List<VehicleJourneyRate> journeyRates, ref List<CarVehicleJourneyRate> rates)
        {
            var result = string.Empty;
            foreach (EsrVehicle carVehicle in carVehicles)
            {
                var added = false;
                var rateFound = false;
                foreach (VehicleJourneyRate vehicleJourneyRate in journeyRates)
                {
                    if (carVehicle.UserRatesTable != null && (vehicleJourneyRate.UserRatesTable != null && vehicleJourneyRate.UserRatesTable.ToUpper() == carVehicle.UserRatesTable.ToUpper()))
                    {
                        rateFound = true;
                        if (carVehicle.EngineCC >= vehicleJourneyRate.UserRatesFromEngineSize && carVehicle.EngineCC <= vehicleJourneyRate.UserRatesToEngineSize)
                        {
                            rates.Add(new CarVehicleJourneyRate { MileageId = vehicleJourneyRate.MileageId, CarId = carId, Action = Action.Create });
                            added = true;
                        }
                    }
                }

                if (!added)
                {
                    if (rateFound)
                    {
                        result += string.Format(
                        "<br/>Found Mileage Rate '{0}' for Vehicle Id '{1}' but no match for an engine capacity of '{2}'.  This car is allocated to employee ID '{3}'", carVehicle.UserRatesTable, carVehicle.ESRVehicleAllocationId, carVehicle.EngineCC, employeeId);
                    }
                    else
                    {
                        result += string.Format(
                            "<br/>Could not find Mileage Rate '{0}' for Vehicle Id '{1}' with an engine capacity of '{2}' for employee ID '{3}'", carVehicle.UserRatesTable, carVehicle.ESRVehicleAllocationId, carVehicle.EngineCC, employeeId);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}