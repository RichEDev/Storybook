namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Cars
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Tools;

    #region car object
    public class Cars
    {
        internal int? employeeID;
        internal int carID;

        internal string make;
        internal string model;
        internal string regNumber;
        internal byte unitOfMeasure;
        internal byte carType;
        internal int engineSize;
        internal bool active;
        internal bool exemptFromHomeToOffice;
        internal DateTime? startDate;
        internal DateTime? endDate;

        internal DateTime? taxExpiry;
        internal DateTime? taxLastChecked;
        internal int? taxCheckedBy;
        internal string taxPath = "";

        internal string motTestNumber;
        internal DateTime? motExpiry;
        internal DateTime? motLastChecked;
        internal int? motCheckedBy;
        internal string motPath = "";

        internal string insuranceNumber;
        internal DateTime? insuranceExpiry;
        internal int? insuranceCheckedBy;
        internal DateTime? insuranceLastChecked;
        internal string insurancePath = "";

        internal DateTime? serviceExpiry;
        internal DateTime? serviceLastChecked;
        internal int? serviceCheckedby;
        internal string servicePath = "";


        internal int? odometer;
        internal bool fuelCard;
        internal int? endOdometer;

        internal int modifiedBy;
        internal int createdBy;
        internal bool approved;

        internal int? nTaxAttachID;
        internal int? nMOTAttachID;
        internal int? nInsuranceAttachID;
        internal int? nServiceAttachID;

        internal byte? vehicletypeid;

        public static string SqlItem = "SELECT * FROM cars where employeeid = @employeeid";

        /// <summary>
        /// Gets the car grid values.
        /// </summary>
        internal List<string> CarGridValues
        {
            get
            {
                return new List<string>
                           {
                               this.make,
                               this.model,
                               this.regNumber,
                               this.startDate.Value.ToShortDateString(),
                               this.endDate.Value.ToShortDateString(),
                               EnumHelper.GetEnumDescription((EngineType)this.carType),
                               EnumHelper.GetEnumDescription((VehicleType)this.vehicletypeid),
                               this.active.ToString()
                           };
            }
        }

        internal string CarIdentity
        {
            get
            {
                return string.Format("{0} {1} ({2})", this.make, this.model, this.regNumber);
            }
        }

        public Cars()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Cars"/> class.
        /// </summary>
        /// <param name="carID">
        /// The car id.
        /// </param>
        /// <param name="make">
        /// The make.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="regNumber">
        /// The reg number.
        /// </param>
        /// <param name="carType">
        /// The car type.
        /// </param>
        /// <param name="engineSize">
        /// The engine size.
        /// </param>
        /// <param name="vehicleTypeId">
        /// The vehicle type id.
        /// </param>
        public Cars(int carID, string make, string model, string regNumber, byte carType, int engineSize, byte? vehicleTypeId)
        {
            this.carID = carID;
            this.make = make;
            this.model = model;
            this.regNumber = regNumber;
            this.carType = carType;
            this.engineSize = engineSize;
            this.vehicletypeid = vehicleTypeId;
        }
        //internal cCar(int accountid, int employeeid, int carid, string make, string model, string regno, DateTime? startdate, DateTime? enddate, bool active, List<int> mileagecats, byte cartypeid, Int64 odometer, bool fuelcard, int endodometer, DateTime? taxexpiry, DateTime? taxlastchecked, int taxcheckedby, string mottestnumber, DateTime? motexpiry, DateTime? motlastchecked, int motcheckedby, string insurancenumber, DateTime? insuranceexpiry, DateTime? insurancelastchecked, int insurancecheckedby, DateTime? serviceexpiry, DateTime? servicelastchecked, int servicecheckedby, SortedList<int, object> userdefined, cOdometerReading[] odometerreadings, string insurancepath, string motpath, string servicepath, string taxpath, MileageUOM defaultuom, int engineSize, DateTime? createdon, int createdby, DateTime? modifiedon, int? modifiedby, bool approved, bool exemptfromhometooffice, int? TaxAttachID, int? MOTAttachID, int? InsuranceAttachID, int? ServiceAttachID)
        //{
        //    nAccountid = accountid;
        //    nEmployeeid = employeeid;
        //    nCarid = carid;
        //    sMake = make;
        //    sModel = model;
        //    sRegno = regno;
        //    dtStartdate = startdate;
        //    dtEnddate = enddate;
        //    bActive = active;
        //    bCartypeid = cartypeid;
        //    nOdometer = odometer;
        //    bFuelcard = fuelcard;
        //    nEndodometer = endodometer;
        //    odoreadings = odometerreadings;
        //    dtTaxExpiry = taxexpiry;
        //    dtTaxLastchecked = taxlastchecked;
        //    nTaxCheckedby = taxcheckedby;
        //    sMotTestNumber = mottestnumber;
        //    dtMotExpiry = motexpiry;
        //    dtMotLastchecked = motlastchecked;
        //    nMotCheckedby = motcheckedby;
        //    sInsurancenumber = insurancenumber;
        //    dtInsuranceExpiry = insuranceexpiry;
        //    dtInsuranceLastchecked = insurancelastchecked;
        //    nInsuranceCheckedby = insurancecheckedby;
        //    dtServiceExpiry = serviceexpiry;
        //    dtServiceLastchecked = servicelastchecked;
        //    serviceCheckedby = servicecheckedby;

        //    lstUserdefined = userdefined;
        //    insurancePath = insurancepath;
        //    sMotpath = motpath;
        //    servicePath = servicepath;
        //    sTaxpath = taxpath;
        //    dtCreatedon = createdon;
        //    createdBy = createdby;
        //    dtModifiedon = modifiedon;
        //    nModifiedby = modifiedby;
        //    lstMileagecats = mileagecats;
        //    eDefaultUom = defaultuom;
        //    nEngineSize = engineSize;
        //    bApproved = approved;
        //    bExemptFromHomeToOffice = exemptfromhometooffice;
        //    nTaxAttachID = TaxAttachID;
        //    nMOTAttachID = MOTAttachID;
        //    nInsuranceAttachID = InsuranceAttachID;
        //    nServiceAttachID = ServiceAttachID;
        //}    
    }
    #endregion

    internal enum UnitOfMeasure
    {

        [Description("Miles")]
        Miles = 0,
        [Description("Kilometres")]
        Kilometers = 1
    }

    internal enum EngineType
    {
        [Description("Petrol")]
        Petrol = 1,
        [Description("Diesel")]
        Diesel = 2,
        [Description("LPG")]
        LPG = 3
    }

    /// <summary>
    /// The vehicle type.
    /// </summary>
    public enum VehicleType
    {
        /// <summary>
        /// No vehicle type set.
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// The bicycle.
        /// </summary>
        [Description("Bicycle")]
        Bicycle = 1,

        /// <summary>
        /// The car.
        /// </summary>
        [Description("Car")]
        Car = 2,

        /// <summary>
        /// The motorcycle.
        /// </summary>
        [Description("Motorcycle")]
        Motorcycle = 3,

        /// <summary>
        /// The moped.
        /// </summary>
        [Description("Moped")]
        Moped = 4,

        /// <summary>
        /// LGV
        /// </summary>
        [Description("Light Goods Vehicle (LGV)")]
        LGV = 5,

        /// <summary>
        /// HGV
        /// </summary>
        [Description("Heavy Goods Vehicle (HGV)")]
        HGV = 6,

        /// <summary>
        /// Minibus
        /// </summary>
        [Description("Minibus")]
        Minibus = 7,

        /// <summary>
        /// Bus
        /// </summary>
        [Description("Bus")]
        Bus = 8
    }

}
