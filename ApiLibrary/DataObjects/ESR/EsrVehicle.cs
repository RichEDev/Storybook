namespace ApiLibrary.DataObjects.ESR
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The ESR cars Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Vehicle, TableId = "4A02E429-5073-4A8A-ADD9-2BB0C426D973")]
    public class EsrVehicle : DataClassBase
    {
        /// <summary>
        /// The ESR Vehicle Allocation id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 3, FieldId = "B7DB17EC-4341-41C2-9ABB-AB80877A0051")]
        [DataMember(IsRequired = true)]
        public long ESRVehicleAllocationId = 0;

        /// <summary>
        /// The ESR person id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 1, FieldId = "50678A74-7DF6-47E1-B304-9107DDDBCF8F")]
        public long ESRPersonId = 0;

        /// <summary>
        /// The ESR assignment id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 2, FieldId = "85172ED6-3F5E-4D40-958C-2A7F94FB1FFF")]
        public long ESRAssignmentId = 0;

        /// <summary>
        /// The effective start date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 4, FieldId = "F13D5AFD-D472-4EF7-BA17-1F2D7D26612B")]
        public DateTime? EffectiveStartDate = null;

        /// <summary>
        /// The effective end date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 5, FieldId = "387580DC-0479-4F1C-9421-A4755959CE4E")]
        public DateTime? EffectiveEndDate = null;

        /// <summary>
        /// The registration number.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "951ECAC0-F38B-44C0-A736-0656B45D06E0")]
        public string RegistrationNumber = string.Empty;

        /// <summary>
        /// The make.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "D6F4DBBE-DCDB-439F-89F9-4B3E278EEE33")]
        public string Make = string.Empty;

        /// <summary>
        /// The model.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "9AFF9B76-A6F1-421B-B4EF-2CE5B11FD9A6")]
        public string Model = string.Empty;

        /// <summary>
        /// The ownership.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "E8D2797A-6BD1-497F-8AB7-AD539C790DBD")]
        public string Ownership = string.Empty;

        /// <summary>
        /// The initial registration date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "2C610772-C186-46C7-95D5-6684749FD569")]
        public DateTime? InitialRegistrationDate = null;

        /// <summary>
        /// The engine CC.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "D8853485-CA03-4912-AC1D-E06BBA8E683F")]
        public int EngineCC = 0;

        /// <summary>
        /// The ESR last update.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "2883F81C-E8A5-462E-B5AE-BE1484703C1D")]
        public DateTime? ESRLastUpdate = null;

        /// <summary>
        /// The pay user table name
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "176A01F3-9D6B-4E86-B997-B057413F9233")]
        public string UserRatesTable = string.Empty;

        /// <summary>
        /// The vehicle fuel type
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "E45C6CA7-7256-4332-A1FB-A037EB5B1A6F")]
        public string FuelType = string.Empty;

        /// <summary>
        /// The ESR assign id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "052237AE-FD91-4314-B3BF-6FB1AA5BA3FC")]
        public int? ESRAssignID = 0;
    }
}