using System;
using System.Collections.Generic;
using System.Linq;
using SpendManagementLibrary.Helpers;
using DOC = SpendManagementLibrary.Employees.DutyOfCare;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cCar : ICloneable
    {
        private int nAccountid;
        private int nEmployeeid;
        private int nCarid;
        private string sMake;
        private string sModel;
        private string sRegno;
        private DateTime? dtStartdate;
        private DateTime? dtEnddate;
        private bool bActive;
        private Int64 nOdometer;
        private bool bFuelcard;
        private int nEndodometer;
        private List<int> lstMileagecats;
        private MileageUOM eDefaultUom;
        private int nEngineSize;
        private DateTime? dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;
        private bool bApproved;
        private bool bExemptFromHomeToOffice;
        private byte? bVehicletypeid;
       

        public cCar()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="cCar"/>
        /// </summary>
        /// <param name="accountid">The ID of the account</param>
        /// <param name="employeeid">The ID of the Employee that uses the car (0 for a pool car)</param>
        /// <param name="carid">The ID of this <see cref="cCar"/></param>
        /// <param name="make">The Make of the Vehicle</param>
        /// <param name="model">The Model of the Vehicle</param>
        /// <param name="regno">The Registration number of the vehicle</param>
        /// <param name="startdate">The date the vehcle was available from</param>
        /// <param name="enddate">the date the vehicle stoped being available</param>
        /// <param name="active">Is the vehicle active</param>
        /// <param name="mileagecats">A <see cref="List{T}"/>Of <seealso cref="cMileageCat"/> IDs used for this vehicle</param>
        /// <param name="vehicleEngineTypeId">The Engine type of the Vehicle</param>
        /// <param name="odometer">Start Odometer reading</param>
        /// <param name="fuelcard">Does this vehicle use a fuel card</param>
        /// <param name="endodometer">End odometer reading</param>
        /// <param name="defaultuom">Unit of measure (miles / kilometers)</param>
        /// <param name="engineSize">The size of the engine in cc</param>
        /// <param name="createdon">Creation Date</param>
        /// <param name="createdby">Employee ID of the creator</param>
        /// <param name="modifiedon">Modified Date</param>
        /// <param name="modifiedby">Employee ID of the modifier</param>
        /// <param name="approved">Is this vehicle approved for use</param>
        /// <param name="exemptfromhometooffice">If this vehicle expempt from home to office rules.</param>
        /// <param name="VehicleTypeId">The type of vehicle (car, HGV etc)</param>
        /// <param name="taxExpiry">The Date the tax expires (if available)</param>
        /// <param name="isTaxValid">Is the tax valid (at time of creation)</param>
        /// <param name="motExpiry">The date the Mot Expires (if available)</param>
        /// <param name="isMotValid">If the Mot valid (at time of creation)</param>
        public cCar(int accountid, int employeeid, int carid, string make, string model, string regno, DateTime? startdate, DateTime? enddate, bool active, List<int> mileagecats, int vehicleEngineTypeId, Int64 odometer, bool fuelcard, int endodometer, MileageUOM defaultuom, int engineSize, DateTime? createdon, int createdby, DateTime? modifiedon, int? modifiedby, bool approved, bool exemptfromhometooffice, byte? VehicleTypeId, DateTime? taxExpiry, bool isTaxValid, DateTime? motExpiry, bool isMotValid )
        {
            this.nAccountid = accountid;
            this.nEmployeeid = employeeid;
            this.nCarid = carid;
            this.sMake = make;
            this.sModel = model;
            this.sRegno = regno;
            this.dtStartdate = startdate;
            this.dtEnddate = enddate;
            this.bActive = active;
            this.nOdometer = odometer;
            this.bFuelcard = fuelcard;
            this.nEndodometer = endodometer;
            this.dtCreatedon = createdon;
            this.nCreatedby = createdby;
            this.dtModifiedon = modifiedon;
            this.nModifiedby = modifiedby;
            this.lstMileagecats = mileagecats;
            this.eDefaultUom = defaultuom;
            this.nEngineSize = engineSize;
            this.bApproved = approved;
            this.bExemptFromHomeToOffice = exemptfromhometooffice;
            this.bVehicletypeid = VehicleTypeId;
            this.VehicleEngineTypeId = vehicleEngineTypeId;
            this.TaxExpiry = taxExpiry;
            this.IsTaxValid = isTaxValid;
            this.MotExpiry = motExpiry;
            this.IsMotValid = isMotValid;

        }

        /// <summary>Creates a new <see cref="cCar"/> that is a copy of the current instance.</summary>
        /// <returns>A new <see cref="cCar"/> that is a copy of this instance.</returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new cCar(this.accountid, this.employeeid, this.carid, this.make, this.model, this.registration,
                this.startdate, this.enddate, this.active, this.mileagecats, this.VehicleEngineTypeId, this.odometer,
                this.fuelcard, this.endodometer, this.defaultuom, this.EngineSize, this.createdon, this.createdby,
                this.modifiedon, this.modifiedby, this.Approved, this.ExemptFromHomeToOffice, this.VehicleTypeID,
                this.TaxExpiry, this.IsTaxValid, this.MotExpiry, this.IsMotValid);
        }

        //checks to see whether the car is active on the date specified
        public bool CarActiveOnDate(DateTime date)
        {
            DateTime noDateTime = new DateTime(1900, 01, 01);
            if ((startdate == noDateTime && enddate == noDateTime) || (startdate == noDateTime && enddate != noDateTime && date <= enddate) || (startdate != noDateTime && enddate == noDateTime && date >= startdate) || (startdate != noDateTime & enddate != noDateTime && startdate <= date && enddate >= date))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public cOdometerReading getLastOdometerReading()
        {
            return GetOdometerReadings(true).FirstOrDefault();
        }
        public List<cOdometerReading> GetAllOdometerReadings()
        {
            return GetOdometerReadings(false);
        }

        /// <summary>Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</returns>
        public override string ToString()
        {
            return $"{this.make} {this.model} {this.registration}";
        }

        /// <summary>
        /// Gets every odometer reading in the database.
        /// </summary>
        /// <returns>A list of Odometer readings.</returns>
        internal List<cOdometerReading> GetOdometerReadings(bool lastReading)
        {
            var readings = new List<cOdometerReading>();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@carid", this.carid);
                string Strsql = string.Empty;
                if (lastReading)
                {
                    Strsql = "SELECT TOP 1 odometerid, carid, datestamp, oldreading, newreading, createdon, createdby FROM odometer_readings WHERE carid = @carid ORDER BY datestamp DESC, newreading DESC";
                }
                else
                {
                    Strsql = "SELECT odometerid, carid, datestamp, oldreading, newreading, createdon, createdby FROM odometer_readings WHERE carid = @carid ORDER BY datestamp, newreading";
                }

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
            }
            return readings;
        }
        
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public int employeeid
        {
            get { return nEmployeeid; }
            set { nEmployeeid = value; }
        }
        public int carid
        {
            get { return nCarid; }
            set { nCarid = value; }
        }
        public string make
        {
            get { return sMake; }
        }
        public string model
        {
            get { return sModel; }
        }
        public string registration
        {
            get { return sRegno; }
        }
        public DateTime? startdate
        {
            get { return dtStartdate; }
        }
        public DateTime? enddate
        {
            get { return dtEnddate; }
            set { dtEnddate = value; }
        }
        public bool active
        {
            get { return bActive; }
            set { bActive = value; }
        }

        public int VehicleEngineTypeId { get; private set; }

        public Int64 odometer
        {
            get { return nOdometer; }
            set { nOdometer = value; }
        }
        public bool fuelcard
        {
            get { return bFuelcard; }
        }
        public int endodometer
        {
            get { return nEndodometer; }
        }

        public List<int> mileagecats
        {
            get { return lstMileagecats; }
            set { lstMileagecats = value; }
        }
        public DateTime? createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedon; }
            set { dtModifiedon = value; }
        }
        public int? modifiedby
        {
            get { return nModifiedby; }
            set { nModifiedby = value; }
        }
        public MileageUOM defaultuom
        {
            get { return eDefaultUom; }
        }
        public int EngineSize
        {
            get { return nEngineSize; }
        }

        public bool Approved
        {
            get { return bApproved; }
            set { bApproved = value; }
        }

        /// <summary>
        /// Gets the vehicle type id.
        /// </summary>
        public byte? VehicleTypeID 
        {
            get { return this.bVehicletypeid; }
        }

        /// <summary>
        /// Gets whether the vehicle is exempt from the home to office mileage calculation if active on the expense item
        /// </summary>
        public bool ExemptFromHomeToOffice
        {
            get { return bExemptFromHomeToOffice; }
        }

        /// <summary>
        /// Gets or sets the Tax Expiry of this vehicle
        /// </summary>
        public DateTime? TaxExpiry { get; set; }

        /// <summary>
        /// Gets or sets the tax status of this vehicle
        /// </summary>
        public bool IsTaxValid { get;set; }

        /// <summary>
        /// Gets or sets the MOT Expiry of this vehicle
        /// </summary>
        public DateTime? MotExpiry { get; set; }

        /// <summary>
        /// Gets or sets the MOT status of this vehicle
        /// </summary>
        public bool IsMotValid { get;set; }
        #endregion
    }

}
