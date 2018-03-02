
namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;
    using Spend_Management.shared.code.DVLA;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Random;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.FinancialYears;
    using expenses.code;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Vehicles;
    using Spend_Management.Bootstrap;

    /// <summary>
    /// Summary description for svcCars
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [ScriptService]
    public class svcCars : WebService
    {
        /// <summary>
        /// Add vehicle type such as bicycle, car, moped etc.
        /// </summary>
        /// <param name="grid">The grid</param>
        public static void AddVehicleTypes(ref cGridNew grid)
        {
            foreach (CarTypes.VehicleType value in Enum.GetValues(typeof(CarTypes.VehicleType)))
            {
                string text = Enum.GetName(typeof(CarTypes.VehicleType), value);
                switch (value)
                {
                    case CarTypes.VehicleType.None:
                        text = String.Empty;
                        break;
                    default:
                        text = CarsBase.GetEnumDescription(value);
                        break;
                }
                ((cFieldColumn)grid.getColumnByName("vehicletypeid")).addValueListItem((int)value, text);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] getCar(int employeeid, int carid)
        {
            object[] arrCar = new object[6];
            string checkerDetails = string.Empty;
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(user.AccountID);
            Employee emp = clsemployees.GetEmployeeById(employeeid);
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(user.AccountID, employeeid);
            cCar car = clsEmployeeCars.GetCarByID(carid);
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(user.AccountID);
            cTables clstables = new cTables(user.AccountID);
            cTable tbl = clstables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));

            cCar tempCar = (cCar)car.Clone();              
            var  udfList = clsEmployeeCars.GetUserDefinedFieldsList(tempCar.carid) ?? new SortedList<int, object>();

            arrCar[0] = tempCar;
            arrCar[1] = clsuserdefined.getUserdefinedValuesForClient(udfList, tbl.GetUserdefinedTable());

            return arrCar;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] getPoolCar(int carid)
        {
            object[] arrCar = new object[6];
            string checkerDetails = string.Empty;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cEmployees clsEmployees = new cEmployees(currentUser.AccountID);
            Employee emp = null;
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);
            cCar car = clsPoolCars.GetCarByID(carid);
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(currentUser.AccountID);
            cTables clstables = new cTables(currentUser.AccountID);
            cTable tbl = clstables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
            cCar tempCar = (cCar)car.Clone();
            var udfList = clsPoolCars.GetUserDefinedFieldsList(car.carid) ?? new SortedList<int, object>();
     
            arrCar[0] = tempCar;
            arrCar[1] = clsuserdefined.getUserdefinedValuesForClient(udfList, tbl.GetUserdefinedTable());  

            return arrCar;
        }

        /// <summary>
        /// The save car.
        /// </summary>
        /// <param name="carid">
        /// The carid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="startdate">
        /// The startdate.
        /// </param>
        /// <param name="enddate">
        /// The enddate.
        /// </param>
        /// <param name="make">
        /// The make.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <param name="cartypeid">
        /// The cartypeid.
        /// </param>
        /// <param name="startodometer">
        /// The startodometer.
        /// </param>
        /// <param name="fuelcard">
        /// The fuelcard.
        /// </param>
        /// <param name="endodometer">
        /// The endodometer.
        /// </param>
        /// <param name="taxexpiry">
        /// The taxexpiry.
        /// </param>
        /// <param name="taxlastchecked">
        /// The taxlastchecked.
        /// </param>
        /// <param name="taxcheckedby">
        /// The taxcheckedby.
        /// </param>
        /// <param name="mottestnumber">
        /// The mottestnumber.
        /// </param>
        /// <param name="motlastchecked">
        /// The motlastchecked.
        /// </param>
        /// <param name="motcheckedby">
        /// The motcheckedby.
        /// </param>
        /// <param name="motexpiry">
        /// The motexpiry.
        /// </param>
        /// <param name="insurancenumber">
        /// The insurancenumber.
        /// </param>
        /// <param name="insuranceexpiry">
        /// The insuranceexpiry.
        /// </param>
        /// <param name="insurancelastchecked">
        /// The insurancelastchecked.
        /// </param>
        /// <param name="insurancecheckedby">
        /// The insurancecheckedby.
        /// </param>
        /// <param name="serviceexpiry">
        /// The serviceexpiry.
        /// </param>
        /// <param name="servicelastchecked">
        /// The servicelastchecked.
        /// </param>
        /// <param name="servicecheckedby">
        /// The servicecheckedby.
        /// </param>
        /// <param name="defaultunit">
        /// The defaultunit.
        /// </param>
        /// <param name="enginesize">
        /// The enginesize.
        /// </param>
        /// <param name="mileagecats">
        /// The mileagecats.
        /// </param>
        /// <param name="udfs">
        /// The udfs.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="exemptfromhometooffice">
        /// The exemptfromhometooffice.
        /// </param>
        /// <param name="replacePreviousCar">
        /// The replace previous car.
        /// </param>
        /// <param name="previousCarId">
        /// The previous car id.
        /// </param>
        /// <param name="isAdmin">
        /// The is admin.
        /// </param>
        /// <param name="isShallowSave">
        /// The is shallow save.
        /// </param>
        /// <param name="vehicletypeid">The vehicle type (Car etc)</param>
        /// <param name="taxExpiry">The date of the Tax expiry</param>
        /// <param name="taxStatus">The tax status "Taxed" or not</param>
        /// <param name="motExpiry">The date of the MOT</param>
        /// <param name="motStatus">The MOT status "MOT" or not</param>
        /// <returns>
        /// The <see cref="object[]"/>.
        /// </returns>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public object[] saveCar(int carid, int employeeid, DateTime? startdate, DateTime? enddate, string make, string model, string registration, bool active, int vehicleEngineTypeId, Int64 startodometer, bool fuelcard, int endodometer, byte defaultunit, int enginesize, List<int> mileagecats, List<object> udfs, bool approved, bool exemptfromhometooffice, bool replacePreviousCar, int previousCarId, bool isAdmin, bool isShallowSave, byte? vehicletypeid, string taxExpiry, string taxStatus, string motExpiry, string motStatus)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCar car;
            var arrCarVals = new object[2];
            var clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

            var mileageUnit = (MileageUOM)defaultunit;

            var userdefined = new SortedList<int, object>();

            var clsModules = new cModules();
            cModule module = clsModules.GetModuleByID((int)user.CurrentActiveModule);
            string brandName = (module != null) ? module.BrandNamePlainText : "Expenses";
            bool carPreviouslyActive = false;


            foreach (object o in udfs)
            {
                userdefined.Add(Convert.ToInt32(((object[])o)[0]), ((object[])o)[1]);
            }

            bool emailMainAdmin = false;

            if (!isAdmin)
            {
                if (reqProperties.ActivateCarOnUserAdd)
                {
                    active = true;
                }
                else
                {
                    active = false;
                    emailMainAdmin = true;
                }
            }

            if (!approved && !isAdmin)
            {
                approved = false;
            }
            else
            {
                approved = true;
            }

            CarsBase clsCars = null;
            if (employeeid > 0) // if the car is an employee car instantiate through employeecars else pool car
            {
                clsCars = new cEmployeeCars(user.AccountID, employeeid);
            }
            else
            {
                clsCars = new cPoolCars(user.AccountID);
            }

            DateTime? taxExpiryDate = NullableDateTimeHelper.Parse(taxExpiry);
            if (taxExpiryDate == DateTime.MinValue)
            {
                taxExpiryDate = null;
            }

            DateTime? motExpiryDate = NullableDateTimeHelper.Parse(motExpiry);
            if (motExpiryDate == DateTime.MinValue)
            {
                motExpiryDate = null;
            }

            var taxValid = taxStatus.ToLower() == "true";
            var motValid = motStatus.ToLower() == "true";

            if (carid > 0)
            {
                cCar oldcar = clsCars.GetCarByID(carid);
                carPreviouslyActive = oldcar.active;
                if (!taxExpiryDate.HasValue)
                {
                    taxExpiryDate = oldcar.TaxExpiry;
                    taxValid = oldcar.IsTaxValid;
                    motExpiryDate = oldcar.MotExpiry;
                    motValid = oldcar.IsMotValid;
                }

                car = new cCar(user.AccountID, employeeid, carid, make, model, registration, startdate, enddate, active, mileagecats, vehicleEngineTypeId, startodometer, fuelcard, endodometer,  mileageUnit, enginesize, oldcar.createdon, oldcar.createdby, DateTime.Now, user.EmployeeID, approved, exemptfromhometooffice, vehicletypeid, taxExpiryDate, taxValid, motExpiryDate, motValid);
            }
            else
            {
                car = new cCar(user.AccountID, employeeid, carid, make, model, registration, startdate, enddate, active, mileagecats, vehicleEngineTypeId, startodometer, fuelcard, endodometer, mileageUnit, enginesize, DateTime.Now, user.EmployeeID, null, null, approved, exemptfromhometooffice, vehicletypeid, taxExpiryDate, taxValid, motExpiryDate, motValid);
            }

            arrCarVals[0] = clsCars.SaveCar(car);

            
            if (reqProperties.VehicleLookup && car.employeeid > 0 && carid == 0)
            {
                if (reqProperties.BlockTaxExpiry && taxExpiryDate.HasValue)
                {
                    var taxRepo = new TaxDocumentRepository(user, 
                        new cCustomEntities(user),
                        new cFields(user.AccountID),
                        new cTables(user.AccountID));
                    taxRepo.Add(taxExpiryDate.Value, (int)arrCarVals[0]);
                }

                if (reqProperties.BlockMOTExpiry)
                {
                    if (reqProperties.BlockMOTExpiry && motExpiryDate.HasValue)
                    {
                        var taxRepo = new MotDocumentRepository(user, 
                            new cCustomEntities(user),
                            new cFields(user.AccountID),
                            new cTables(user.AccountID));
                        taxRepo.Add(motExpiryDate.Value, (int)arrCarVals[0]);
                    }      
                }
            }

            clsCars.SaveUserDefinedFieldsValues((int)arrCarVals[0], userdefined, user,car.registration);

            if (!isShallowSave && emailMainAdmin && !reqProperties.ActivateCarOnUserAdd)
            {
                clsCars.NotifyAdminOfNewVehicle(employeeid, user, reqProperties, (int)arrCarVals[0]);

           
            }

            if (!carPreviouslyActive && car.active && employeeid > 0)
            {
                this.SendCarIsActiveEmail(user, employeeid, (int)arrCarVals[0], car.registration, reqProperties);
            }

            if (reqProperties.ActivateCarOnUserAdd)
            {
                arrCarVals[1] = true;
            }
            else
            {
                arrCarVals[1] = false;
            }

            // set the previous car to inactive and update its end date to be the start date of the new car
            if (replacePreviousCar && previousCarId > 0)
            {
                var setEndDate = reqProperties.AllowEmpToSpecifyCarStartDateOnAdd;              
                var previousCar = clsCars.GetCarByID(previousCarId);
                clsCars.UpdateVehicleBeingReplaced(previousCar, car.startdate, user.EmployeeID, setEndDate);
            }

            return arrCarVals;
        }

        /// <summary>
        /// Lookup a vehicle details from an external service based on the registration number (UK only).
        /// </summary>
        /// <param name="registrationNumber">The registration nunmber to lookup</param>
        /// <returns>An instance of <see cref="cCar"/> or null is the lookup fails.</returns>
        [WebMethod(EnableSession=true)]
        public cCar LookupVehicle(string registrationNumber)
        {
            var user = cMisc.GetCurrentUser();
            cCar result = null;
            var subAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties reqProperties = subAccounts.getFirstSubAccount().SubAccountProperties.Clone();
            if (!reqProperties.VehicleLookup)
            {
                return null;
            }

            var dvlaApi = BootstrapDvla.CreateNew();
            var lookupResult = dvlaApi.Lookup(registrationNumber, BootstrapDvla.CreateLogger(user));
            if (lookupResult.Code == "200")
            {
                if (lookupResult.Code != "200")
                {
                    return null;
                }

                result = new LookupServiceCar(user.AccountID,
                    user.EmployeeID,
                    0,
                    lookupResult.Vehicle.Make,
                    lookupResult.Vehicle.Model,
                    lookupResult.Vehicle.RegistrationNumber,
                    null,
                    null,
                    false,
                    new List<int>(),
                    FuelTypeFactory.Convert(lookupResult.Vehicle.FuelType, user)
                    ,
                    0,
                    false,
                    0,
                    MileageUOM.Mile,
                    lookupResult.Vehicle.EngineCapacity,
                    null,
                    user.EmployeeID,
                    null,
                    null,
                    false,
                    false,
                    (byte?) VehicleTypeFactory.Convert(lookupResult.Vehicle.VehicleType),
                    lookupResult.Vehicle.TaxExpiry,
                    lookupResult.Vehicle.TaxStatus == "Taxed",
                    lookupResult.Vehicle.MotExpiry,
                    lookupResult.Vehicle.MotStatus == "MOT");
                
                return result;
            }

            return null;
        }


        /// <summary>
        /// Send a "car is active email".
        /// </summary>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="employeeid">
        /// The employee id.
        /// </param>
        /// <param name="car">The id of the car that has been activated.</param>
        /// <param name="registrationNumber">The  registration number of the car that has been activated</param>
        /// <param name="accountProperties">
        /// The account Properties.
        /// </param>
        private void SendCarIsActiveEmail(ICurrentUserBase user, int employeeid, int carId, string registrationNumber, cAccountProperties accountProperties)
        {
            var notifications = new NotificationTemplates((ICurrentUser)user);
            var clsEmployees = new cEmployees(user.AccountID);
            var reqEmployee = clsEmployees.GetEmployeeById(employeeid);
            var templateName = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenaVehicleHasBeenActivatedForUse);
            try
            {
                string msgFrom;

                if (accountProperties.SourceAddress == 1)
                {
                    msgFrom = accountProperties.EmailAdministrator.Trim() == string.Empty ? "admin@sel-expenses.com" : accountProperties.EmailAdministrator;
                }
                else
                {
                    if (reqEmployee.EmailAddress != string.Empty)
                    {
                        msgFrom = reqEmployee.EmailAddress;
                    }
                    else
                    {
                        // If no email address set then send from admin
                        msgFrom = accountProperties.EmailAdministrator.Trim() == string.Empty ? "admin@sel-expenses.com" : accountProperties.EmailAdministrator;
                    }
                }

                var senderId = user.EmployeeID;
                int[] recipients = { reqEmployee.EmployeeID };
                notifications.SendMessage(templateName, senderId, recipients, carId, defaultSender: msgFrom);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Failed to send vehicle activation email\nAccountID: " + user.AccountID + "\nCar: " + registrationNumber + "\n Exception:" + ex.Message);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] createOdoGrid(int carid)
        {
            string sHtml = "<a href=\"javascript:odometerid=0;showOdometerModal(true)\">Add Odometer Reading</a>";
            CurrentUser user = cMisc.GetCurrentUser();

            var clsEmps = new cEmployees(user.AccountID);

            cGridNew newgrid = null;

            newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridOdometerReadings", clsEmps.getOdometerGrid());

            newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("carid")).field, ConditionType.Equals, new object[] { carid }, null, ConditionJoiner.None);

            newgrid.EmptyText = "There are currently no Odometer Readings for this vehicle";
            newgrid.enabledeleting = true;
            newgrid.enableupdating = true;
            newgrid.editlink = "javascript:editOdometerReading({odometerid});";
            newgrid.deletelink = "javascript:deleteOdometerReading({odometerid});";
            newgrid.KeyField = "odometerid";
            newgrid.getColumnByName("carid").hidden = true;
            newgrid.getColumnByName("odometerid").hidden = true;

            newgrid.KeyField = "carid";
            string[] gridData = newgrid.generateGrid();
            sHtml += gridData[1];

            var retVals = new List<string> { newgrid.GridID, gridData[0], sHtml };

            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void deleteOdometerReading(int carID, int odometerID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsEmps = new cEmployees(user.AccountID);

            clsEmps.deleteOdometerReading(user.EmployeeID, carID, odometerID);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int saveOdometerReading(int odometerid, int carID, int employeeID, DateTime readingDate, int oldReading, int newReading)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsEmps = new cEmployees(user.AccountID);

            int odometerId = clsEmps.saveOdometerReading(odometerid, employeeID, carID, readingDate, oldReading, newReading, 2);
            return odometerId;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cOdometerReading getOdometerReading(int odometerID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsEmps = new cEmployees(user.AccountID);

            return clsEmps.getOdometerReadingByID(odometerID);
        }

        /// <summary>
        /// This method Displays Mileage grid on vehicle popup for the selected engine type,UOM,Financial year 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="financialYearId"></param>
        /// <param name="vehicleEngineTypeId"></param>
        /// <returns>VJR records to bind the cGridNew</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] createMileageGrid(string unit, int financialYearID, int vehicleEngineTypeId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridMileage", "select mileageid, carsize, unit, catvalid, financialYearID,VehicleEngineTypeId from MileageCategoriesView");
            clsgrid.getColumnByName("mileageid").hidden = true;
            clsgrid.getColumnByName("unit").hidden = true;
            clsgrid.getColumnByName("VehicleEngineTypeId").hidden = true;
            var clsFields = new cFields(user.AccountID);
            // don't show incomplete categories
            clsgrid.getColumnByName("catvalid").hidden = true;
            clsgrid.getColumnByName("financialYearID").hidden = true;
            clsgrid.addFilter(clsFields.GetFieldByID(new Guid("E9ACA009-D0E4-4AAD-A039-8E1D9667CCEB")), ConditionType.Equals, new object[] { unit }, null, ConditionJoiner.And);
            clsgrid.addFilter(clsFields.GetFieldByID(new Guid("77A1DB6D-61CD-49ED-BBA0-8FD9234380D1")), ConditionType.Equals, new object[] { vehicleEngineTypeId }, null, ConditionJoiner.And);
            clsgrid.addFilter(clsFields.GetFieldByID(new Guid("9240EF60-86DD-4599-969C-1B6F370ECA0D")), ConditionType.Equals, new object[] { true }, null, ConditionJoiner.And);
            clsgrid.addFilter(clsFields.GetFieldByID(new Guid("D97BDF16-2BC5-4745-BD62-386CF2F4A711")), ConditionType.Equals, new object[] { financialYearID }, null, ConditionJoiner.And);
            clsgrid.EnableSelect = true;
            clsgrid.KeyField = "mileageid";
            clsgrid.SortedColumn = clsgrid.getColumnByName("unit");
            clsgrid.enablepaging = false;
            clsgrid.EmptyText = "No vehicle journey rates to display";
            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void updateEmployeeCache(int editedEmployeeID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cEmployees clsEmps = new cEmployees(currentUser.AccountID);

            clsEmps.UpdateEmployeeModifiedOn(editedEmployeeID);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void updatePoolCarCache(int carID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

            clsPoolCars.UpdateCarModifiedOn(carID);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public CarReturnVal deleteCar(int employeeid, int carid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployeeCars clsemployeecars = new cEmployeeCars(user.AccountID, employeeid);
            CarReturnVal returnVal = clsemployeecars.DeleteCar(carid);

            return returnVal;
        }

        /// <summary>
        /// Create ESR grid.
        /// </summary>
        /// <param name="carid">
        /// The car id.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string createEsrDetails(int carid)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cGridNew newgrid = null;

            var sqlScript = string.Format("select ESRVehicleAllocationId from CarAssignmentNumberAllocations where Carid = {0} ", carid);

            var connection = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            var data = connection.GetDataSet(sqlScript);

            var employees = new svcEmployees();

            var results = new StringBuilder();

            foreach (DataRow row in data.Tables[0].Rows)
            {
                long id = 0;
                long.TryParse(row[0].ToString(), out id);
                results.Append(string.Format("<div class='sectiontitle'>ESR Vehicle Details - {0}</div>", id));
                results.Append(employees.GetEsrDetails(5, id));
            }

            return results.ToString();
        }    

        /// <summary>
        /// Get entity and view id
        /// </summary>
        /// <param name="entity">entity systemguid</param>
        /// <param name="view">view systemguid</param>
        /// <returns>entity and view id</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetDocEntityAndViewId(string entity, string view)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var results = new DutyOfCareDocumentsInformation().GetDocEntityAndViewIdByGuid(entity, view, currentUser.AccountID);
            return results.ToString();
        }

        [WebMethod(EnableSession=true)]
        public ListItem[] GetFinancialYears(int employeeId)
        { 
            var result = new List<ListItem>();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var years = FinancialYears.ActiveYears(currentUser);
            int financialYearID = 0;
            var employeeHasYear = false;
            CarsBase clsCars = null;
            if (employeeId == 0)
            {
                clsCars = new cPoolCars(currentUser.AccountID);
            }
            else
            {
                clsCars = new cEmployeeCars(currentUser.AccountID, employeeId);
            }

            if (employeeId > 0)
            {
                var carList = clsCars.CreateDropDownArray();
                var mileagecats = new cMileagecats(currentUser.AccountID);
                for (int i = 0; i <= carList.GetUpperBound(0); i++)
                {
                    var mileageCats = clsCars.GetCarByID(int.Parse(carList[i].Value)).mileagecats;
                    if (mileageCats.Count > 0)
                    {
                        var yearID = mileagecats.GetMileageCatById(mileageCats[0]).FinancialYearID;
                        if (yearID != null)
                        {
                            financialYearID = (int)yearID;
                            employeeHasYear = true;
                            break;
                        }
                    }
                }
            }

            foreach (FinancialYear year in years)
            {
                if ((employeeHasYear && financialYearID == year.FinancialYearID) || !employeeHasYear)
                {
                    result.Add(new ListItem(year.Description.TrimEnd(), year.FinancialYearID.ToString()));    
                }
            }

            result[0].Selected = true;

            return result.ToArray();
        }

        [WebMethod]
        public ListItem[] CreateCurrentValidCarDropDown(int accountId, int employeeId, DateTime date)
        {
            cEmployeeCars cars = new cEmployeeCars(accountId, employeeId);
            return cars.CreateCurrentValidCarDropDown(date,fromAeExpenses:true).ToArray();
        }

        /// <summary>
        /// Indicates whether or not an employee has at least one car with a given registration number
        /// </summary>
        /// <param name="employeeId">The employee to check, 0 if there is no employee (pool cars)</param>
        /// <param name="registration">The car registration</param>
        /// <returns>True if a match was found</returns>
        [WebMethod(EnableSession = true)]
        public bool CheckDuplicateCarRegistration(int employeeId, string registration)
        {
            var user = cMisc.GetCurrentUser();
            var cars = new cEmployeeCars(user.AccountID, employeeId);
            var car = cars.GetCarByRegistration(registration);

            return car != null;
        }

        /// <summary>
        /// Indicates whether or not an account has at least one pool car with a given registration number
        /// </summary>
        /// <param name="registration">The car registration</param>
        /// <returns>True if a match was found</returns>
        [WebMethod(EnableSession = true)]
        public bool CheckDuplicatePoolCarRegistration(string registration)
        {
            var user = cMisc.GetCurrentUser();
            var cars = new cPoolCars(user.AccountID);
            var car = cars.GetCarByRegistration(registration);

            return car != null;
        }
    }
}
