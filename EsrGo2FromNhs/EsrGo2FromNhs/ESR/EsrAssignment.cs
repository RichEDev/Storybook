namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The ESR assignments Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.Assignment, TableId = "BF9AA39A-82D6-4960-BFEF-C5943BC0542D")]
    public class EsrAssignment : DataClassBase
    {
        /// <summary>
        /// The esr assign id. 
        /// (Primary key) Identity
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true, ColumnRef = 0, FieldId = "E7850CCA-7EEF-4AF2-98BF-F6E7089FDB15")]
        public int esrAssignID;
        
        /// <summary>
        /// The ESR person id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 1, FieldId = "C08FF30B-997E-48F5-A6D1-E4C24AC38CC1")]
        public long? ESRPersonId;

        /// <summary>
        /// The employee id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 0, FieldId = "0CFDCDCD-1F51-4578-BE89-90610D6D7F7D")]
        public int employeeid;

        /// <summary>
        /// The assignment id.
        /// </summary>
        [DataClass(ColumnRef = 2, FieldId = "7C0D8EAB-D9AF-415F-9BB7-D1BE01F69E2F")]
        [DataMember]
        public long AssignmentID;

        /// <summary>
        /// The effective start date.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 3, FieldId = "438AA555-1748-4C26-91FE-8044138A2E76")]
        public DateTime EffectiveStartDate;

        /// <summary>
        /// The effective end date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 4, FieldId = "9A0C99B6-DA12-44DD-894F-DAC34CAB04FD")]
        public DateTime? EffectiveEndDate;

        /// <summary>
        /// The earliest assignment start date.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 5, FieldId = "C53828AF-99FF-463F-93F9-2721DF44E5F2")]
        public DateTime EarliestAssignmentStartDate;

        /// <summary>
        /// The assignment type.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 6, FieldId = "4AA315A6-6472-4C71-8C85-D4F117DAF2E1")]
        public string AssignmentType;
        
        /// <summary>
        /// The assignment number.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 7, FieldId = "C23858B8-7730-440E-B481-C43FE8A1DBEF")]
        public string AssignmentNumber;

        /// <summary>
        /// The system assignment status as defined by ESR
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 8, FieldId = "FC25E956-648A-4066-9E08-C94B7B6706F5")]
        public string SystemAssignmentStatus;

        /// <summary>
        /// The assignment status as seen by the end user
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 9, FieldId = "9721EC22-404B-468B-83A4-D17D7559D3EF")]
        public string AssignmentStatus;

        /// <summary>
        /// The employee status flag.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 10, FieldId = "D2B92C35-E369-4CE9-95A4-AF2173840B01")]
        public string EmployeeStatusFlag;

        /// <summary>
        /// The payroll name.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 11, FieldId = "E8EA6D07-EC23-4D41-8B42-416F0E139990")]
        public string PayrollName;

        /// <summary>
        /// The payroll period type.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 12, FieldId = "4FCA7747-A09A-424F-A1E1-371C91367840")]
        public string PayrollPeriodType;

        /// <summary>
        /// The esr location id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 13, FieldId = "CAEE4043-B236-47EB-A067-C11422F38A17")]
        public long? EsrLocationId;

        /// <summary>
        /// The supervisor flag.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 14, FieldId = "AC849695-59E2-46BF-BB19-5B57F830FEEB")]
        public string SupervisorFlag;

        /// <summary>
        /// The supervisor person id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 15, FieldId = "D68DB9D1-0A73-4A76-AAE9-F0DE5F19F9FF")]
        public long? SupervisorPersonId;

        /// <summary>
        /// The supervisor person id (preserved copy).
        /// </summary>
        [DataMember]
        public long? SafeSupervisorPersonId { get; set; }

        /// <summary>
        /// The supervisor assignment id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 16, FieldId = "3A0A78FB-EED3-4D70-94C1-12B8DE558CD1")]
        public long? SupervisorAssignmentId;

        /// <summary>
        /// The supervisor assignment id (preserved copy).
        /// </summary>
        [DataMember]
        public long? SafeSupervisorAssignmentId { get; set; }

        /// <summary>
        /// The supervisor assignment number.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 17, FieldId = "ED828976-F992-4ADC-B461-27EE83EBFDC8")]
        public string SupervisorAssignmentNumber;

        /// <summary>
        /// The supervisor assignment number (preserved copy).
        /// </summary>
        [DataMember]
        public string SafeSupervisorAssignmentNumber { get; set; }

        /// <summary>
        /// The ESR Person Id of the department manager
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 18, FieldId = "081837AA-A9EF-4316-BF97-15507B7FEBFE")]
        public long? DepartmentManagerPersonId;

        /// <summary>
        /// The ESR Person Id of the department manager (preserved copy)
        /// </summary>
        [DataMember]
        public long? SafeDepartmentManagerPersonId { get; set; }

        /// <summary>
        /// The employee category.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 19, FieldId = "192C97C3-7180-45BD-9931-E324646ADDDA")]
        public string EmployeeCategory;

        /// <summary>
        /// The assignment category.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 20, FieldId = "D7DF66FF-B7F4-41E6-B288-CDE04281FB99")]
        public string AssignmentCategory;

        /// <summary>
        /// Primary assignment description
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 0, FieldId = "BAFC33B5-7538-4118-8C59-8095D0B285EF")]
        public string PrimaryAssignmentString = string.Empty;

        /// <summary>
        /// Indicates that this is the primary assignment
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(ColumnRef = 21, FieldId = "FEC46ED7-57F9-4C51-9916-EC92834371C3")]
        public bool PrimaryAssignment;

        /// <summary>
        /// The normal hours.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 22, FieldId = "47FE49AE-6417-4A0E-84A9-3EB73899F419")]
        public decimal NormalHours;

        /// <summary>
        /// The normal hours frequency.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 23, FieldId = "29E030BD-1436-4C2D-BAD6-A6152EC2B0CA")]
        public string NormalHoursFrequency;

        /// <summary>
        /// The grade contract hours.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 24, FieldId = "8BE1B94F-D037-464D-8777-9A3F038406B1")]
        public decimal GradeContractHours;

        /// <summary>
        /// The grade contract hours / normal hours for the grade
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 25, FieldId = "495A9714-5ED7-414F-8E9F-B08A7A7CAD5C")]
        public decimal Fte;

        /// <summary>
        /// The flexible working pattern.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 26, FieldId = "49443762-7092-4B05-A311-9DAD3E893DB3")]
        public string FlexibleWorkingPattern;

        /// <summary>
        /// The ID of Assignment Organisation Unit Record
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 27, FieldId = "5D35FFE4-6915-474C-B195-93DD543474C0")]
        public long? EsrOrganisationId;

        /// <summary>
        /// The ID of Assignment Position Record
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 28, FieldId = "DB2943C1-EF09-452C-B295-7CB64D6D0B85")]
        public long? EsrPositionId;

        /// <summary>
        /// The full name of the assignment position
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 29, FieldId = "1A623436-381A-4508-A8CF-B6AC4CD50F00")]
        public string PositionName;

        /// <summary>
        /// The grade.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 30, FieldId = "EF3A5A18-D783-4415-B0A5-8EE36CBD4EA8")]
        public string Grade;

        /// <summary>
        /// The grade step
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 31, FieldId = "836BA37D-2721-4777-8F2D-956922ECE316")]
        public string GradeStep;

        /// <summary>
        /// The date started in grade
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 32, FieldId = "5B89A474-9907-458A-8E1E-65B54DA3FE88")]
        public DateTime? StartDateInGrade;

        /// <summary>
        /// The Annual Salary Value
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 33, FieldId = "EAB628FA-4BEA-4881-88CC-7D17DEF33378")]
        public decimal AnnualSalaryValue;

        /// <summary>
        /// The job name.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 34, FieldId = "59A8B2FB-1874-41B8-AEAF-16E61AB01F8E")]
        public string JobName;

        /// <summary>
        /// The people group.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 35, FieldId = "A213CCDB-A364-449B-814D-7F433A182DDE")]
        public string Group;

        /// <summary>
        /// The t and a flag.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 36, FieldId = "D1F40F3F-F14E-4082-B7D6-87DECD3FAA2B")]
        public string TAndAFlag;

        /// <summary>
        /// The night worker opt out.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 37, FieldId = "48A15E1E-0729-4CC6-A702-E848B35F3B41")]
        public string NightWorkerOptOut;

        /// <summary>
        /// The projected hire date
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 38, FieldId = "772C17FA-A2C6-4562-835B-5162E397CA62")]
        public DateTime? ProjectedHireDate;

        /// <summary>
        /// The vacancy id.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 39, FieldId = "C44A365C-72B7-4462-8FC9-335239D19913")]
        public int? VacancyID;

        /// <summary>
        /// The contract end date for fixed term contracts
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 40, FieldId = "BAB9BE53-7DA3-4548-8076-394EEDAAD899")]
        public DateTime? ContractEndDate;

        /// <summary>
        /// The assignment increment date
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 41, FieldId = "81D74885-BF2F-45F4-B32D-5E671EEE693C")]
        public DateTime? IncrementDate;

        /// <summary>
        /// The maximum part time flag
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 42, FieldId = "A1852807-CB7A-42FD-876A-D73480372BBF")]
        public string MaximumPartTimeFlag;

        /// <summary>
        /// The AFC flag
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 43, FieldId = "7A4888B6-3F19-48F4-9CBB-6F54FD906CE0")]
        public string AfcFlag;

        /// <summary>
        /// The ESR last update date
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 44, FieldId = "7DC52316-924C-4A11-9FDA-17595A48E7CD")]
        public DateTime? EsrLastUpdate;

        /// <summary>
        /// The last working day
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 45, FieldId = "1D806CA8-DCE1-4E72-A31F-C9ECE41E27A6")]
        public DateTime? LastWorkingDay;

        /// <summary>
        /// The e-KSF Spinal Point
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 46, FieldId = "555D05FA-ACB8-4F3E-8C74-EDE58A854156")]
        public string eKSFSpinalPoint;

        /// <summary>
        /// The manager flag
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 47, FieldId = "EB20ACA8-2E16-46D0-B539-61C0DB8B7F34")]
        public string ManagerFlag;

        /// <summary>
        /// The final assignment end date.
        /// </summary>
        [DataMember]
        [DataClass(ColumnRef = 48, FieldId = "36EB4BB6-F4D5-414C-9106-EE62DB01D902")]
        public DateTime? FinalAssignmentEndDate;
        
        /////// <summary>
        /////// The createdon.
        /////// </summary>
        public DateTime createdon { get; set; }

        /////// <summary>
        /////// The modifiedon.
        /////// </summary>
        public DateTime? modifiedon { get; set; }

        ///// <summary>
        ///// The active.
        ///// </summary>
        [DataMember(IsRequired = true)]
        public bool Active;

        ///// <summary>
        ///// The created by.
        ///// </summary>
        public int? CreatedBy { get; set; }

        ///// <summary>
        ///// Modified by
        ///// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// The supervisor ESR assign id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass]
        public int? SupervisorEsrAssignID;

        /// <summary>
        /// The employee/team/budget holder who may sign-off claims in the format "{SpendManagementElementType},{PrimaryKeyId}"
        /// </summary>
        [DataMember]
        [DataClass]
        public string SignOffOwner;
    }
}
