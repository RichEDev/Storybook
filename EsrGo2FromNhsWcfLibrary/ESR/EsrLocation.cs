namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The ESR location.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Location, TableId = "1F8A5A08-DEA3-4EE2-A612-C79B9FB4670B")]
    public class EsrLocation : DataClassBase
    {
        /// <summary>
        /// The ESR location id.
        /// </summary>
        [DataClass(IsKeyField = true, ColumnRef = 1, FieldId = "CA5BC3A8-6583-4EB3-AD94-004557017DBD")]
        [DataMember(IsRequired = true)]
        public long ESRLocationId = 0;

        /// <summary>
        /// The location code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 2, FieldId = "71841C00-FFD9-4410-BC4F-58593FFB7977")]
        public string LocationCode = string.Empty;

        /// <summary>
        /// The description.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 3, FieldId = "D62DB224-5C6C-4CC6-A8FD-930A1EB711F8")]
        public string Description = string.Empty;

        /// <summary>
        /// The inactive date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 4, FieldId = "10A90941-1DE1-48D1-A6AA-40E0D7A70DDE")]
        public DateTime? InactiveDate;

        /// <summary>
        /// The address line 1.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 5, FieldId = "90947D48-361F-486B-8E1A-24A2062C2F58")]
        public string AddressLine1 = string.Empty;

        /// <summary>
        /// The address line 2.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "4B919FBC-79DE-4B51-A25D-6FE15BB86542")]
        public string AddressLine2 = string.Empty;

        /// <summary>
        /// The address line 3.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "C70E8753-6D2D-4803-B934-A154FA78BD19")]
        public string AddressLine3 = string.Empty;

        /// <summary>
        /// The town.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "D67964AA-046E-4C6E-9993-D9F736BE5973")]
        public string Town = string.Empty;

        /// <summary>
        /// The county.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "D7E22FAA-5925-4DC8-ABDB-00A338BBFCE5")]
        public string County = string.Empty;

        /// <summary>
        /// The postcode.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "8BB3C636-EAA1-40A8-A36B-3DA140887602")]
        public string Postcode = string.Empty;

        /// <summary>
        /// The country.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "EEFAD055-E0B3-4961-B79C-62AAEC38B71C")]
        public string Country = string.Empty;

        /// <summary>
        /// The telephone.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "907987B5-A9B1-4AD4-A810-E957D3EF4A77")]
        public string Telephone = string.Empty;

        /// <summary>
        /// The fax.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "1AE6707F-1BF3-4DF1-B936-F2C6903EDC29")]
        public string Fax = string.Empty;

        /// <summary>
        /// The payslip delivery point.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "26B4B106-4648-4AEA-A3A0-08ED8470FB0A")]
        public string PayslipDeliveryPoint = string.Empty;

        /// <summary>
        /// The site code.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 15, FieldId = "B16330AF-670C-471A-BD9B-43C7713CC714")]
        public string SiteCode = string.Empty;

        /// <summary>
        /// The welsh location translation.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 16, FieldId = "404F17C8-537E-44F3-8B8F-AC34539E05C2")]
        public string WelshLocationTranslation = string.Empty;

        /// <summary>
        /// The welsh address 1.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 17, FieldId = "F648F51A-18A2-4662-9A36-0DF33675B908")]
        public string WelshAddress1 = string.Empty;

        /// <summary>
        /// The welsh address 2.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 18, FieldId = "FD9220F8-250A-45D7-B0BA-DC8AD14F3838")]
        public string WelshAddress2 = string.Empty;

        /// <summary>
        /// The welsh address 3.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 19, FieldId = "94EF7BAB-D2F5-4206-89EB-FB804D2FDCF9")]
        public string WelshAddress3 = string.Empty;

        /// <summary>
        /// The welsh town translation.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 20, FieldId = "43B82F7A-259F-4A31-93AB-8F1E8D004E05")]
        public string WelshTownTranslation = string.Empty;

        /// <summary>
        /// The ESR last update.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 21, FieldId = "D62DB224-5C6C-4CC6-A8FD-930A1EB711F8")]
        public DateTime? ESRLastUpdate;
    }
}
