using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Microsoft.SqlServer.Server;

namespace SpendManagementLibrary
{
    using System.Data;
    using System.Linq;

    using Enumerators;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using System.Reflection;
    using System.ComponentModel;
    using System.Globalization;

    public abstract class cCarsBase
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
        protected List<cCar> GetEmployeeCarsCollection(int employeeID, IDBConnection connection = null)
        {
            using (IDBConnection expdata = connection ?? new DatabaseConnection(sConnectionString))
            {
                var cars = new SortedList<int, cCar>();
                var ids = new List<int>();

                cTable tbl = clsTables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
                cTable udftbl = clsTables.GetTableByID(tbl.UserDefinedTableID);

                const string SQL = "SELECT employeeid, carid, make, model, registration, startdate, enddate, cartypeid, odometer, active, fuelcard, endodometer, createdon, createdby, modifiedon, modifiedby, default_unit, enginesize, approved, exemptfromhometooffice, vehicletypeid FROM dbo.cars WHERE employeeid = @employeeid OR carid IN (SELECT carid FROM dbo.pool_car_users WHERE employeeid = @employeeid)";

                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);

                using (IDataReader reader = expdata.GetReader(SQL))
                {
                    while (reader.Read())
                    {
                        int caremployeeid = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        int carid = reader.GetInt32(1);
                        string make = reader.GetString(2);
                        string model = reader.GetString(3);
                        string registration = reader.GetString(4);
                        DateTime startdate = reader.IsDBNull(5) ? new DateTime(1900, 01, 01) : reader.GetDateTime(5);
                        DateTime enddate = reader.IsDBNull(6) ? new DateTime(1900, 01, 01) : reader.GetDateTime(6);
                        int vehicleEngineTypeId = reader.IsDBNull(7) ? 0 : reader.GetByte(7);
                        Int64 odometer = reader.IsDBNull(8) ? 0 : reader.GetInt64(8);
                        bool active = reader.GetBoolean(9);
                        bool fuelcard = reader.GetBoolean(10);
                        int endodometer = reader.IsDBNull(11) ? 0 : reader.GetInt32(11);
                        DateTime createdon = reader.IsDBNull(12) ? new DateTime(1900, 01, 01) : reader.GetDateTime(12);
                        int createdby = reader.IsDBNull(13) ? 0 : reader.GetInt32(13);
                        DateTime modifiedon = reader.IsDBNull(14) ? new DateTime(1900, 01, 01) : reader.GetDateTime(14);
                        int modifiedby = reader.IsDBNull(15) ? 0 : reader.GetInt32(15);
                        MileageUOM defaultuom = (MileageUOM)reader.GetByte(16);
                        int engineSize = reader.IsDBNull(17) ? 0 : reader.GetInt32(17);
                        bool approved = reader.IsDBNull(18) || reader.GetBoolean(18);
                        bool exemptfromhometooffice = reader.GetBoolean(19);
                        byte vehicletypeid = reader.IsDBNull(20) ? (byte)0 : reader.GetByte(20);

                        ids.Add(carid);
                        cars.Add(carid, new cCar(nAccountID, caremployeeid, carid, make, model, registration, startdate, enddate, active, new List<int>(), vehicleEngineTypeId, odometer, fuelcard, endodometer, defaultuom, engineSize, createdon, createdby, modifiedon, modifiedby, approved, exemptfromhometooffice, vehicletypeid));
                    }

                    reader.Close();
                }

                if (ids.Count > 0)
                {
                    SortedList<int, SortedList<int, object>> userdefined = clsUserDefinedFields.GetRecords(udftbl, ids, clsTables, clsFields);
                    SortedList<int, List<int>> carMileageCategories = GetCarMileageCats(ids);
                    SortedList<int, List<cOdometerReading>> odometerReadings = GetOdometerReadings(ids);

                    foreach (int id in cars.Keys)
                    {
                        if (carMileageCategories.ContainsKey(id))
                        {
                            cars[id].mileagecats = carMileageCategories[id];
                        }
                    }
                }

                return cars.Values.ToList();
            }
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
            var expdata = new DBConnection(sConnectionString);
            string strsql;
            SqlDataReader reader;
            SortedList<int, object> userdefined;
            int carid;
            string make, model, registration;
            DateTime startdate, enddate;
            bool active;
            int endodometer;
            Int64 odometer;
            bool fuelcard;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            List<cCar> cars = new List<cCar>();
            int engineSize = 0;
            bool approved, exemptfromhometooffice;
            byte vehicletypeid;

            cTable tbl = clsTables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
            cTable udftbl = clsTables.GetTableByID(tbl.UserDefinedTableID);
            strsql = "SELECT * FROM cars WHERE employeeid IS NULL";

            SortedList<AttachDocumentType, string> doctypes;

            MileageUOM defaultuom;

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    carid = reader.GetInt32(reader.GetOrdinal("carid"));
                    make = reader.GetString(reader.GetOrdinal("make"));
                    model = reader.GetString(reader.GetOrdinal("model"));
                    registration = reader.GetString(reader.GetOrdinal("registration"));
                    startdate = reader.IsDBNull(reader.GetOrdinal("startdate")) == false ? reader.GetDateTime(reader.GetOrdinal("startdate")) : new DateTime(1900, 01, 01);
                    enddate = reader.IsDBNull(reader.GetOrdinal("enddate")) == false ? reader.GetDateTime(reader.GetOrdinal("enddate")) : new DateTime(1900, 01, 01);
                    var vehicleEngineTypeId = reader.IsDBNull(reader.GetOrdinal("cartypeid")) == false ? reader.GetByte(reader.GetOrdinal("cartypeid")) : 0;
                    odometer = reader.IsDBNull(reader.GetOrdinal("odometer")) == false ? reader.GetInt64(reader.GetOrdinal("odometer")) : 0;
                    active = reader.GetBoolean(reader.GetOrdinal("active"));
                    fuelcard = reader.GetBoolean(reader.GetOrdinal("fuelcard"));
                    endodometer = reader.IsDBNull(reader.GetOrdinal("endodometer")) == false ? reader.GetInt32(reader.GetOrdinal("endodometer")) : 0; 
                    createdon = reader.IsDBNull(reader.GetOrdinal("createdon")) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                    createdby = reader.IsDBNull(reader.GetOrdinal("createdby")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));
                    modifiedon = reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    modifiedby = reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    userdefined = clsUserDefinedFields.GetRecord(udftbl, carid, clsTables, clsFields);
                    defaultuom = (MileageUOM)reader.GetByte(reader.GetOrdinal("default_unit"));

                    if (!reader.IsDBNull(reader.GetOrdinal("enginesize")))
                    {
                        engineSize = reader.GetInt32(reader.GetOrdinal("enginesize"));
                    }

                    approved = reader.IsDBNull(reader.GetOrdinal("approved")) || reader.GetBoolean(reader.GetOrdinal("approved"));
                    exemptfromhometooffice = reader.GetBoolean(reader.GetOrdinal("exemptfromhometooffice"));
                    vehicletypeid = !reader.IsDBNull(reader.GetOrdinal("VehicleTypeId")) ? reader.GetByte(reader.GetOrdinal("VehicleTypeId")) : (byte)0;

                    cars.Add(new cCar(nAccountID, 0, carid, make, model, registration, startdate, enddate, active, GetCarMileageCats(carid), vehicleEngineTypeId, odometer, fuelcard, endodometer, defaultuom, engineSize, createdon, createdby, modifiedon, modifiedby, approved, exemptfromhometooffice, vehicletypeid));
                }
                reader.Close();
            }

            return cars;
        }

        /// <summary>
        /// Find an individual car in the database from its ID
        /// </summary>
        /// <param name="carid">the car id to find</param>
        /// <param name="ensureEmployeeIsPopulated">Ensure whether the EmployeeId is populated. Note that this has been added as a fix for the API.</param>
        /// <returns>Car details</returns>
        public cCar GetCarFromDB(int carID, bool ensureEmployeeIsPopulated = false)
        {
            DBConnection expdata = new DBConnection(sConnectionString);
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;
            SortedList<int, object> userdefined;

            string make, model, registration;
            DateTime? startdate, enddate;
            bool active;
            int endodometer;
            Int64 odometer;
            bool fuelcard;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            cCar car = null;
            SortedList<AttachDocumentType, string> doctypes;
            MileageUOM defaultuom;
            int enginesize = 0;
            bool approved, exemptfromhometooffice;
            int employeeId = 0;
            int employeeOrdinal;
            byte vehicletypeid;

            cTable tbl = clsTables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
            cTable udftbl = clsTables.GetTableByID(tbl.UserDefinedTableID);

            strsql = "select * from cars where carid = @carid;";
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    make = reader.GetString(reader.GetOrdinal("make"));
                    model = reader.GetString(reader.GetOrdinal("model"));
                    registration = reader.GetString(reader.GetOrdinal("registration"));

                    startdate = reader.IsDBNull(reader.GetOrdinal("startdate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime(reader.GetOrdinal("startdate"));

                    enddate = reader.IsDBNull(reader.GetOrdinal("enddate"))
                            ? (DateTime?)null
                            : reader.GetDateTime(reader.GetOrdinal("enddate"));

                    var vehicleEngineTypeId = reader.IsDBNull(reader.GetOrdinal("cartypeid")) == false ? reader.GetByte(reader.GetOrdinal("cartypeid")) : 0;
                    odometer = reader.IsDBNull(reader.GetOrdinal("odometer")) == false ? reader.GetInt64(reader.GetOrdinal("odometer")) : 0;
                    active = reader.GetBoolean(reader.GetOrdinal("active"));
                    fuelcard = reader.GetBoolean(reader.GetOrdinal("fuelcard"));
                    endodometer = reader.IsDBNull(reader.GetOrdinal("endodometer")) == false ? reader.GetInt32(reader.GetOrdinal("endodometer")) : 0;
                    createdon = reader.IsDBNull(reader.GetOrdinal("createdon")) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                    createdby = reader.IsDBNull(reader.GetOrdinal("createdby")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));
                    modifiedon = reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    modifiedby = reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    userdefined = clsUserDefinedFields.GetRecord(udftbl, carID, clsTables, clsFields);
                    defaultuom = (MileageUOM)reader.GetByte(reader.GetOrdinal("default_unit"));
                    if (!reader.IsDBNull(reader.GetOrdinal("enginesize")))
                    {
                        enginesize = reader.GetInt32(reader.GetOrdinal("enginesize"));
                    }

                    approved = reader.IsDBNull(reader.GetOrdinal("approved")) || reader.GetBoolean(reader.GetOrdinal("approved"));
                    exemptfromhometooffice = reader.GetBoolean(reader.GetOrdinal("exemptfromhometooffice"));
                    vehicletypeid = reader.IsDBNull(reader.GetOrdinal("vehicletypeid")) == false ? reader.GetByte(reader.GetOrdinal("vehicletypeid")) : (byte)0;

                    employeeOrdinal = reader.GetOrdinal("employeeid");
                    if (!reader.IsDBNull(employeeOrdinal))
                    {
                        employeeId = ensureEmployeeIsPopulated ? reader.GetInt32(employeeOrdinal) : 0;
                    }

                    car = new cCar(nAccountID, employeeId, carID, make, model, registration, startdate, enddate, active, GetCarMileageCats(carID), vehicleEngineTypeId, odometer, fuelcard, endodometer, defaultuom, enginesize, createdon, createdby, modifiedon, modifiedby, approved, exemptfromhometooffice, vehicletypeid);

                }
                reader.Close();
            }

            return car;
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
        /// Save a car object
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
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

