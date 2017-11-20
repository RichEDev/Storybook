namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The car.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Car, TableId = "A184192F-74B6-42F7-8FDB-6DCF04723CEF")]
    public class Car : DataClassBase
    {
        /// <summary>
        /// The car id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 0, FieldId = "DF695804-D4D7-4552-A396-9A5EF81491D3")]
        [DataMember]
        public int carid = 0;

        /// <summary>
        /// The employee id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "5DDBF0EF-FA06-4E7C-A45A-54E50E33307E")]
        public int? employeeid;

        /// <summary>
        /// The start date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "D226C1BD-ECC3-4F37-A5FE-58638B1BD66C")]
        public DateTime? startdate;

        /// <summary>
        /// The end date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "2AB21296-77EE-4B3D-807C-56EDF936613D")]
        public DateTime? enddate;

        /// <summary>
        /// The make.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "B7961F43-E439-4835-9709-396FFF9BBD0C")]
        public string make = string.Empty;

        /// <summary>
        /// The model.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "99A078D9-F82C-4474-BDDE-6701D4BD51EA")]
        public string model = string.Empty;

        /// <summary>
        /// The registration.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "156CCCA7-1F5C-45BE-920C-5E5C199EE81A")]
        public string registration = string.Empty;

        /// <summary>
        /// The mileage id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int mileageid = 0;

        /// <summary>
        /// The cartypeid.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "24172542-3E15-4FCA-B4F5-D7FFEF9EED4E")]
        public byte? cartypeid = 0;

        /// <summary>
        /// The active.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "DAD8F087-497B-4A83-AB40-6B5B816911EB")]
        public bool active = false;

        /// <summary>
        /// The odometer.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public Int64? odometer;

        /// <summary>
        /// The fuelcard.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "9D3081C9-789D-49DF-8764-1A3D4F32AE29")]
        public bool fuelcard = false;

        /// <summary>
        /// The endodometer.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "CF725092-D2F0-48B0-A359-D8149750DE81")]
        public int? endodometer;

        /// <summary>
        /// The taxexpiry.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "3E49160F-349A-466F-B9BF-6F1015B8415A")]
        public DateTime? taxexpiry;

        /// <summary>
        /// The taxlastchecked.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "22F22E47-7756-4CEB-B2CE-9D7008BE8E6A")]
        public DateTime? taxlastchecked;

        /// <summary>
        /// The taxcheckedby.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? taxcheckedby;

        /// <summary>
        /// The mottestnumber.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "77A9770F-12C9-4966-98CE-6300CE5FBB57")]
        public string mottestnumber = string.Empty;

        /// <summary>
        /// The motlastchecked.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "B1C98355-AE47-4BD1-B4A8-71622ACC1B2F")]
        public DateTime? motlastchecked;

        /// <summary>
        /// The motcheckedby.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? motcheckedby;

        /// <summary>
        /// The motexpiry.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "EEDFB5E6-848B-4C5F-9CB9-20E20ACE0F7D")]
        public DateTime? motexpiry;

        /// <summary>
        /// The insurancenumber.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "5D90D3A4-40DA-403B-B417-2AC445393A37")]
        public string insurancenumber = string.Empty;

        /// <summary>
        /// The insuranceexpiry.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "BAB44A26-5690-4F3E-8003-206EE3FB673F")]
        public DateTime? insuranceexpiry;

        /// <summary>
        /// The insurancelastchecked.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "7C8B48F2-6299-400C-85C0-D6584232CDDF")]
        public DateTime? insurancelastchecked;

        /// <summary>
        /// The insurancecheckedby.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? insurancecheckedby;

        /// <summary>
        /// The serviceexpiry.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "A380A705-C14B-4A07-A510-2F1A34FADA98")]
        public DateTime? serviceexpiry;

        /// <summary>
        /// The servicelastchecked.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "766682AF-1FDE-43F5-941D-8C5FCAD1C7DB")]
        public DateTime? servicelastchecked;

        /// <summary>
        /// The servicecheckedby.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? servicecheckedby;

        /// <summary>
        /// The created on.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "2383F2CD-D9B0-42B2-B2F2-ED323065203E")]
        public DateTime? CreatedOn;

        /// <summary>
        /// The created by.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? CreatedBy;

        /// <summary>
        /// The modified on.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "9A48D0DA-E8EF-4A75-A4D4-0EC531B3AC8E")]
        public DateTime? ModifiedOn;

        /// <summary>
        /// The modified by.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? ModifiedBy;

        /// <summary>
        /// The default_unit.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "F64AE656-CB64-4A9B-88FE-50D73538166A")]
        public byte default_unit;

        /// <summary>
        /// The enginesize.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "1F4E203C-1436-494E-9F53-9FF4FC6E2BE3")]
        public int? enginesize;

        /// <summary>
        /// The approved.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "81F4CBA4-C74D-449E-ACD4-3DDD44D97A9B")]
        public bool approved = false;

        /// <summary>
        /// The exempt from home to office.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public bool exemptFromHomeToOffice;

        /// <summary>
        /// The tax attach id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? taxAttachID;

        /// <summary>
        /// The mot attach id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? MOTAttachID;

        /// <summary>
        /// The insurance attach id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? insuranceAttachID;

        /// <summary>
        /// The service attach id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "")]
        public int? serviceAttachID;

        /// <summary>
        /// Gets or sets the vehicle allocations.
        /// </summary>
        [IgnoreDataMember]
        public List<CarAssignmentNumberAllocation> Allocations { get; set; }
    }
}
