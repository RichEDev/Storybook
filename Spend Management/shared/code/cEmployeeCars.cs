using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    using System.Globalization;

    using SpendManagementLibrary.Helpers;
    using System.Text;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.UserDefinedFields;

    using Spend_Management.expenses.code;

    /// <summary>
    /// Car class for dealing with cars relating to an employee, caches per employee
    /// </summary>
    public class cEmployeeCars : cCarsBase
    {
        Utilities.DistributedCaching.Cache cache = new Utilities.DistributedCaching.Cache();

        #region properties
        /// <summary>
        /// The list of cars for the employee, both pool cars and normal
        /// </summary>
        public List<cCar> Cars
        {
            get { return lstCars; }
        }

        /// <summary>
        /// The list of cars declared SORN
        /// </summary>
        public List<ListItem> SornCars
        {
            get
            {
                if (this.sornCars == null)
                {
                    this.sornCars = base.GetSORNDeclaredCars(this.nAccountID, this.nEmployeeID, this.dutyOfCareCheckDate == DateTime.MinValue ? DateTime.Now : this.dutyOfCareCheckDate);
                }

                return this.sornCars;
            }
        }

        /// <summary>
        /// The private backing collection for <see cref="SornCars"/>.
        /// </summary>
        private List<ListItem> sornCars;

        private cAccountSubAccount subAccount;
        private cGlobalProperties clsproperties;
        private cAccountProperties accountProperties;

        /// <summary>
        /// The date used for duty of care checks
        /// </summary>
        private DateTime dutyOfCareCheckDate;

        #endregion properties

        /// <summary>
        /// Constructor for the collection of cars relating to an Employee
        /// </summary>
        /// <param name="accountID">Account ID</param>
        /// <param name="employeeID">Employee ID to get cars for, can be 0 to use only utility methods</param>
        /// <param name="dutyOfCareCheckDate">The date used for duty of care checks</param>
        public cEmployeeCars(int accountID, int employeeID, DateTime dutyOfCareCheckDate = new DateTime())
        {
            lstCars = null;
            nEmployeeID = employeeID;
            nAccountID = accountID;
            sCacheKeyPrefix = "";
            sConnectionString = cAccounts.getConnectionString(accountID);
            clsTables = new cTables(accountID);
            clsFields = new cFields(accountID);
            clsUserDefinedFields = new cUserdefinedFields(accountID);          
            
            var subAccounts = new cAccountSubAccounts(accountID);
            this.subAccount =  subAccounts.getFirstSubAccount();
            this.accountProperties = subAccount.SubAccountProperties.Clone();

            this.dutyOfCareCheckDate = dutyOfCareCheckDate;

            InitialiseData();

        }

        private void InitialiseData()
        {
            this.lstCars = cache.Get(this.nAccountID, CacheKey, this.nEmployeeID.ToString()) as List<cCar>;
            if (lstCars == null)
            {
                lstCars = CacheList();
            }
        }

        private List<cCar> CacheList()
        {
            List<cCar> cars = this.GetEmployeeCarsCollection(this.nEmployeeID);

            cache.Add(this.nAccountID, CacheKey, this.nEmployeeID.ToString(), cars);

            return cars;
        }

        private void ResetCache()
        {
            cache.Delete(this.nAccountID, CacheKey, this.nEmployeeID.ToString());
            lstCars = null;
            InitialiseData();
        }

        public const  string CacheKey = "employeeCars";

        private bool CarValid(cCar car, DateTime expenseDate)
        {
            DateTime defaultDate = new DateTime(1900, 01, 01);
            if ((car.startdate == null && car.enddate == null) || (car.startdate == defaultDate && car.enddate == defaultDate))
            {
                return true;
            }

            if (car.startdate <= expenseDate && (car.enddate == null || car.enddate == defaultDate))
            {
                return true;
            }

            if ((car.startdate == null || car.startdate == defaultDate) && car.enddate >= expenseDate)
            {
                return true;
            }

            if (car.startdate <= expenseDate && car.enddate >= expenseDate)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves a car
        /// </summary>
        /// <param name="oCar">A car object</param>
        /// <param name="clearCache">If true, clear the chached list.</param>
        /// <returns>The new ID for the car</returns>
        public override int SaveCar(cCar oCar, bool clearCache = true)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int nReturnValue = base.SaveCarToDB(oCar, currentUser);

            if (clearCache)
            {
                ResetCache();
            }
            return nReturnValue;
        }

        /// <summary>
        /// Deletes a car
        /// </summary>
        /// <param name="carId">Car ID</param>
        public override CarReturnVal DeleteCar(int carId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            CarReturnVal returnValue = base.DeleteCarFromDB(carId, currentUser);
            ResetCache();
            return returnValue;
        }
    
        /// <summary>
        /// Create a list of listitems that are valid cars for the user, sorted by the created date newest to oldest
        /// Used to be CreateCarDropDown
        /// </summary>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <param name="noItemsMessage">
        /// The no Items Message.
        /// </param>
        /// <returns>
        /// cars
        /// </returns>
        public List<ListItem> CreateCurrentValidCarDropDown( DateTime expenseDate, string noItemsMessage = @"Mileage cannot be claimed as you do not have an active vehicle for the date entered.", bool isEnableDoc = true, int claimId = 0, bool fromAeExpenses = false)
        {            
            var cars = new List<ListItem>();

            bool submittedClaim = false;

            if (claimId>0)
            {
                cClaims clsclaims = new cClaims(nAccountID);
                var reqclaim = clsclaims.getClaimById(claimId);
                submittedClaim = reqclaim.submitted;
            }
           
            foreach (cCar car in this.Cars.OrderBy(x => x.createdon).Reverse())
            {
                string description = this.GenerateVehicleDescription(
                    car,
                    fromAeExpenses,
                    submittedClaim,
                    this.accountProperties.DisableCarOutsideOfStartEndDate,
                    expenseDate,
                    isEnableDoc,
                    this.accountProperties.BlockTaxExpiry);

                if (description != string.Empty)
                {
                    var listItem = new ListItem(description, car.carid.ToString());
                    listItem.Attributes.Add("data-defaultuom", car.defaultuom.ToString().ToLower());
                    cars.Add(listItem);
                }              
            }
            

            if (cars.Count == 0)
            {
                cars.Add(new ListItem(noItemsMessage, "0"));
            }

            return cars;
        }

        /// <summary>
        /// Passes the vehicle details into a vehicle checker and generates the Vehicle description accordingly
        /// </summary>
        /// <param name="car">The <see cref="cCar"/></param>
        /// <param name="fromAeExpenses">Is the request part of add/edit expense</param>
        /// <param name="submittedClaim">Is the claim submitted</param>
        /// <param name="disableCarOutsideOfStartEndDate">Whether to exclude cars outside of expense start/end date</param>
        /// <param name="expenseDate">The date of the expense</param>
        /// <param name="isEnableDoc">Whether duty of care checks apply</param>
        /// <returns>The vehcile description</returns>
        public string GenerateVehicleDescription(cCar car, bool fromAeExpenses, bool submittedClaim, bool disableCarOutsideOfStartEndDate, DateTime expenseDate, bool isEnableDoc, bool blockTax)
        {
            string desc = string.Empty;
            bool carIsSorn = false;

            if (fromAeExpenses && this.accountProperties.BlockTaxExpiry)
            {
                this.dutyOfCareCheckDate = this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? expenseDate : DateTime.UtcNow.Date;
                carIsSorn = this.SornCars.Any(item => item.Text == car.carid.ToString());
            }

            bool valid = this.CheckVehicleAgainstCriteria(car, disableCarOutsideOfStartEndDate, expenseDate, carIsSorn);

            if (valid)
            {
                desc = car.make + " " + car.model;
                if (car.registration != string.Empty)
                {
                    desc += " (" + car.registration + ")";
                }
            }

            return desc;

        }

        /// <summary>
        /// Gets the active vehicle count for the supplied criteria.
        /// </summary>
        /// <param name="vehicles">
        /// The list of vehicles.
        /// </param>
        /// <param name="disableCarOutsideOfStartEndDate">
        /// Whether the vehicle is classed as disabled, if outside of the the start end date.
        /// </param>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <returns>
        /// The count of active vehicles.
        /// </returns>
        public int GetEmployeeActiveVehicleCount(List<cCar> vehicles, bool disableCarOutsideOfStartEndDate, DateTime expenseDate)
        {
            int count = 0;
            this.dutyOfCareCheckDate = this.accountProperties.UseDateOfExpenseForDutyOfCareChecks? expenseDate: DateTime.UtcNow.Date;

            foreach (var vehicle in vehicles)
            {
                var carIsSorn = this.SornCars.Any(item => item.Text == vehicle.carid.ToString());
                bool valid = this.CheckVehicleAgainstCriteria(vehicle, disableCarOutsideOfStartEndDate, expenseDate, carIsSorn);

                if (valid)
                {
                    count = count + 1;
                }
            }

            return count;
        }
 
        /// <summary>
        /// Create a list of valid mileage categories for the user
        /// Used to be CreateMileageCatDropdown
        /// </summary>
        /// <param name="carid">car id to return mileage cats for</param>
        /// <returns></returns>
        public List<ListItem> CreateValidMileageCatDropdown(int carid)
        {
            var cats = new List<ListItem>();
            cCar car = base.GetCarFromDB(carid);

            var clsmileagecats = new cMileagecats(nAccountID);

            foreach (int i in car.mileagecats)
            {
                var mileagecat = clsmileagecats.GetMileageCatById(i);
                if (mileagecat != null && mileagecat.catvalid && car.defaultuom == mileagecat.mileUom)
                {
                    cats.Add(cMileagecats.GetMileageCatListItem(mileagecat));
                }
            }
            return cats;
        }

        /// <summary>
        /// Get the car that is within a valid date range of the expense item 
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="expenseDate">This is the expense item date</param>
        /// <returns></returns>
        public cCar GetValidCarWithinExpenseDate(int carID, DateTime expenseDate)
        {
            if (this.lstCars != null && lstCars.Count > 0)
            {
                foreach (cCar car in this.lstCars)
                {
                    if (car.carid == carID)
                    {
                        if (this.CarValid(car, expenseDate))
                        {
                            return car;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the count of active cars with mileage categories set where Vehicle engine type is not 0
        /// </summary>
        /// <param name="date">Expense item date</param>
        /// <param name="includePoolCars">Include pool cars in the list?</param>
        /// <returns></returns>

        public List<cCar> GetActiveCars(DateTime? date = null, bool includePoolCars = true)
        {
            List<cCar> cars = new List<cCar>();

            int count = 0;

            var actualDate = this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date ?? DateTime.UtcNow.Date : DateTime.UtcNow.Date;
            this.dutyOfCareCheckDate = actualDate;
            foreach (cCar car in this.lstCars)
            {
                bool carIsSorn = false;

                if (includePoolCars == false && car.employeeid == 0)
                {
                    continue;
                }

                if (this.accountProperties.BlockTaxExpiry)
                {
                    carIsSorn = this.SornCars.Any(item => item.Text == car.carid.ToString());
                }

                if (((this.accountProperties.DisableCarOutsideOfStartEndDate == false && car.active == true && !carIsSorn) || (this.accountProperties.DisableCarOutsideOfStartEndDate && this.CarValid(car, actualDate) && !carIsSorn)) && car.mileagecats.Count != 0)
                {
                    cars.Add(car);
                }
            }

            return cars;
        }

        /// <summary>
        /// Returns an array of cars that have either a fuelcard or no fuelcard depending on the parameter
        /// </summary>
        /// <param name="hasFuelcard">Cars with a fuelcard against them or cars with no fuelcard?</param>
        /// <returns>Return cars with a fuelcard against them or cars with no fuelcard on them</returns>
        public cCar[] GetCarArray(bool hasFuelcard)
        {
            var cars = new List<cCar>();

            if (this.lstCars != null && this.lstCars.Count > 0)
            {
                foreach (cCar car in this.lstCars)
                {
                    if (car.fuelcard == hasFuelcard)
                    {
                        cars.Add(car);
                    }
                }
            }

            return cars.ToArray();
        }

        /// <summary>
        /// Creates the javascript odometer array for fuelcard cars
        /// </summary> 
        /// <returns>Javascript string</returns>
        public string CreateClientOdometerArray()
        {
            var output = new System.Text.StringBuilder();
            int i;
            var cars = GetActiveCars().Where(e => e.fuelcard == true).ToList();

            output.Append("var cars = new Array();\n");
            for (i = 0; i < cars.Count; i++)
            {
                var reading = cars[i].getLastOdometerReading();
                int oldreading = reading == null ? 0 : reading.newreading;
                output.Append("cars[" + i + "] = new Array(" + cars[i].carid + ",");
                output.Append(oldreading + ");\n");
            }
            
            return output.ToString();
        }


        /// <summary>
        /// Get the first active validated car that passes duty of care checks
        /// </summary>
        /// <param name="checkTaxExpiry">Account property</param>
        /// <param name="checkMOTExpiry"></param>
        /// <param name="checkInsuranceExpiry"></param>
        /// <param name="checkBreakdownCoverExpiry"></param>
        /// <param name="DisableCarOutsideOfStartEndDate"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetDefaultCarID(bool checkTaxExpiry, bool checkMOTExpiry, bool checkInsuranceExpiry, bool checkBreakdownCoverExpiry, bool DisableCarOutsideOfStartEndDate, DateTime? date = null)
        {
            var actualDate = date == null ? DateTime.Now : (DateTime)date;
            var dutyOfCareCheck = checkTaxExpiry || checkMOTExpiry || checkInsuranceExpiry || checkBreakdownCoverExpiry;
            
            foreach (cCar car in this.lstCars)
            {
                var carCheck = ((!DisableCarOutsideOfStartEndDate && car.active)
                                || this.CarValid(car, actualDate))
                               && car.mileagecats.Count != 0 && car.VehicleEngineTypeId != 0;

                var carIsValid = DisableCarOutsideOfStartEndDate ? this.CarValid(car, actualDate) : car.active;

                if (dutyOfCareCheck)
                {
                    if (carCheck)
                    {
                        if (carIsValid)
                        {
                            return car.carid;
                        }
                    }
                }
                else
                {
                    if (carCheck && carIsValid)
                    {
                        return car.carid;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Get the most recently created active validated car that passes duty of care checks
        /// </summary>
        /// <param name="checkTaxExpiry">Account property</param>
        /// <param name="checkMotExpiry">Account property</param>
        /// <param name="checkInsuranceExpiry">Account property</param>
        /// <param name="checkBreakdownCoverExpiry"></param>
        /// <param name="disableCarOutsideOfStartEndDate">Account property</param>
        /// <param name="date">The date to check for</param>
        /// <returns>A car object or null if none found</returns>
        public cCar GetDefaultCar(bool checkTaxExpiry, bool checkMotExpiry, bool checkInsuranceExpiry, bool checkBreakdownCoverExpiry, bool disableCarOutsideOfStartEndDate, List<DocumentExpiryResult> documentExpiryResult, DateTime? date = null)
        {
            var actualDate = date == null ? DateTime.Now : (DateTime)date;
            var dutyOfCareCheck = checkTaxExpiry || checkMotExpiry || checkInsuranceExpiry || checkBreakdownCoverExpiry;

            foreach (cCar car in this.lstCars.OrderByDescending(x=>x.createdon))
            {
                var carCheck = ((!disableCarOutsideOfStartEndDate && car.active)
                                || this.CarValid(car, actualDate))
                               && car.mileagecats.Count != 0 && car.VehicleEngineTypeId != 0;
                var carIsValid = disableCarOutsideOfStartEndDate ? this.CarValid(car, actualDate) : car.active;


                if (carCheck && carIsValid)
                {
                    if (dutyOfCareCheck)
                    {
                        if (documentExpiryResult.Count<=0)
                        {
                            return car;
                        }
                    }
                    else
                    {
                        return car;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Create the HTML for an odometer readings table
        /// </summary>
        /// <returns>HTML string table</returns>
        public string CreateFuelCardOdometerTable()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            output.Append("<table align=center width=99% class=borderedtable>");
            output.Append("<tr><th>Vehicle</th><th>Current Odometer Reading</th><th>New Odometer Reading</th></tr>");
            foreach (cCar car in lstCars)
            {
                if (car.fuelcard == true)
                {
                    output.Append("<tr>");
                    output.Append("<td>" + car.make + "&nbsp;" + car.model + "&nbsp;(" + car.registration + ")</td>");
                    output.Append("<td align=center>" + car.odometer + "</td>");
                    output.Append("<td align=center><input type=text name=\"newodo" + car.carid + "\" size=5 value=\"" + car.odometer + "\"></td>");
                    output.Append("</tr>");
                }
            }
            output.Append("</table>");

            return output.ToString();
        }
        /// <summary>
        /// Generate grid for claimant to enter latest odometer readings
        /// </summary>
        /// <returns></returns>
        public string GenerateOdometerGrid()
        {
            StringBuilder output = new StringBuilder();
            cOdometerReading lastodo;
            DateTime startdate = DateTime.Today;
            string rowclass = "row1";

            output.Append("<table>");
            output.Append("<tr><th>Car</th><th>Last Reading Date</th><th>Last Reading</th><th>New Reading</th></tr>");
            foreach (cCar car in Cars)
            {
                if (car.fuelcard)
                {
                    lastodo = car.getLastOdometerReading();
                    if (lastodo == null)
                    {
                        lastodo = new cOdometerReading(0, 0, DateTime.Today, 0, 0, new DateTime(1900, 01, 01), 0);
                    }
                    output.Append("<tr>");
                    output.Append("<td>" + car.make + " " + car.model + " (" + car.registration + ")</td>");
                    output.Append("<td>" + lastodo.datestamp.ToShortDateString() + "</td>");
                    output.Append("<td>" + lastodo.newreading + "</td>");
                    output.Append("<td style=\"width:50px;\"><input type=text id=\"newodo" + car.carid + "\" name=\"newodo" + car.carid + "\" style=\"width:50px;\"></td>");
                    output.Append("</tr>");
                    if (lastodo.datestamp < startdate)
                    {
                        startdate = lastodo.datestamp;
                    }
                }
            }
            output.Append("</table>");
            return output.ToString();
        }

        /// <summary>
        /// Gets the UDF details for associated with Vehicles
        /// </summary>
        /// <returns>A list of <see cref="UserDefinedFieldValue">UserDefinedFieldValue</see></returns>
        public List<UserDefinedFieldValue> GetVehicleDefinitionUDFs(int accountId)
        {
            SortedList<int, cUserDefinedField> userDefinedList = this.GetUserDefinedFieldsForVehicles(accountId);
            var userDefinedFieldValues = new List<UserDefinedFieldValue>();

            foreach (KeyValuePair<int, cUserDefinedField> keyValuePair in userDefinedList)
            {
                userDefinedFieldValues.Add(new UserDefinedFieldValue(keyValuePair.Key, keyValuePair.Value));
            }

            return userDefinedFieldValues;
        }

        /// <summary>
        /// Notifies the account admin of new vehicle that has been added and needs activating
        /// </summary>
        /// <param name="employeeId">
        /// The employeeId.
        /// </param>
        /// <param name="user">
        /// An instance of <see cref="ICurrentUserBase"/>
        /// </param>
        /// <param name="accountProperties">
        /// An instance of <see cref="cAccountProperties"/>
        /// </param>
        /// <param name="vehicleId">
        /// The vehicle Id of the vehicle to notify the admin of
        /// </param>
        public override void NotifyAdminOfNewVehicle(
              int employeeId,
              ICurrentUserBase user,
              cAccountProperties accountProperties,
              int vehicleId)
        {

            var emailTemplate = new cEmailTemplates((ICurrentUser)user);
            var employees = new cEmployees(user.AccountID);
            Employee reqEmployee = employees.GetEmployeeById(employeeId);
            var emailMessage = emailTemplate.DetermineDefaultSender(accountProperties, reqEmployee.EmailAddress);

            try
            {
                Guid templateName = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenaVehicleHasBeenAdded);

                var senderId = user.EmployeeID;
                int[] recipientIds = { accountProperties.MainAdministrator };
                emailTemplate.SendMessage(templateName, senderId, recipientIds, vehicleId, defaultSender: emailMessage);
            }
            catch (Exception)
            {
                cEventlog.LogEntry(
                    "Failed to send vehicle activation email\nAccountID: " + user.AccountID + "\nCarID: " + vehicleId);
            }
        }

        /// <summary>
        /// Gets the user defined fields for vehicles
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <returns>A sorted list of udf ids, and their <see cref="cUserDefinedField">cUserDefinedField</see></returns>
        private SortedList<int, cUserDefinedField> GetUserDefinedFieldsForVehicles(int accountId)
        {
            var userDefinedFields = new cUserdefinedFields(accountId);
            var tables = new cTables(accountId);
            cTable table = tables.GetTableByID(new Guid(ReportTable.VehicleUdfs));

            SortedList<int, cUserDefinedField> claimDefinitionUDF = new SortedList<int, cUserDefinedField>();

            foreach (var field in userDefinedFields.UserdefinedFields.Values)
            {

                if (field.table.TableID == table.TableID)
                {
                    claimDefinitionUDF.Add(field.order, field);
                }
            }

            return claimDefinitionUDF;
        }

        private bool CheckVehicleAgainstCriteria(
        cCar car,
        bool disableCarOutsideOfStartEndDate,
        DateTime expenseDate,
         bool carIsSorn)
        {
            bool outcome = ((!disableCarOutsideOfStartEndDate && car.active && !carIsSorn)
                            || ((disableCarOutsideOfStartEndDate && car.CarActiveOnDate(expenseDate)) 
                                && !carIsSorn)) && car.mileagecats.Count > 0;

            return outcome;
        }

    }
}