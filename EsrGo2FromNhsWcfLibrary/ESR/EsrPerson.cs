namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The ESR person.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Employee, TableId = "59EBDE1E-3E1D-408A-8D86-5C35CAF7FD5F")]
    public class EsrPerson : DataClassBase
    {
        [DataClass(IsKeyField = true, ColumnRef = 1, FieldId = "D9141B31-B265-4ACE-9525-E18D1B76BB63")]
        [DataMember(IsRequired = true)]
        public long ESRPersonId;
        [DataMember]
        [DataClass(ColumnRef = 2, FieldId = "46663A57-596C-4693-9A72-92A009429080")]
        public DateTime? EffectiveStartDate;
        [DataMember]
        [DataClass(ColumnRef = 3, FieldId = "9FAD6B7F-56D1-4960-AB79-114F2488F6AD")]
        public DateTime? EffectiveEndDate;
        [DataMember]
        [DataClass(ColumnRef = 4, FieldId = "E842B645-13FC-45CF-B995-2D624156FBAE")]
        public string EmployeeNumber = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 5, FieldId = "32E1A4D1-0FC5-4517-8506-144841397179")]
        public string Title = string.Empty;
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 6, FieldId = "4542D749-14CC-44B2-86D8-5ABD1AC79B86")]
        public string LastName = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 7, FieldId = "19C0C542-C5FE-42CE-AF4D-042213C4D07A")]
        public string FirstName = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "00C46631-7F24-4587-8374-9C4F6D44E105")]
        public string MiddleNames = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "FF893463-47E8-4148-9051-763A1FE37D53")]
        public string MaidenName = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "77771D45-18CA-4220-9B76-BB3C7D0F944C")]
        public string PreferredName = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "FFE03D40-C93F-48C6-9B2A-5234F53665C0")]
        public string PreviousLastName = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "D1940F37-F3C6-4346-97EB-4C72B8CBF5F5")]
        public string Gender = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "EC2EEA61-27BA-4814-93BE-CFD2747F8CA1")]
        public DateTime? DateOfBirth;
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "43D43B3E-C969-4BE1-97E5-BA8CF6E5105F")]
        public string NINumber = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 15, FieldId = "01C68BEB-0D3E-40C8-9D77-E842E5D7368F")]
        public string NHSUniqueId = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 16, FieldId = "1BE36538-E5C5-4460-B51E-5B6A3C2CFA8F")]
        public DateTime? HireDate;
        [DataMember]
        [DataClass(ColumnRef = 17, FieldId = "DA1F50A5-63C7-4A00-BE8C-94C2AF399B11")]
        public DateTime? ActualTerminationDate;
        [DataMember]
        [DataClass(ColumnRef = 18, FieldId = "46CD5333-B3A0-45E4-A36E-A60826A1DC91")]
        public string TerminationReason = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 19, FieldId = "732DD2E5-395C-4754-9A7F-0868FE43EE88")]
        public string EmployeeStatusFlag = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 20, FieldId = "1EFEA25A-C5FF-4721-B74A-18E967A8F450")]
        public string WTROptOut = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 21, FieldId = "775F6E6C-EF80-4961-8959-7C7EEED16B30")]
        public DateTime? WTROptOutDate;
        [DataMember]
        [DataClass(ColumnRef = 22, FieldId = "2B56BCBB-DE48-4B9E-8615-CB161639ABD5")]
        public string EthnicOrigin = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 23, FieldId = "1444F729-1386-41D6-BE47-8E9D95571966")]
        public string MaritalStatus = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 24, FieldId = "3FF0E8AE-B0BF-4BFD-8098-7C18DDCF3B8F")]
        public string CountryOfBirth = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 25, FieldId = "283CEE14-C66E-44BC-BEA9-7CD18AB62529")]
        public string PreviousEmployer = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 26, FieldId = "706CD23E-1A01-4680-8D1D-CCFEB155D0CB")]
        public string PreviousEmployerType = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 27, FieldId = "E3C89ACF-5129-40C0-A9A2-3261B4A3485A")]
        public DateTime? CSD3Months;
        [DataMember]
        [DataClass(ColumnRef = 28, FieldId = "051ACF61-5A48-4A86-82B8-1DC639ADD9F8")]
        public DateTime? CSD12Months;
        [DataMember]
        [DataClass(ColumnRef = 29, FieldId = "601D387F-B7CE-4E5C-954C-CCBBDE170187")]
        public string NHSCRSUUID = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 33, FieldId = "B943520A-693B-4040-96AF-44CF3D5C5520")]
        public string SystemPersonType = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 34, FieldId = "B4A8657E-9C40-407D-BE00-1C33C36A1094")]
        public string UserPersonType = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 35, FieldId = "354879AC-E509-40D4-AA5E-4D5A44380E53")]
        public string OfficeEmailAddress = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 36, FieldId = "9086D822-7B9D-414E-B325-E051D4EC925A")]
        public DateTime? NHSStartDate;
        [DataMember]
        [DataClass(ColumnRef = 38, FieldId = "3CEB6D76-971A-405D-B60B-3CC14326380B")]
        public DateTime? ESRLastUpdateDate;
        [DataMember]
        [DataClass(ColumnRef = 39, FieldId = "0D9AC626-9A47-4535-B920-D14494BB49CF")]
        public string DisabilityFlag = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 40, FieldId = "4DB3334A-796A-4A8F-A261-865B46916983")]
        public string LegacyPayrollNumber = string.Empty;
        [DataMember]
        [DataClass(ColumnRef = 41, FieldId = "DED19FB5-57C1-4AA9-AAB8-C69EA1F1456E")]
        public string Nationality = string.Empty;

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Notes the position index within the source collection (used for post processing batches)
        /// </summary>
        [DataMember]
        public int RecordPositionIndex { get; set; }
    }
}
