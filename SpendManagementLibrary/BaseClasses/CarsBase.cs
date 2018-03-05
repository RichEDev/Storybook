namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Web.UI.WebControls;
    using Microsoft.SqlServer.Server;
    using System.Data;
    using System.Linq;

    using Enumerators;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using System.Reflection;
    using System.ComponentModel;
    using System.Globalization;

    public abstract class CarsBase
    {
        protected List<cCar> lstCars;
        protected int nEmployeeID;
        protected int nAccountID;
        protected string sConnectionString;
        protected string sCacheKeyPrefix;
        protected cTables clsTables;
        protected cFields clsFields;
        protected cUserDefinedFieldsBase clsUserDefinedFields;

        /// <summary>
        /// Returns the list of cars and pool cars for an employee
        /// </summary>
        /// <param name="employeeID">The employee to return the cars for</param>
        /// <param name="connection">A connection object to override the standard one for unit testing</param>
        /// <returns>The list of cars</returns>
        protected List<cCar> GetEmployeeCarsCollection(int employeeID)
        {
            using (IDBConnection expdata = new DatabaseConnection(sConnectionString))
            {
                var cars = new List<cCar>();

                cTable tbl = clsTables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
                cTable udftbl = clsTables.GetTableByID(tbl.UserDefinedTableID);

                const string SQL = "SELECT employeeid, carid, make, model, registration, startdate, enddate, cartypeid, odometer, active, fuelcard, endodometer, createdon, createdby, modifiedon, modifiedby, default_unit, enginesize, approved, exemptfromhometooffice, vehicletypeid, taxexpiry, taxvalid, motexpiry, motvalid FROM dbo.cars WHERE employeeid = @employeeid OR carid IN (SELECT carid FROM dbo.pool_car_users WHERE employeeid = @employeeid)";

                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);

                cars = this.ExtractCarsFromReader(expdata, SQL);


                return cars.ToList();
            }
        }

        private List<cCar> ExtractCarsFromReader(IDBConnection expdata, string sql)
        {
            var cars = new List<cCar>();
            var ids = new List<int>();


            using (IDataReader reader = expdata.GetReader(sql))
            {
                var employeeIdOrd = reader.GetOrdinal("employeeid");
                var carIdOrd = reader.GetOrdinal("carid");
                var makeOrd = reader.GetOrdinal("make");
                var modelOrd = reader.GetOrdinal("model");
                var registrationOrd = reader.GetOrdinal("registration");
                var startDateOrd = reader.GetOrdinal("startdate");
                var endDateOrd = reader.GetOrdinal("enddate");
                var carTypeIdOrd = reader.GetOrdinal("cartypeid");
                var odomoterOrd = reader.GetOrdinal("odometer");
                var activeOrd = reader.GetOrdinal("active");
                var fuelcardOrd = reader.GetOrdinal("fuelcard");
                var endOdomoterOrd = reader.GetOrdinal("endodometer");
                var createdOnOrd = reader.GetOrdinal("CreatedOn");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedOn");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var defaultUnitOrd = reader.GetOrdinal("default_unit");
                var engineSizeOrd = reader.GetOrdinal("enginesize");
                var approvedOrd = reader.GetOrdinal("approved");
                var exemptOrd = reader.GetOrdinal("exemptfromhometooffice");
                var vehicleTypeOrd = reader.GetOrdinal("vehicletypeid");
                var taxExpiryOrd = reader.GetOrdinal("taxexpiry");
                var taxValidOrd = reader.GetOrdinal("taxvalid");
                var motExpiryOrd = reader.GetOrdinal("motexpiry");
                var motValidOrd = reader.GetOrdinal("motvalid");
                while (reader.Read())
                {
                    int caremployeeid = reader.IsDBNull(employeeIdOrd) ? 0 : reader.GetInt32(employeeIdOrd);
                    int carid = reader.GetInt32(carIdOrd);
                    string make = reader.GetString(makeOrd);
                    string model = reader.GetString(modelOrd);
                    string registration = reader.GetString(registrationOrd);
                    DateTime startdate = reader.IsDBNull(startDateOrd)
                        ? new DateTime(1900, 01, 01)
                        : reader.GetDateTime(startDateOrd);
                    DateTime enddate = reader.IsDBNull(endDateOrd)
                        ? new DateTime(1900, 01, 01)
                        : reader.GetDateTime(endDateOrd);
                    int vehicleEngineTypeId = reader.IsDBNull(carTypeIdOrd) ? 0 : reader.GetByte(carTypeIdOrd);
                    Int64 odometer = reader.IsDBNull(odomoterOrd) ? 0 : reader.GetInt64(odomoterOrd);
                    bool active = reader.GetBoolean(activeOrd);
                    bool fuelcard = reader.GetBoolean(fuelcardOrd);
                    int endodometer = reader.IsDBNull(endOdomoterOrd) ? 0 : reader.GetInt32(endOdomoterOrd);
                    DateTime createdon = reader.IsDBNull(createdOnOrd)
                        ? new DateTime(1900, 01, 01)
                        : reader.GetDateTime(createdOnOrd);
                    int createdby = reader.IsDBNull(createdByOrd) ? 0 : reader.GetInt32(createdByOrd);
                    DateTime modifiedon = reader.IsDBNull(modifiedOnOrd)
                        ? new DateTime(1900, 01, 01)
                        : reader.GetDateTime(modifiedOnOrd);
                    int modifiedby = reader.IsDBNull(modifiedByOrd) ? 0 : reader.GetInt32(modifiedByOrd);
                    MileageUOM defaultuom = (MileageUOM) reader.GetByte(defaultUnitOrd);
                    int engineSize = reader.IsDBNull(engineSizeOrd) ? 0 : reader.GetInt32(engineSizeOrd);
                    bool approved = reader.IsDBNull(approvedOrd) || reader.GetBoolean(approvedOrd);
                    bool exemptfromhometooffice = reader.GetBoolean(exemptOrd);
                    byte vehicletypeid = reader.IsDBNull(vehicleTypeOrd) ? (byte) 0 : reader.GetByte(vehicleTypeOrd);
                    DateTime? taxExpiry = null;
                    if (!reader.IsDBNull(taxExpiryOrd))
                    {
                        taxExpiry = reader.GetDateTime(taxExpiryOrd);
                    }

                    var taxValid = !reader.IsDBNull(taxValidOrd) && reader.GetBoolean(taxValidOrd);

                    DateTime? motExpiry = null;
                    if (!reader.IsDBNull(motExpiryOrd))
                    {
                        motExpiry = reader.GetDateTime(motExpiryOrd);
                    }

                    var motValid = !reader.IsDBNull(motValidOrd) && reader.GetBoolean(motValidOrd);

                    cars.Add(
                        new cCar(this.nAccountID, caremployeeid, carid, make, model, registration, startdate, enddate, active,
                            new List<int>(), vehicleEngineTypeId, odometer, fuelcard, endodometer, defaultuom, engineSize,
                            createdon, createdby, modifiedon, modifiedby, approved, exemptfromhometooffice, vehicletypeid,
                            taxExpiry, taxValid, motExpiry, motValid));
                    ids.Add(carid);
                }

                reader.Close();
            }

            if (ids.Count > 0)
            {
                SortedList<int, List<int>> carMileageCategories = this.GetCarMileageCats(ids);
                foreach (cCar car in cars)
                {
                    if (carMileageCategories.ContainsKey(car.carid))
                    {
                        car.mileagecats = carMileageCategories[car.carid];
                    }
                }
            }

            return cars;
        }

        public SortedList<int, object> GetUserDefinedFieldsList(int carId)
        {
            var tbl = clsTables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
            var udftbl = clsTables.GetTableByID(tbl.UserDefinedTableID);
            var userdefined = clsUserDefinedFields.GetRecords(udftbl, new List<int> { carId }, clsTables, clsFields);
            if (userdefined.ContainsKey(carId))
            {
                return userdefined[carId];
            }

            return null;
        }

        /// <summary>
        /// Get the list of all pool cars
        /// </summary>
        /// <returns></returns>
        protected List<cCar> GetPoolCarsCollection()
        {
            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                string strsql = "SELECT * FROM cars WHERE employeeid IS NULL";
                return this.ExtractCarsFromReader(expdata, strsql);
            }
        }

        /// <summary>
        /// Find an individual car in the database from its ID
        /// </summary>
        /// <param name="carid">the car id to find</param>
        /// <param name="ensureEmployeeIsPopulated">Ensure whether the EmployeeId is populated. Note that this has been added as a fix for the API.</param>
        /// <returns>Car details</returns>
        public cCar GetCarFromDB(int carID, bool ensureEmployeeIsPopulated = false)
        {
            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                string strsql;
                strsql = "select * from cars where carid = @carid;";
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
                return this.ExtractCarsFromReader(expdata, strsql).FirstOrDefault();
            }
        }

        protected List<int> GetCarMileageCats(int carID)
        {
            DBConnection expdata = new DBConnection(sConnectionString);
            List<int> lstMileageCats = new List<int>();
            SqlDataReader reader;
            string strSQL;

            //strsql = "select mileageid from car_mileagecats where carid = @carid;
            strSQL = "select car_mileagecats.mileageid from car_mileagecats inner join mileage_categories on mileage_categories.mileageid = car_mileagecats.mileageid where car_mileagecats.carid = @carid and mileage_categories.catvalid = @catvalid";

            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            expdata.sqlexecute.Parameters.AddWithValue("@catvalid", true);
            using (reader = expdata.GetReader(strSQL))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    lstMileageCats.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            return lstMileageCats;
        }

        /// <summary>
        /// Gets a single odometer reading by it's id.
        /// </summary>
        /// <returns>A single Odometer reading.</returns>
        public cOdometerReading GetOdometerReadingById(int id)
        {
            cOdometerReading reading = null;
            var expdata = new DBConnection(sConnectionString);
            expdata.sqlexecute.Parameters.AddWithValue("@id", id);
            const string Strsql = "SELECT odometerid, carid, datestamp, oldreading, newreading, createdon, createdby FROM odometer_readings WHERE odometerid = @id";
            using (var reader = expdata.GetReader(Strsql))
            {
                if (reader.Read())
                {
                    var odometerid = reader.GetInt32(0);
                    var carid = reader.GetInt32(1);
                    var datestamp = reader.GetDateTime(2);
                    var oldodometer = reader.GetInt32(3);
                    var newodometer = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                    var createdon = reader.IsDBNull(5) ? new DateTime(1900, 01, 01) : reader.GetDateTime(5);
                    var createdby = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    reading = new cOdometerReading(odometerid, carid, datestamp, oldodometer, newodometer, createdon, createdby);
                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            return reading;
        }


        /// <summary>
        /// Gets every odometer reading in the database.
        /// </summary>
        /// <returns>A list of Odometer readings.</returns>
        public IList<cOdometerReading> GetAllOdometerReadings()
        {
            var expdata = new DBConnection(sConnectionString);
            var readings = new List<cOdometerReading>();

            const string Strsql = "SELECT odometerid, carid, datestamp, oldreading, newreading, createdon, createdby FROM odometer_readings";
            using (var reader = expdata.GetReader(Strsql))
            {
                while (reader.Read())
                {
                    var odometerid = reader.GetInt32(0);
                    var carid = reader.GetInt32(1);
                    var datestamp = reader.GetDateTime(2);
                    var oldodometer = reader.GetInt32(3);
                    var newodometer = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                    var createdon = reader.IsDBNull(5) ? new DateTime(1900, 01, 01) : reader.GetDateTime(5);
                    var createdby = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    readings.Add(new cOdometerReading(odometerid, carid, datestamp, oldodometer, newodometer, createdon, createdby));
                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            return readings;
        }


        /// <summary>
        /// Get the odometer readings for a car
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        public cOdometerReading[] GetOdometerReadings(int carID)
        {
            cOdometerReading[] readings;
            SqlDataReader reader;
            DBConnection expdata = new DBConnection(sConnectionString);
            string strsql;
            int count;
            int odometerid, oldodometer, newodometer, createdby;
            int i;
            DateTime datestamp, createdon;
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);

            strsql = "select count(*) from odometer_readings where carid = @carid";
            count = expdata.getcount(strsql);

            readings = new cOdometerReading[count];

            if (count != 0)
            {
                i = 0;
                strsql = "SELECT odometerid, datestamp, oldreading, newreading, createdon, createdby FROM odometer_readings WHERE carid = @carid";
                using (reader = expdata.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        odometerid = reader.GetInt32(0); //reader.GetOrdinal("odometerid")
                        datestamp = reader.GetDateTime(1); //reader.GetOrdinal("datestamp")
                        oldodometer = reader.GetInt32(2); //reader.GetOrdinal("oldreading")
                        if (reader.IsDBNull(3) == true) //reader.GetOrdinal("newreading")
                        {
                            newodometer = 0;
                        }
                        else
                        {
                            newodometer = reader.GetInt32(3); //reader.GetOrdinal("newreading")
                        }
                        if (reader.IsDBNull(4) == true) //reader.GetOrdinal("createdon")
                        {
                            createdon = new DateTime(1900, 01, 01);
                        }
                        else
                        {
                            createdon = reader.GetDateTime(4); //reader.GetOrdinal("createdon")
                        }
                        if (reader.IsDBNull(5) == true) //reader.GetOrdinal("createdby")
                        {
                            createdby = 0;
                        }
                        else
                        {
                            createdby = reader.GetInt32(5); //reader.GetOrdinal("createdby")
                        }
                        readings[i] = new cOdometerReading(odometerid, carID, datestamp, oldodometer, newodometer, createdon, createdby);
                        i++;
                    }
                    reader.Close();
                }
            }

            expdata.sqlexecute.Parameters.Clear();

            return readings;
        }

        /// <summary>
        /// Returns a car if any for the user match the registration
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public cCar GetCarByRegistration(string registration)
        {
            foreach (cCar car in lstCars)
            {
                if (car.registration.ToLower().Replace(" ", "") == registration.ToLower().Trim().Replace(" ", ""))
                {
                    return car;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a list of unapproved cars
        /// </summary>
        /// <returns></returns>
        public List<int> GetUnapprovedCars()
        {
            List<int> lstUnapprovedCars = new List<int>(); ;

            foreach (cCar car in lstCars)
            {
                if (car.Approved == false)
                {
                    lstUnapprovedCars.Add(car.carid);
                }
            }

            return lstUnapprovedCars;
        }

        /// <summary>
        /// Creates an array of listitems to populate a dropdown list with
        /// Uses "make model registration" text, "carid" value
        /// </summary>
        /// <returns></returns>
        public ListItem[] CreateDropDownArray()
        {
            List<ListItem> lstItems = new List<ListItem>();

            foreach (cCar car in lstCars)
            {
                lstItems.Add(new ListItem(car.make + " " + car.model + " " + car.registration, car.carid.ToString()));
            }

            return lstItems.ToArray();
        }

        /// <summary>
        /// Placeholder for the method that needs to be in the inheriting classes
        /// </summary>
        /// <param name="oCar">Car Object</param>
        /// <param name="clearCache">Clear the cache for the current object.</param>
        /// <returns>Car ID or error code</returns>
        public abstract int SaveCar(cCar oCar, bool clearCache = true);

        /// <summary>
        /// Save a car object to the database
        /// </summary>
        /// <param name="car">Car object</param>
        /// <param name="currentUser">The current user, who is saving the car</param>
        /// <returns>The ID of the car saved</returns>
        protected int SaveCarToDB(cCar car, cCurrentUserBase currentUser)
        {
            var data = new DBConnection(sConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@carid", car.carid);
            if (car.employeeid == 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeid", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeid", car.employeeid);
            }
            data.AddWithValue("@make", car.make, clsFields.GetFieldSize("cars", "make"));
            data.AddWithValue("@model", car.model, clsFields.GetFieldSize("cars", "model"));
            data.AddWithValue("@registration", car.registration, clsFields.GetFieldSize("cars", "registration"));
            data.sqlexecute.Parameters.AddWithValue("@defaultunit", Convert.ToByte(car.defaultuom));
            data.sqlexecute.Parameters.AddWithValue("@cartypeid", car.VehicleEngineTypeId);
            data.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(car.active));
            data.sqlexecute.Parameters.AddWithValue("@odometer", car.odometer);
            data.sqlexecute.Parameters.AddWithValue("@fuelcard", Convert.ToByte(car.fuelcard));
            data.sqlexecute.Parameters.AddWithValue("@endodometer", car.endodometer);
            data.sqlexecute.Parameters.AddWithValue("@engineSize", car.EngineSize);
            data.sqlexecute.Parameters.AddWithValue("@approved", car.Approved);
            data.sqlexecute.Parameters.AddWithValue("@vehicletypeid", car.VehicleTypeID);


            if (car.startdate == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@startdate", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@startdate", car.startdate);
            }

            var defaultDate = new DateTime(1900, 01, 01);
            if (car.enddate == null || car.enddate == defaultDate)
            {
                data.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@enddate", car.enddate);
            }

            data.sqlexecute.Parameters.AddWithValue("@exemptFromHomeToOffice", Convert.ToByte(car.ExemptFromHomeToOffice));
            if (car.carid > 0)
            {                            
                data.sqlexecute.Parameters.AddWithValue("@date", car.modifiedon);
                data.sqlexecute.Parameters.AddWithValue("@userid", car.modifiedby);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@date", car.createdon);
                data.sqlexecute.Parameters.AddWithValue("@userid", car.createdby);
            }

            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            if (car.TaxExpiry.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@taxexpiry", car.TaxExpiry.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@taxexpiry", DBNull.Value);
            }

            if (car.MotExpiry.HasValue)
            {
                data.sqlexecute.Parameters.AddWithValue("@motexpiry", car.MotExpiry.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@motexpiry", DBNull.Value);
            }

            data.sqlexecute.Parameters.AddWithValue("@Istaxvalid", car.IsTaxValid);
            data.sqlexecute.Parameters.AddWithValue("@Ismotvalid", car.IsMotValid);

            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveCar");
            int carid = (int)data.sqlexecute.Parameters["@identity"].Value;

            UpdateMileageCats(carid, car.mileagecats, currentUser);

            data.sqlexecute.Parameters.Clear();

            return carid;
        }

        /// <summary>
        /// Sets the previous car to inactive and update its end date to be the start date of the new car
        /// </summary>
        /// <param name="previousVehicle">
        /// The previous <see cref="cCar">cCar</see>.
        /// </param>
        /// <param name="vehicleEndDate">
        /// The vehicle end date.
        /// </param>
        /// <param name="modifiedByEmployeeId">
        /// The Id of the employee making the change
        /// </param>
        /// <param name="setEndDate">
        /// Whether the end date for the vehicle being replaced should be set.
        /// </param>
        /// <returns>
        /// The saved previous vehicle Id <see cref="int"/>.
        /// </returns>
        public int UpdateVehicleBeingReplaced(cCar previousVehicle, DateTime? vehicleEndDate, int modifiedByEmployeeId, bool setEndDate)
        {
            previousVehicle.active = false;
            previousVehicle.enddate = setEndDate ? vehicleEndDate : null;      
            previousVehicle.modifiedby = modifiedByEmployeeId;
            previousVehicle.modifiedon = DateTime.UtcNow;

            return this.SaveCar(previousVehicle);
        }

        private void UpdateMileageCats(int carid, List<int> mileagecats, cCurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (IDBConnection expdata = connection ?? new DatabaseConnection(sConnectionString))
            {
                expdata.AddWithValue("@carID", carid);
                expdata.AddWithValue("@carJourneyRates", mileagecats);

                if (currentUser != null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.ExecuteProc("saveCarVehicleJourneyRates");
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Returns the car from the list of cars if it exists
        /// </summary>
        /// <param name="carID">Car ID to search for</param>
        /// <returns>Car details</returns>
        public cCar GetCarByID(int carID)
        {
            if (lstCars != null && lstCars.Count > 0)
            {
                foreach (cCar car in lstCars)
                {
                    if (car.carid == carID)
                    {
                        return car;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Delete a car
        /// </summary>
        /// <param name="carID">Car ID</param>
        public abstract CarReturnVal DeleteCar(int carID);

        /// <summary>
        /// Delete a car by id
        /// </summary>
        /// <param name="carid"></param>
        /// <param name="employeeID">Optional EmployeeID if CurrentUser is not available</param>
        protected CarReturnVal DeleteCarFromDB(int carid, cCurrentUserBase currentUser, int employeeID = 0)
        {
            DBConnection expdata = new DBConnection(sConnectionString);
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            if (currentUser != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeID);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            if (currentUser != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            }

            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;

            expdata.ExecuteProc("deleteCar");
            CarReturnVal returnValue = (CarReturnVal)expdata.sqlexecute.Parameters["@returnvalue"].Value;

            expdata.sqlexecute.Parameters.Clear();

            return returnValue;
        }


        /// <summary>
        /// Delete all users associated to a pool car ID
        /// </summary>
        /// <param name="carID">Pool Car ID</param>
        public void DeletePoolCarUsersFromCarID(int carID)
        {
            DBConnection expdata = new DBConnection(sConnectionString);

            string sSQL = "DELETE FROM pool_car_users WHERE carid = @carid";

            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            expdata.ExecuteSQL(sSQL);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Add an employee pool car allocation
        /// </summary>
        /// <param name="carid">Car ID</param>
        /// <param name="employeeid">Employee ID</param>
        public void AddPoolCarUserToDB(int carID, int employeeID, cCurrentUserBase currentUser)
        {
            DBConnection expdata = new DBConnection(sConnectionString);

            if (currentUser != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeID);
            expdata.ExecuteProc("addPoolCarUser");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Remove an employee id from the specified pool car id
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="employeeID"></param>
        public void DeletePoolCarUserFromDB(int carID, int employeeID, cCurrentUserBase currentUser)
        {
            DBConnection data = new DBConnection(sConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@carid", carID);
            data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);

            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            }
            data.ExecuteProc("deleteUserFromPoolCar");
            data.sqlexecute.Parameters.Clear();
        }

        [Obsolete]
        public void UploadCarDocument(int employeeid, int carid, AttachDocumentType documentType, string filename)
        {
            DBConnection expdata = new DBConnection(sConnectionString);
            string strsql = "insert into car_documents (employeeid, carid, documenttype, filename) " +
                "values (@employeeid, @carid, @documenttype, @filename)";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            if (carid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@documenttype", (byte)documentType);
            expdata.AddWithValue("@filename", filename, 500);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        [Obsolete]
        public void DeleteCarDocument(int employeeid, int carid, AttachDocumentType documentType)
        {
            DBConnection expdata = new DBConnection(sConnectionString);
            string strsql = "delete from car_documents where employeeid = @employeeid and documenttype = @documenttype";
            if (carid != 0)
            {
                strsql += " and carid = @carid";
                expdata.sqlexecute.Parameters.AddWithValue("@carid", carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

            expdata.sqlexecute.Parameters.AddWithValue("@documenttype", (byte)documentType);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Add car engine types to drop down list.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="dropDownList">
        /// The drop down list.
        /// </param>
        public static void AddCarEngineTypesToDropDownList(ICurrentUserBase currentUser, ref DropDownList dropDownList)
        {
            dropDownList.Items.Clear();
            dropDownList.Items.Add(new ListItem("[None]", "0"));
            foreach (VehicleEngineType value in VehicleEngineType.GetAll(currentUser))
            {
                dropDownList.Items.Add(new ListItem(value.Name, value.VehicleEngineTypeId.ToString()));
            }
        }

        /// <summary>
        /// Add vehicle types to drop down list.
        /// </summary>
        /// <param name="dropDownList">
        /// The drop down list.
        /// </param>
        public static void AddVehicleTypesToDropDownList(ref DropDownList dropDownList)
        {
            var sorted = new SortedDictionary<string, int>();

            foreach (CarTypes.VehicleType value in Enum.GetValues(typeof(CarTypes.VehicleType)))
            {
                if (value != CarTypes.VehicleType.None)
                {
                    sorted.Add(Enum.GetName(typeof(CarTypes.VehicleType), value), (int)value);                    
                }
            }

            dropDownList.Items.Add(new ListItem("[None]", "0"));

            foreach (KeyValuePair<string, int> keyValuePair in sorted)
            {
                dropDownList.Items.Add(new ListItem(keyValuePair.Key, keyValuePair.Value.ToString()));
            }

        }

        public static string GetEnumDescription(Enum EnumConstant)
        {
            FieldInfo fi = EnumConstant.GetType().GetField(EnumConstant.ToString());
            DescriptionAttribute[] attr = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attr.Length > 0)
            {
                return attr[0].Description;
            }
            else
            {
                return EnumConstant.ToString();
            }
        }

        /// <summary>
        /// The activate car.
        /// </summary>
        /// <param name="carId">
        /// The car id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ActivateCar(int carId)
        {
            var expdata = new DBConnection(this.sConnectionString);
            const string Strsql = "UPDATE cars SET active = 1, approved = 1 WHERE carid = @carid";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carId);
            return expdata.ExecuteSQLWithAffectedRows(Strsql) == 1;
        }

        /// <summary>
        /// Gets a collection of lists of mileage categories for a list of cars
        /// </summary>
        /// <param name="carIds">The cars to fetch mileage categories for</param>
        /// <returns>The collection of lists of mileage categories sorted by carid</returns>
        private SortedList<int, List<int>> GetCarMileageCats(List<int> carIds)
        {
            using (IDBConnection expdata = new DatabaseConnection(sConnectionString))
            {
                var lstMileageCats = new SortedList<int, List<int>>();

                const string SQL = "SELECT [cm].[carid], [cm].[mileageid] FROM dbo.[car_mileagecats] [cm] INNER JOIN [dbo].[mileage_categories] [mc] ON [mc].[mileageid] = [cm].[mileageid] WHERE [cm].[carid] IN (SELECT [c1] FROM @carid) AND [mc].[catvalid] = @catvalid";

                expdata.AddWithValue("@carid", carIds);
                expdata.AddWithValue("@catvalid", true);

                using (IDataReader reader = expdata.GetReader(SQL))
                {
                    expdata.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        if (lstMileageCats.ContainsKey(reader.GetInt32(0)))
                        {
                            lstMileageCats[reader.GetInt32(0)].Add(reader.GetInt32(1));
                        }
                        else
                        {
                            lstMileageCats.Add(reader.GetInt32(0), new List<int> { reader.GetInt32(1) });
                        }
                    }

                    reader.Close();
                }

                return lstMileageCats;
            }
        }

        /// <summary>
        /// Get the odometer readings for a collection of cars
        /// </summary>
        /// <param name="carIds">The cars to get odo readings for</param>
        /// <param name="connection">A connection override for testing</param>
        /// <returns>The collection of lists of odometer readings for the cars requested sorted by carid</returns>
        private SortedList<int, List<cOdometerReading>> GetOdometerReadings(List<int> carIds, IDBConnection connection = null)
        {
            SortedList<int, List<cOdometerReading>> readings = new SortedList<int, List<cOdometerReading>>();

            using (IDBConnection expdata = connection ?? new DatabaseConnection(sConnectionString))
            {
                const string SQL = "SELECT [carid], [odometerid], [datestamp], [oldreading], [newreading], [createdon], [createdby] FROM [dbo].[odometer_readings] WHERE [carid] IN (SELECT [c1] FROM @carid)";

                expdata.AddWithValue("@carid", carIds);

                using (IDataReader reader = expdata.GetReader(SQL))
                {
                    int carIdOrd = reader.GetOrdinal("carid");
                    int odometerIdOrd = reader.GetOrdinal("odometerid");
                    int dateStampOrd = reader.GetOrdinal("datestamp");
                    int oldOdometerOrd = reader.GetOrdinal("oldreading");
                    int newOdometerOrd = reader.GetOrdinal("newreading");
                    int createdOnOrd = reader.GetOrdinal("createdon");
                    int createdByOrd = reader.GetOrdinal("createdby");

                    while (reader.Read())
                    {
                        int carId = reader.GetInt32(carIdOrd);
                        int odometerid = reader.GetInt32(odometerIdOrd);
                        DateTime datestamp = reader.GetDateTime(dateStampOrd);
                        int oldodometer = reader.GetInt32(oldOdometerOrd);
                        int newodometer = reader.IsDBNull(newOdometerOrd) ? 0 : reader.GetInt32(newOdometerOrd);
                        DateTime createdon = reader.IsDBNull(createdOnOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdOnOrd);
                        int createdby = reader.IsDBNull(createdByOrd) ? 0 : reader.GetInt32(createdByOrd);

                        if (readings.ContainsKey(carId))
                        {
                            readings[carId].Add(new cOdometerReading(odometerid, carId, datestamp, oldodometer, newodometer, createdon, createdby));
                        }
                        else
                        {
                            readings.Add(carId, new List<cOdometerReading> { new cOdometerReading(odometerid, carId, datestamp, oldodometer, newodometer, createdon, createdby) });
                        }
                    }

                    reader.Close();
                }

                expdata.sqlexecute.Parameters.Clear();

                return readings;
            }
        }

        public void SaveUserDefinedFieldsValues(int carId, SortedList<int, object> userdefined, ICurrentUserBase user, string carRegistration)
        {
            var tables = new cTables(this.nAccountID);
            var tbl = tables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
            clsUserDefinedFields.SaveValues(tables.GetTableByID(tbl.UserDefinedTableID), carId, userdefined, tables, clsFields, user, elementId: (int)SpendManagementElement.Cars, record: carRegistration);
        }

        /// <summary>
        /// Get the list of SORN declared cars for an employee
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="employeeId"></param>
        /// <param name="expenseItemDate"></param>
        /// <returns>List of carid and registration number which are SORN Declared</returns>
        public List<ListItem> GetSORNDeclaredCars(int accountId, int employeeId, DateTime expenseItemDate)
        {
            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                List<ListItem> sornCars = new List<ListItem>();

                connection.sqlexecute.Parameters.AddWithValue("@EmployeeId", employeeId);
                connection.sqlexecute.Parameters.AddWithValue("@expenseItemDate", Convert.ToDateTime(expenseItemDate));
                using (IDataReader reader = connection.GetReader("GetCarsWithSornDeclaration", CommandType.StoredProcedure))
                {
                    int carIdOrd;
                    int carRegNoOrd;
                    while (reader.Read())
                    {
                        carIdOrd = reader.GetOrdinal("CarId");
                        carRegNoOrd = reader.GetOrdinal("registration");
                        sornCars.Add(new ListItem(reader.GetInt32(carIdOrd).ToString(CultureInfo.InvariantCulture), reader.GetString(carRegNoOrd)));
                    }
                    return sornCars;
                }
            }
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
        public abstract void NotifyAdminOfNewVehicle(int employeeId, ICurrentUserBase user, cAccountProperties accountProperties, int vehicleId);
    }
}

