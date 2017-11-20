namespace ApiLibrary.DataObjects.Spend_Management
{
    using System;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// How the address was created, taken from SpendManagementLibrary Addresses.cs
    /// </summary>
    public enum AddressCreationMethod
    {
        None = 0,

        CapturePlus = 1,

        Global = 2,

        EsrOutbound = 3,

        ImplementationImportRoutine = 4,

        DataImportWizard = 5,

        ManualByAdministrator = 6,

        ManualByClaimant = 7
    }

    /// <summary>
    /// The Addresses data table.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Address, TableId = "A3713E93-0ABE-4EAC-9533-6A22AA4C1F62")]
    public class Address : DataClassBase
    {
        [DataMember]
        [DataClass(IsKeyField = true, FieldId = "")]
        public int AddressID;

        [DataMember]
        [DataClass(FieldId = "85DB77A3-6BC0-494F-8085-C35E96B042BE")]
        public string AddressName;

        [DataMember]
        [DataClass(FieldId = "404B7297-FF10-45A1-8CF1-EA15CA0DF949")]
        public bool Archived;

        [DataMember]
        [DataClass(FieldId = "25597581-F3C5-40F0-B7BC-53FB67AC1A0E")]
        public string Line1;

        [DataMember]
        [DataClass(FieldId = "2FD99466-554C-43DB-93DE-878B17B0421F")]
        public string Line2;

        [DataMember]
        [DataClass(FieldId = "E38EB767-8A40-4DA1-BF1E-0F8F65C3A146")]
        public string Line3;

        [DataMember]
        [DataClass(FieldId = "B90920FE-BEA1-4948-A26B-CB997136C9F4")]
        public string City;

        [DataMember]
        [DataClass(FieldId = "4B2D3322-4651-4C22-9FB9-A5C806038D20")]
        public string County;

        [DataMember]
        [DataClass(FieldId = "52524292-4A74-4405-94AD-F8F680984AD7")]
        public int? Country;

        [DataMember]
        [DataClass(FieldId = "5A999C9F-440E-4D65-8B3C-187BA69E0234")]
        public string Postcode;

        [DataMember]
        [DataClass(FieldId = "")]
        public string Latitude;

        [DataMember]
        [DataClass(FieldId = "")]
        public string Longitude;

        [DataMember]
        [DataClass(FieldId = "")]
        public AddressCreationMethod CreationMethod;

        [DataMember]
        [DataClass(FieldId = "")]
        public string GlobalIdentifier;

        [DataMember]
        [DataClass(FieldId = "")]
        public int Udprn;

        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? LookupDate;

        [DataMember]
        [DataClass(FieldId = "")]
        public int? SubAccountID;

        [DataMember]
        [DataClass(FieldId = "")]
        public bool AccountWideFavourite;

        [DataMember]
        [DataClass(FieldId = "")]
        public long? EsrAddressID;

        [DataMember]
        [DataClass(FieldId = "")]
        public long? EsrLocationID;

        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? CreatedOn;

        [DataMember]
        [DataClass(FieldId = "")]
        public int? CreatedBy;

        [DataMember]
        [DataClass(FieldId = "")]
        public DateTime? ModifiedOn;

        [DataMember]
        [DataClass(FieldId = "")]
        public int? ModifiedBy;
    }
}
