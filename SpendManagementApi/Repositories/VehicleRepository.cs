namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Models.Common;
    using Models.Types;

    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Models;

    using Utilities;
    using SpendManagementLibrary;
    using SpendManagementLibrary.FinancialYears;

    using Spend_Management;

    using FinancialYear = SpendManagementApi.Models.Types.FinancialYear;
    using VehicleEngineType = SpendManagementLibrary.VehicleEngineType;
    using common = SpendManagementApi.Common;
    
    /// <summary>
    /// CarRepository manages data access for Cars.
    /// </summary>
    internal class VehicleRepository : BaseRepository<Vehicle>
    {
        private cMileagecats _mData;
        private cEmployees _eData;
        private cPoolCars _pData;
        
        private cEmployeeCars _employeeCarsData;

        /// <summary>
        /// Creates a new VehicleRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public VehicleRepository(ICurrentUser user) : base(user, x => x.Id, null)
        {
            _mData = new cMileagecats(User.AccountID);
            _eData = new cEmployees(User.AccountID);
            _pData = new cPoolCars(User.AccountID);
            this._employeeCarsData = new cEmployeeCars(User.AccountID,User.EmployeeID);
        }

        /// <summary>
        /// Gets all the Vehicles within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<Vehicle> GetAll()
        {
            return _pData.Cars.Select(c => c.Cast<Vehicle>(_eData, _mData, _pData)).ToList();
        }
     
        /// <summary>
        /// Gets a single Vehicle by it's id.
        /// </summary>
        /// <param name="id">The id of the Vehicle to get.</param>
        /// <returns>The Vehicle.</returns>
        public override Vehicle Get(int id)
        {
            var item = _pData.GetCarFromDB(id, true); 
            var vehicle = item?.Cast<Vehicle>(_eData, _mData, _pData);

            if (vehicle != null)
            {
                vehicle.UnapprovedVehicleCount = this._employeeCarsData.GetUnapprovedCars().Count;
                return vehicle;
            }

            return null;
        }

        /// <summary>
        /// Gets the various data to help build the vehicles form
        /// </summary>
        /// <returns>
        /// The <see cref="VehicleDefinition">VehicleDefinition</see>
        /// </returns>
        public VehicleDefinition GetVehicleDefinition()
        {  
            ReadOnlyCollection<VehicleEngineType> engineTypes = VehicleEngineType.GetAll(User);
            var engines = new List<Models.Types.VehicleEngineType>();

            foreach (var engine in engineTypes)
            {
                engines.Add(new Models.Types.VehicleEngineType().From(engine, ActionContext));
            }
               
            var mileageRepository = new MileageCategoryRepository(User, ActionContext);
            IList<MileageCategory> mileageCategories = mileageRepository.GetAll();
            IList<MileageCategory> validMileageCategories = new List<MileageCategory>();

            var financialYearId = SpendManagementLibrary.FinancialYears.FinancialYear.GetPrimary(this.User).FinancialYearID;

            foreach (MileageCategory category in mileageCategories)
            {
                if (category.CatValid && category.FinancialYearId == financialYearId)
                {
                    validMileageCategories.Add(category);
                }
            }

            var vehicleUDFs = this._employeeCarsData.GetVehicleDefinitionUDFs(User.AccountID);
            var userDefinedFields = new List<UserDefinedFieldType>();

            if (vehicleUDFs != null)
            {
                foreach (SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue userDefinedField in vehicleUDFs)
                {
                    var userDefinedFiledValue = (cUserDefinedField)userDefinedField.Value;

                    if (userDefinedFiledValue.AllowEmployeeToPopulate)
                    {
                        var userDefinedFieldType = new UserDefinedFieldType().From(userDefinedFiledValue, ActionContext);
                        userDefinedFields.Add(userDefinedFieldType);
                    }
                }
            }

            List<FinancialYear> financialYears = FinancialYears.Years(this.User).Select(year => year.Cast<SpendManagementApi.Models.Types.FinancialYear>()).ToList();

            var vehicleDefinition = new VehicleDefinition
                                        {
                                            VehicleEngineTypes = engines,
                                            VehicleJourneyRates = validMileageCategories,
                                            UserDefinedFields = userDefinedFields,
                                            FinancialYears = financialYears
            };

            return vehicleDefinition;

        }

        /// <summary>
        /// Adds a Car.
        /// </summary>
        /// <param name="item">The Vehicle to add.</param>
        /// <returns></returns>
        public override Vehicle Add(Vehicle item)
        {
            item = base.Add(item);
            return Save(item);
        }

        /// <summary>
        /// Updates a Vehicle.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Vehicle.</returns>
        public override Vehicle Update(Vehicle item)
        {
            item = base.Update(item);
            return Save(item);
        }

        /// <summary>
        /// Links a Vehicle to an employee.
        /// </summary>
        /// <param name="id">The Id of the Vehicle</param>
        /// <param name="employeeId">The Id of the User</param>
        /// <returns></returns>
        public bool LinkUser(int id, int employeeId)
        {
            cCar car;
            CheckVehicleAndOptionalEmployee(id, out car, employeeId);

            if (car != null && car.employeeid != 0)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.Vehicles_CannotUnlinkAnOwnedCar);
            }

            _pData.SaveCar(car);
            _pData.AddPoolCarUser(id, employeeId);
            return true;
        }

        /// <summary>
        /// Unlinks a Vehicle from an employee.
        /// </summary>
        /// <param name="id">The Id of the Vehicle</param>
        /// <param name="employeeId">The Id of the User</param>
        /// <returns></returns>
        public bool UnLinkUser(int id, int employeeId)
        {
            cCar car;
            CheckVehicleAndOptionalEmployee(id, out car, employeeId);

            if (car != null && car.employeeid != 0)
        {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.Vehicles_CannotUnlinkAnOwnedCar);
        }

            _pData.DeleteUserFromPoolCar(employeeId, id);
            return true;
        }

        /// <summary>
        /// Unlinks all users from a vehicle.
        /// </summary>
        /// <param name="id">The Id of the Vehicle</param>
        /// <returns></returns>
        public Vehicle UnLinkAllUsers(int id)
        {
            cCar car;
            CheckVehicleAndOptionalEmployee(id, out car);

            if (car != null && car.employeeid != 0)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.Vehicles_CannotUnlinkAnOwnedCar);
            }

            _pData.DeletePoolCarUsers(id);
            return Get(id);
        }

        /// <summary>
        /// Deletes a Vehicle, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Vehicle to delete.</param>
        /// <returns>The deleted Vehicle.</returns>
        public override Vehicle Delete(int id)
        {
            base.Delete(id);

            // attempt the delete
            _pData.DeletePoolCarUsersFromCarID(id);
            var result = _pData.DeleteCar(id);
            
            if (result != CarReturnVal.Success)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return Get(id);
        }

        /// <summary>
        /// Gets the list of the user's vehicles.
        /// </summary>
        /// <returns>A list of <see cref="VehicleBasic">VehicleBasic</see></returns>
        public List<VehicleBasic> GetMyVehicles()
        {
            return this.GenerateValidListOfVehicles(DateTime.UtcNow);  
        }

        /// <summary>
        /// Gets the list of the user's <see cref="Vehicle">Vehicles for the add/edit expense screen</see>.
        /// </summary>
        /// <param name="claimId">The Id of the claim the expense is against</param>
        /// <param name="expenseDate">The expense date</param>
        /// <param name="subCatId">The subcatId of the expense</param>
        /// <returns>A list of <see cref="VehicleBasic">VehicleBasic</see></returns>
        public List<VehicleBasic> GetMyVehiclesForAddEditExpense(int claimId, DateTime expenseDate, int subCatId)
        {
            return this.GenerateValidListOfVehicles(expenseDate, claimId, subCatId);
        }

        /// <summary>
        /// Gets the list of the user's <see cref="VehicleBasic">Vehicles for the add/edit expense screen</see>.
        /// </summary>
        /// <param name="expenseId">The Id of the expense item</param>
        /// <param name="expenseDate">The expense date</param>
        /// <param name="subCatId">The subcatId of the expense</param>
        /// <param name="employeeId">The employee id</param>
        /// <returns>A list of <see cref="VehicleBasic">VehicleBasic</see></returns>
        public List<VehicleBasic> GetMyVehiclesForEmployee(int expenseId, DateTime expenseDate, int subCatId, int employeeId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);

            var expenseSubCatRepository = new ExpenseSubCategoryRepository(this.User, this.ActionContext);
            var claimRepository = new ClaimRepository(this.User, this.ActionContext);
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);

            return this.GenerateValidListOfVehiclesByEmployee(expenseDate, expenseId, subCatId, employeeId, expenseSubCatRepository,claimRepository, expenseItemRepository, subAccounts);
        }

        /// <summary>
        /// Gets the count of active vehicles for the supplied criteria
        /// </summary>
        /// <param name="disableCarOutsideOfStartEndDate">Whether the vehicle is classed as disabled, if outside of the the start end date.</param>
        /// <param name="expenseDate">The date of the expense</param>
        /// <returns></returns>
        public int EmployeeActiveVehicleCount(bool disableCarOutsideOfStartEndDate, DateTime expenseDate)
        {
            List<cCar> employeeVehicles = this._eData.GetEmployeeCars(this.User.EmployeeID, this.User.AccountID);
            int vehiclecount = this._employeeCarsData.GetEmployeeActiveVehicleCount(employeeVehicles,disableCarOutsideOfStartEndDate, expenseDate);
         
            return vehiclecount;
        }

        /// <summary>
        /// Gets a count of unapproved vehicles for the current user
        /// </summary>
        /// <returns>A count of unapproved vehicles</returns>
        public int GetUnapprovedVehiclesCount()
        {
            return _employeeCarsData.GetUnapprovedCars().Count;
        }
 
        /// <summary>
        /// Gets a count of unapproved vehicles for the current user.
        /// </summary>
        /// <returns>A count of unapproved vehicles</returns>
        public int GetUnapprovedVehiclesCountForClaimant(int employeeId, int expenseId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);

            this._employeeCarsData = new cEmployeeCars(User.AccountID, employeeId);
            return _employeeCarsData.GetUnapprovedCars().Count;
        }

        private void CheckVehicleAndOptionalEmployee(int id, out cCar car, int? employeeId = null)
        {
            car = _pData.GetCarFromDB(id, true);
            if (car == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId,
                    ApiResources.ApiErrorUpdateObjectWithWrongIdMessage);
            }
            
            if (employeeId == null) return;

            var employee = _eData.GetEmployeeById((int) employeeId);
            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }
        }

        private Vehicle Save(Vehicle item)
        {
            var subAccounts = new cAccountSubAccounts(User.AccountID);
            cAccountProperties subAccountProperties = subAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

            ValidateSaveAction(item, subAccountProperties);
  
            item.AccountId = User.AccountID;
            item.ModifiedById = User.EmployeeID;
            item.ModifiedOn = DateTime.UtcNow;

            item.Validate(ActionContext);

            int employeeId = User.EmployeeID;

            if (!item.EmployeeId.HasValue || item.IsPoolVehicle)
            {
                employeeId = 0;
            }
            else if (item.EmployeeId.Value > 0)
            {
                employeeId = item.EmployeeId.Value;
            }

            item.EmployeeId = employeeId;    
            item.IsActive = subAccountProperties.ActivateCarOnUserAdd;
            item.Approved = subAccountProperties.ActivateCarOnUserAdd;

            // save the Car
            var car = item.Cast<cCar>(_eData, _mData, _pData);

            var carId = _employeeCarsData.SaveCar(car);

            this.ProcessVehicleSave(car.employeeid, carId);

            if (!subAccountProperties.ActivateCarOnUserAdd)
            {
              this._employeeCarsData.NotifyAdminOfNewVehicle(car.employeeid, this.User, subAccountProperties, carId);
            }

            // set the previous car to inactive and update its end date to be the start date of the new car
            if (item.PreviousVehicleId > 0)
            {
                var previousVehicle = this.Get(item.PreviousVehicleId).Cast<cCar>(_eData, _mData, _pData);
                var setEndDate = subAccountProperties.AllowEmpToSpecifyCarStartDateOnAdd;
                var previousVehicleId = this._employeeCarsData.UpdateVehicleBeingReplaced(previousVehicle, car.startdate, this.User.EmployeeID, setEndDate);        
                this.ProcessVehicleSave(previousVehicle.employeeid, previousVehicleId);
            }

            // re-initialise cache with updated vehicle 
            var vehicleRepository = new VehicleRepository(this.User);
            // get the added / updated db item
            var result = vehicleRepository.Get(carId);
            result.Id = carId;
            _employeeCarsData.SaveUserDefinedFieldsValues(result.Id, item.UserDefined.ToSortedList(), User, result.Registration);

            return result;
        }

        /// <summary>
        /// Processes the outcome of saving a vehicle
        /// </summary>
        /// <param name="employeeId">
        /// The employee Id.
        /// </param>
        /// <param name="carId">
        /// The id of the vehicle returned by the save method
        /// </param>
        /// <exception cref="ApiException">
        /// </exception>
        private void ProcessVehicleSave(int employeeId , int carId)
        {
            if (employeeId > 0)
            {
                this._eData.Cache.Delete(this.User.AccountID, cEmployeeCars.CacheKey, employeeId.ToString());           
            }
            else
            {
                this._eData.Cache.Delete(this.User.AccountID, cPoolCars.CacheKey, string.Empty);
            }

   
            if (carId == -1)
            {
                throw new ApiException(
                    ApiResources.ApiErrorRecordAlreadyExists,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (carId < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }
        }

        /// <summary>
        /// Validates the Vehicle against the account property settings for Vehicles, to ensure the save action can take place.
        /// <param name="vehicle">The <see cref="Vehicle"/></param>
        /// <param name="accountProperties">The <see cref="cAccountProperties"/></param>
        /// </summary>
        private void ValidateSaveAction(Vehicle vehicle, cAccountProperties accountProperties)
        { 
            if (!accountProperties.AllowUsersToAddCars && !this.User.Employee.AdminOverride)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.APIErrorAccountDoesNotPermitEmployeesVehicles);
            }

            if (vehicle.PreviousVehicleId == 0)
            {
                //only validate if we are not replacing a vehicle, otherwise start date is always passed when replacing a vehicle, regardless of account options.
                if (!accountProperties.AllowEmpToSpecifyCarStartDateOnAdd && vehicle.CarUsageStartDate != null)
                {
                    throw new ApiException(
                       ApiResources.ApiErrorSaveUnsuccessful,
                       ApiResources.APIErrorAccountDoesNotPermitEmployeesToSpecifyStartDateForVehicle);
                }
            }

            if (accountProperties.EmpToSpecifyCarStartDateOnAddMandatory && vehicle.CarUsageStartDate == null)
            {
                throw new ApiException(
                   ApiResources.ApiErrorSaveUnsuccessful,
                   ApiResources.APIErrorAccountRequiresStartDateToBeSpecifiedForAVehicle);
            }

            if (!accountProperties.ShowMileageCatsForUsers && vehicle.MileageCategoryIds.Count > 0)
            {
                throw new ApiException(
                   ApiResources.ApiErrorSaveUnsuccessful,
                   ApiResources.APIErrorAccountDoesNotPermitEmployeesToSetMileageCategories);
            }
        }

        /// <summary>
        /// Generates a list of vehicles for an employee, for the supplied criteria
        /// </summary>
        /// <param name="date">The date the vehicles are active on</param>
        /// <param name="claimId">The Id of the claim</param>
        /// <param name="subCatId">The subcat</param>
        /// <param name="isFromAEExpense" If the request is from add edit expense</para>
        /// <returns></returns>
        private List<VehicleBasic> GenerateValidListOfVehicles(DateTime date, int claimId = 0, int subCatId = 0, bool isFromAEExpense = false)
        {
            bool enableDoc;

            if (subCatId == 0)
            {
                enableDoc = true;
            }
            else
            {
                var subcatRepo = new ExpenseSubCategoryRepository(this.User, this.ActionContext);
                ExpenseSubCategory subCat = subcatRepo.Get(subCatId);
                enableDoc = subCat.EnableDoc;
            }

            List<cCar> employeeVehicles = this._eData.GetEmployeeCars(this.User.EmployeeID, this.User.AccountID);

            var subaccs = new cAccountSubAccounts(this.User.AccountID);
            bool isSubmited = false;
            cAccountProperties properties = this.User != null
                                                   ? subaccs.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties
                                                   : subaccs.getFirstSubAccount().SubAccountProperties;

            if (claimId > 0)
            {
                ClaimRepository claimRepository = new ClaimRepository(this.User, this.ActionContext);
                var claim = claimRepository.Get(claimId);
                isSubmited = claim.Submitted;
            }

            var vehicles = new List<VehicleBasic>();

            foreach (var vehicle in employeeVehicles)
            {
                string vehicleDescription = this._employeeCarsData.GenerateVehicleDescription(
                    vehicle,
                    isFromAEExpense,
                    isSubmited,
                    properties.DisableCarOutsideOfStartEndDate,
                    date,
                    enableDoc,
                    properties.BlockTaxExpiry);

                if (vehicleDescription != string.Empty)
                {
                    vehicles.Add(
                        new VehicleBasic { Id = vehicle.carid, VehicleDescription = vehicleDescription, UnitOfMeasure = vehicle.defaultuom, Registration = vehicle.registration, VehicleTypeId = (VehicleType)vehicle.VehicleTypeID});
                }
            }

            return vehicles;
        }

        /// <summary>
        /// Generates a valid list of vehicles by for the employeeId.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="expenseId">
        /// The expense id.
        /// </param>
        /// <param name="subCatId">
        /// The subcat id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="subcatRepository">
        /// An instance of the subcat repository.
        /// </param>
        /// <param name="claimRepository">
        ///  An instance of the claim repository.
        /// </param>
        /// <param name="expenseItemRepository">
        ///  An instance of the expense item repository.
        /// </param>
        /// <param name="subAccounts">
        ///  An instance of the sub accounts.
        /// </param>
        /// <returns>
        /// The a list of <see cref="VehicleBasic">VehicleBasic</see>
        /// </returns>
        private List<VehicleBasic> GenerateValidListOfVehiclesByEmployee(DateTime date, int expenseId, int subCatId, int employeeId, ExpenseSubCategoryRepository subcatRepository, ClaimRepository claimRepository, ExpenseItemRepository expenseItemRepository, cAccountSubAccounts subAccounts)
        {         
            ExpenseSubCategory subCat = subcatRepository.Get(subCatId);
            var enableDoc = subCat.EnableDoc;
            List<cCar> employeeVehicles = this._eData.GetEmployeeCars(employeeId > 0 ? employeeId : this.User.EmployeeID, this.User.AccountID);
            bool isSubmited = false;

            cAccountProperties properties = this.User != null
                                                   ? subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties
                                                   : subAccounts.getFirstSubAccount().SubAccountProperties;
     
            var claim = claimRepository.Get(expenseItemRepository.Get(expenseId).ClaimId);
            isSubmited = claim.Submitted;

            var vehicles = new List<VehicleBasic>();

            foreach (var vehicle in employeeVehicles)
            {
                string vehicleDescription = this._employeeCarsData.GenerateVehicleDescription(
                    vehicle,
                    false,
                    isSubmited,
                    properties.DisableCarOutsideOfStartEndDate,
                    date,
                    enableDoc,
                    properties.BlockTaxExpiry);

                if (vehicleDescription != string.Empty)
                {
                    vehicles.Add(
                        new VehicleBasic { Id = vehicle.carid, VehicleDescription = vehicleDescription, UnitOfMeasure = vehicle.defaultuom, Registration = vehicle.registration, VehicleTypeId = (VehicleType)vehicle.VehicleTypeID });
                }
            }

            return vehicles;
        }
    }
}
