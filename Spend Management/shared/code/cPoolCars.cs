using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    using SpendManagementLibrary.Employees;

    using Spend_Management.expenses.code;

    /// <summary>
    /// Car class for dealing with pool cars, caches account's pool cars
    /// </summary>
    public class cPoolCars : cCarsBase
    {
        Utilities.DistributedCaching.Cache cache = new Utilities.DistributedCaching.Cache();

        #region properties
        /// <summary>
        /// The list of all pool cars
        /// </summary>
        public List<cCar> Cars
        {
            get { return lstCars; }
        }
        #endregion properties

        /// <summary>
        /// Constructor to obtain the full list of pool cars
        /// </summary>
        /// <param name="accountID"></param>
        public cPoolCars(int accountID)
        {
            lstCars = null;
            nAccountID = accountID;
            sConnectionString = cAccounts.getConnectionString(accountID);
            clsTables = new cTables(accountID);
            clsFields = new cFields(accountID);
            clsUserDefinedFields = new cUserdefinedFields(accountID);

            InitialiseData();
        }

        private void InitialiseData()
        {
            this.lstCars = cache.Get(this.nAccountID, CacheKey, string.Empty) as List<cCar>;
            if (lstCars == null)
            {
                CacheList();
            }
        }

        private void CacheList()
        {
            lstCars = GetPoolCarsCollection();

            cache.Add(this.nAccountID, CacheKey, string.Empty, lstCars);
        }

        private void ResetCache()
        {
            cache.Delete(this.nAccountID, CacheKey, string.Empty); 
            lstCars = null;
            InitialiseData();
        }

        public const string CacheKey = "poolCars" ;

        /// <summary>
        /// Saves a car
        /// </summary>
        /// <param name="oCar">A car object</param>
        /// <param name="clearCache">If true, clear the chached list of cars.</param>
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
        /// <param name="carID">Car ID</param>
        public override CarReturnVal DeleteCar(int carID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            CarReturnVal returnValue = base.DeleteCarFromDB(carID, currentUser);
            ResetCache();
            return returnValue;
        }
   
        /// <summary>
        /// Generate a cGridNew string of all pool cars
        /// </summary>
        /// <param name="employeeID">Optional EmployeeID if CurrentUser is not available</param>
        /// <returns>HTML String</returns>
        public string[] CreatePoolCarsGrid(int employeeID = 0)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string sSQL = "SELECT cars.carid, cars.employeeid, cars.vehicletypeid, cars.make, cars.model, cars.registration, cars.startdate, cars.enddate, cars.cartypeid, cars.enginesize FROM dbo.cars";
            cGridNew gridPoolCars = null;
            if (employeeID == 0 && currentUser != null)
            {
                gridPoolCars = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridPoolCars", sSQL);
            }
            else
            {
                gridPoolCars = new cGridNew(nAccountID, employeeID, "gridPoolCars", sSQL);
            }

            gridPoolCars.KeyField = "carid";
            gridPoolCars.getColumnByName("carid").hidden = true;
            gridPoolCars.getColumnByName("employeeid").hidden = true;
            gridPoolCars.EmptyText = "There are no pool vehicles to display.";

            gridPoolCars.enableupdating = true;
            gridPoolCars.enabledeleting = true;
            gridPoolCars.editlink = "aepoolcar.aspx?action=2&carid={carid}";
            gridPoolCars.deletelink = "javascript:DeletePoolCar({carid});";
            svcCars.AddVehicleTypes(ref gridPoolCars);

            gridPoolCars.addFilter(clsFields.GetFieldByID(new Guid("5DDBF0EF-FA06-4E7C-A45A-54E50E33307E")), ConditionType.DoesNotContainData, null, null, ConditionJoiner.None);
            List<string> retVals = new List<string>();
            retVals.Add(gridPoolCars.GridID);
            retVals.AddRange(gridPoolCars.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="employeeID">Optional EmployeeID if CurrentUser is not available</param>
        /// <returns></returns>
        public string[] CreatePoolCarsUsersGrid(int carID, int employeeID = 0)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string sSQL = "SELECT pool_car_users.carid, pool_car_users.employeeid, employees.username, employees.title, employees.firstname, employees.surname FROM dbo.pool_car_users";
            cGridNew gridPoolCarUsers = null;
            if (employeeID == 0 && currentUser != null)
            {
                gridPoolCarUsers = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridPoolCarUsers", sSQL);
            }
            else
            {
                gridPoolCarUsers = new cGridNew(nAccountID, employeeID, "gridPoolCarUsers", sSQL);
            }

            gridPoolCarUsers.KeyField = "employeeid";
            gridPoolCarUsers.getColumnByName("carid").hidden = true;
            gridPoolCarUsers.getColumnByName("employeeid").hidden = true;
            gridPoolCarUsers.EmptyText = "There are no pool vehicles users assigned to this vehicle.";

            gridPoolCarUsers.enableupdating = false;
            gridPoolCarUsers.enabledeleting = true;
            gridPoolCarUsers.deletelink = "javascript:DeletePoolCarUser({employeeid});";

            gridPoolCarUsers.addFilter(clsFields.GetFieldByID(new Guid("E7DA8FD2-4BAB-453B-8678-CE64A7B0859B")), ConditionType.Equals, new object[] { carID }, null, ConditionJoiner.None);

            List<string> retVals = new List<string>();
            retVals.Add(gridPoolCarUsers.GridID);
            retVals.AddRange(gridPoolCarUsers.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID">Optional EmployeeID if CurrentUser is not available</param>
        /// <returns></returns>
        public string[] CreatePoolCarEmployeeGrid(int employeeID = 0)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string sSQL = "SELECT employees.employeeid, employees.username, employees.title, employees.firstname, employees.surname FROM dbo.employees";
            cGridNew gridPoolCarEmployees = null;
            if (employeeID == 0 && currentUser != null)
            {
                gridPoolCarEmployees = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridPoolCarEmployees", sSQL);
            }
            else
            {
                gridPoolCarEmployees = new cGridNew(nAccountID, employeeID, "gridPoolCarEmployees", sSQL);
            }

            gridPoolCarEmployees.KeyField = "employeeid";
            gridPoolCarEmployees.getColumnByName("employeeid").hidden = true;
            gridPoolCarEmployees.EmptyText = "There are no relevant employees to add.";

            gridPoolCarEmployees.enableupdating = false;
            gridPoolCarEmployees.enabledeleting = false;
            gridPoolCarEmployees.EnableSelect = true;
            gridPoolCarEmployees.pagesize = 10;

            gridPoolCarEmployees.addFilter(clsFields.GetFieldByID(new Guid("3A6A93F0-9B30-4CC2-AFC4-33EC108FA77A")), ConditionType.Equals, new object[] { false }, null, ConditionJoiner.None);
            List<string> retVals = new List<string>();
            retVals.Add(gridPoolCarEmployees.GridID);
            retVals.AddRange(gridPoolCarEmployees.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// Add an employee pool car allocation
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="employeeID"></param>
        public void AddPoolCarUser(int carID, int employeeID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            base.AddPoolCarUserToDB(carID, employeeID, currentUser);
            ResetCache();
        }

        /// <summary>
        /// Delete all users associated to a pool car ID
        /// </summary>
        /// <param name="carID">Pool Car ID</param>
        public void DeletePoolCarUsers(int carID)
        {
            base.DeletePoolCarUsersFromCarID(carID);
            ResetCache();
        }

        /// <summary>
        /// Remove a user pool car association
        /// </summary>
        /// <param name="employeeID">Employee ID</param>
        /// <param name="carID">Car ID</param>
        public void DeleteUserFromPoolCar(int employeeID, int carID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            base.DeletePoolCarUserFromDB(carID, employeeID, currentUser);
            ResetCache();
        }

        /// <summary>
        /// Get a list of employees assigned to use a pool car
        /// </summary>
        /// <param name="carID">Pool Car ID</param>
        /// <returns>List of employee ids</returns>
        public List<int> GetUsersPerPoolCar(int carID)
        {
            DBConnection expdata = new DBConnection(sConnectionString);
            List<int> lstUsers = new List<int>();
            System.Data.SqlClient.SqlDataReader reader;

            string sSQL = "SELECT employeeid FROM pool_car_users WHERE carid = @carid";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            using (reader = expdata.GetReader(sSQL))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    lstUsers.Add(reader.GetInt32(0)); //reader.GetOrdinal("employeeid")));
                }
                reader.Close();
            }

            return lstUsers;
        }

        /// <summary>
        /// Update an individual car's modified on for cache update
        /// </summary>
        /// <param name="carId">Car ID</param>
        /// <param name="employeeID">Optional EmployeeID if CurrentUser is not available</param>
        public void UpdateCarModifiedOn(int carId, int employeeID = 0)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(sConnectionString);
            if (employeeID == 0 && currentUser != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", currentUser.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@carId", carId);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.ExecuteProc("updateCarModifiedOn");
            expdata.sqlexecute.Parameters.Clear();
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
        public override void NotifyAdminOfNewVehicle(int employeeId, ICurrentUserBase user, cAccountProperties accountProperties, int vehicleId)
        {
            var emailTemplates = new cEmailTemplates((ICurrentUser)user);
            var employees = new cEmployees(user.AccountID);
            Employee reqEmployee = employees.GetEmployeeById(employeeId);
            var emailMessage = emailTemplates.DetermineDefaultSender(accountProperties, reqEmployee.EmailAddress);

            try
            {      
                var templateName = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentWhenaVehicleHasBeenAdded);

                var senderId = user.EmployeeID;
                int[] recipientIds = { accountProperties.MainAdministrator };
                emailTemplates.SendMessage(templateName, senderId, recipientIds, vehicleId, defaultSender: emailMessage);
            }
            catch (Exception)
            {
                cEventlog.LogEntry(
                    "Failed to send vehicle activation email\nAccountID: " + user.AccountID + "\nCarID: " + vehicleId);
            }
        }
    }
}
