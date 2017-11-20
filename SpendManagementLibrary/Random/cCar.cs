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

        public cCar(int accountid, int employeeid, int carid, string make, string model, string regno, DateTime? startdate, DateTime? enddate, bool active, List<int> mileagecats, int vehicleEngineTypeId, Int64 odometer, bool fuelcard, int endodometer, MileageUOM defaultuom, int engineSize, DateTime? createdon, int createdby, DateTime? modifiedon, int? modifiedby, bool approved, bool exemptfromhometooffice, byte? VehicleTypeId )
        {
            nAccountid = accountid;
            nEmployeeid = employeeid;
            nCarid = carid;
            sMake = make;
            sModel = model;
            sRegno = regno;
            dtStartdate = startdate;
            dtEnddate = enddate;
            bActive = active;
            this.VehicleEngineTypeId = vehicleEngineTypeId;
            nOdometer = odometer;
            bFuelcard = fuelcard;
            nEndodometer = endodometer;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            lstMileagecats = mileagecats;
            eDefaultUom = defaultuom;
            nEngineSize = engineSize;
            bApproved = approved;
            bExemptFromHomeToOffice = exemptfromhometooffice;
            bVehicletypeid = VehicleTypeId;
        }

        public object Clone()
        {
            return new cCar(this.accountid, this.employeeid, this.carid, this.make, this.model, this.registration, this.startdate, this.enddate, this.active, this.mileagecats, this.VehicleEngineTypeId, this.odometer, this.fuelcard, this.endodometer, this.defaultuom, this.EngineSize, this.createdon, this.createdby, this.modifiedon, this.modifiedby, this.Approved, this.ExemptFromHomeToOffice, this.VehicleTypeID);
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
        #endregion
    }

}
