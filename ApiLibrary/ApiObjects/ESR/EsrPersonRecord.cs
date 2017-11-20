namespace ApiLibrary.ApiObjects.ESR
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;

    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// The ESR person record.
    /// </summary>
    [KnownType(typeof(EsrAddressRecord))]
    [KnownType(typeof(EsrAssignmentRecord))]
    [KnownType(typeof(EsrAssignmentCostingRecord))]
    [KnownType(typeof(EsrLocationRecord))]
    [KnownType(typeof(EsrPhoneRecord))]
    [KnownType(typeof(EsrVehicleRecord))]
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrPersonRecord : EsrPerson
    {
        #region Fields

        /// <summary>
        /// The assignments for this person record.
        /// </summary>
        [DataMember]
        private List<EsrAssignment> assignments;

        /// <summary>
        /// The phones for this person.
        /// </summary>
        [DataMember]
        private List<EsrPhone> phones;

        /// <summary>
        /// The addresses for this person.
        /// </summary>
        [DataMember]
        private List<EsrAddress> addresses;

        /// <summary>
        /// The vehicles assigned to this person.
        /// </summary>
        [DataMember]
        private List<EsrVehicle> vehicles;

        /// <summary>
        /// The assignment COSTINGS assigned to this person
        /// </summary>
        [DataMember]
        private List<EsrAssignmentCostings> assignmentCostings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPersonRecord"/> class.
        /// </summary>
        public EsrPersonRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPersonRecord"/> class.
        /// </summary>
        /// <param name="esrPersonId">
        /// The ESR person id.
        /// </param>
        /// <param name="effectiveStartDate">
        /// The effective start date.
        /// </param>
        /// <param name="effectiveEndDate">
        /// The effective end date.
        /// </param>
        /// <param name="employeeNumber">
        /// The employee number.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="lastname">
        /// The last name.
        /// </param>
        /// <param name="firstname">
        /// The first name.
        /// </param>
        /// <param name="middleNames">
        /// The middle names.
        /// </param>
        /// <param name="maidenName">
        /// The maiden name.
        /// </param>
        /// <param name="preferredName">
        /// The preferred name.
        /// </param>
        /// <param name="previousLastName">
        /// The previous last name.
        /// </param>
        /// <param name="gender">
        /// The gender.
        /// </param>
        /// <param name="dateOfBirth">
        /// The date of birth.
        /// </param>
        /// <param name="nationalInsuranceNumber">
        /// The NI number.
        /// </param>
        /// <param name="nhsUniqueId">
        /// The NHS unique id.
        /// </param>
        /// <param name="hireDate">
        /// The hire date.
        /// </param>
        /// <param name="actualTerminationDate">
        /// The actual termination date.
        /// </param>
        /// <param name="terminationReason">
        /// The termination reason.
        /// </param>
        /// <param name="employeeStatusFlag">
        /// The employee status flag.
        /// </param>
        /// <param name="wtrOptOut">
        /// The WTR opt out.
        /// </param>
        /// <param name="wtrOptOutDate">
        /// The WTR opt out date.
        /// </param>
        /// <param name="ethnicOrigin">
        /// The ethnic origin.
        /// </param>
        /// <param name="maritalStatus">
        /// The marital status.
        /// </param>
        /// <param name="countryOfBirth">
        /// The country of birth.
        /// </param>
        /// <param name="previousEmployer">
        /// The previous employer.
        /// </param>
        /// <param name="previousEmployerType">
        /// The previous employer type.
        /// </param>
        /// <param name="csd3Months">
        /// The CSD 3 months.
        /// </param>
        /// <param name="csd12Months">
        /// The CSD 12 months.
        /// </param>
        /// <param name="nhsCrdsUuid">
        /// The NHS CRDS UUID.
        /// </param>
        /// <param name="systemPersonType">
        /// The system person type.
        /// </param>
        /// <param name="userPersonType">
        /// The user person type.
        /// </param>
        /// <param name="officeEmailAddress">
        /// The office email address
        /// </param>
        /// <param name="nhsStartDate">
        /// The NHS start date.
        /// </param>
        /// <param name="esrLastUpdateDate">
        /// The ESR last update date.
        /// </param>
        /// <param name="disabilityFlag">
        /// The disability flag.
        /// </param>
        /// <param name="legacyPayrollNumber">
        /// The legacy payroll number.
        /// </param>
        /// <param name="nationality">
        /// The persons nationality
        /// </param>
        public EsrPersonRecord(
            long esrPersonId,
            DateTime effectiveStartDate,
            DateTime? effectiveEndDate,
            string employeeNumber,
            string title,
            string lastname,
            string firstname,
            string middleNames,
            string maidenName,
            string preferredName,
            string previousLastName,
            string gender,
            DateTime? dateOfBirth,
            string nationalInsuranceNumber,
            string nhsUniqueId,
            DateTime? hireDate,
            DateTime? actualTerminationDate,
            string terminationReason,
            string employeeStatusFlag,
            string wtrOptOut,
            DateTime? wtrOptOutDate,
            string ethnicOrigin,
            string maritalStatus,
            string countryOfBirth,
            string previousEmployer,
            string previousEmployerType,
            DateTime? csd3Months,
            DateTime? csd12Months,
            string nhsCrdsUuid,
            string systemPersonType,
            string userPersonType,
            string officeEmailAddress,
            DateTime? nhsStartDate,
            DateTime? esrLastUpdateDate,
            string disabilityFlag,
            string legacyPayrollNumber,
            string nationality)
        {
            this.ESRPersonId = esrPersonId;
            this.EffectiveStartDate = effectiveStartDate;
            this.EffectiveEndDate = effectiveEndDate;
            this.EmployeeNumber = employeeNumber;
            this.Title = title;
            this.LastName = lastname;
            this.FirstName = firstname;
            this.MiddleNames = middleNames;
            this.MaidenName = maidenName;
            this.PreferredName = preferredName;
            this.PreviousLastName = previousLastName;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.NINumber = nationalInsuranceNumber;
            this.NHSUniqueId = nhsUniqueId;
            this.HireDate = hireDate;
            this.ActualTerminationDate = actualTerminationDate;
            this.TerminationReason = terminationReason;
            this.EmployeeStatusFlag = employeeStatusFlag;
            this.WTROptOut = wtrOptOut;
            this.WTROptOutDate = wtrOptOutDate;
            this.EthnicOrigin = ethnicOrigin;
            this.MaritalStatus = maritalStatus;
            this.CountryOfBirth = countryOfBirth;
            this.PreviousEmployer = previousEmployer;
            this.PreviousEmployerType = previousEmployerType;
            this.CSD3Months = csd3Months;
            this.CSD12Months = csd12Months;
            this.NHSCRSUUID = nhsCrdsUuid;
            this.SystemPersonType = systemPersonType;
            this.UserPersonType = userPersonType;
            this.OfficeEmailAddress = officeEmailAddress;
            this.NHSStartDate = nhsStartDate;
            this.ESRLastUpdateDate = esrLastUpdateDate;
            this.DisabilityFlag = disabilityFlag;
            this.LegacyPayrollNumber = legacyPayrollNumber;
            this.Nationality = nationality;

            this.assignments = new List<EsrAssignment>();
            this.phones = new List<EsrPhone>();
            this.addresses = new List<EsrAddress>();
            this.vehicles = new List<EsrVehicle>();
            this.assignmentCostings = new List<EsrAssignmentCostings>();
        }

        #endregion Constructors

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum PersonRecordColumnsRef
        {
            RecordType = 0,
            PersonId,
            EffectiveStartDate,
            EffectiveEndDate,
            EmployeeNumber,
            Title,
            LastName,
            FirstName,
            MiddleNames,
            MaidenName,
            PreferredName,
            PreviousLastName,
            Gender,
            DateOfBirth,
            NiNumber,
            NhsUniqueId,
            HireDate,
            ActualTerminationDate,
            TerminationReason,
            EmployeeStatusFlag,
            WtrOptOut,
            WtrOptOutDate,
            EthnicOrigin,
            MaritalStatus,
            CountryOfBirth,
            PreviousEmployer,
            PreviousEmployerType,
            Csd3Months,
            Csd12Months,
            NhsCrsUuid,
            Spare1,
            Spare2,
            Spare3,
            SystemPersonType,
            UserPersonType,
            OfficeEmailAddress,
            NhsStartDate,
            Spare4,
            LastUpdateDate,
            DisabilityFlag,
            LegacyPayrollNumber,
            Nationality,
            Spare6,
            Spare7
        }

        #region Properties

        /// <summary>
        /// Gets or sets the assignments for this person.
        /// </summary>
        public List<EsrAssignment> EsrAssignments
        {
            get
            {
                return this.assignments;
            }

            set
            {
                this.assignments = value;
            }
        }

        /// <summary>
        /// Gets or sets the phones for this person.
        /// </summary>
        public List<EsrPhone> Phones
        {
            get
            {
                return this.phones;
            }

            set
            {
                this.phones = value;
            }
        }

        /// <summary>
        /// Gets or sets the addresses for this person.
        /// </summary>
        public List<EsrAddress> Addresses
        {
            get
            {
                return this.addresses;
            }

            set
            {
                this.addresses = value;
            }
        }

        /// <summary>
        /// Gets or sets the vehicles for this person.
        /// </summary>
        public List<EsrVehicle> Vehicles
        {
            get
            {
                return this.vehicles;
            }

            set
            {
                this.vehicles = value;
            }
        }

        /// <summary>
        /// Gets or sets the assignment COSTINGS for this person
        /// </summary>
        public List<EsrAssignmentCostings> AssignmentCostings
        {
            get
            {
                return this.assignmentCostings;
            }

            set
            {
                this.assignmentCostings = value;
            }
        }

        /// <summary>
        /// Gets the primary assignment for this person. If the assignment has no effective end date, 
        /// then this is deemed the primary assignment, otherwise it's the assignment with the latest 
        /// effective end date.
        /// </summary>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        [ScriptIgnore]
        public EsrAssignment PrimaryAssignment
        {
            get
            {
                if (this.EsrAssignments == null) return null;

                EsrAssignment first = null;
                EsrAssignment first1 = null;               
                List<EsrAssignment> list = this.EsrAssignments.Where(assignment => assignment.PrimaryAssignment && (assignment.EffectiveEndDate == null || assignment.EffectiveEndDate >= DateTime.Now.Date)).ToList();

                switch (list.Count)
                {
                    case 1:
                        first = list[0];
                        break;
                    default:
                    {
                        var latestEffectiveStartDate = EsrAssignments.Max(assignment => assignment.EffectiveStartDate);
                        foreach (EsrAssignment assignment in list.Where(assignment => assignment.EffectiveStartDate == latestEffectiveStartDate))
                        {
                            first1 = assignment;
                        }
                    }
                        break;
                }

                return first ?? first1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into ESRPersonRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns>
        /// ESR record
        /// </returns>
        public static EsrPersonRecord ParseEsrPersonRecord(string recordLine)
        {
            #region record variables

            long esrPersonId = 0;
            DateTime effectiveStartDate = new DateTime(1900, 1, 1);
            DateTime? effectiveEndDate = null;
            string employeeNumber = string.Empty;
            string title = string.Empty;
            string lastname = string.Empty;
            string firstname = string.Empty;
            string middleNames = string.Empty;
            string maidenName = string.Empty;
            string preferredName = string.Empty;
            string previousLastName = string.Empty;
            string gender = string.Empty;
            DateTime? dateOfBirth = null;
            string niNumber = string.Empty;
            string nhsUniqueId = string.Empty;
            DateTime? hireDate = null;
            DateTime? actualTerminationDate = null;
            string terminationReason = string.Empty;
            string employeeStatusFlag = string.Empty;
            string wtrOptOut = string.Empty;
            DateTime? wtrOptOutDate = null;
            string ethnicOrigin = string.Empty;
            string maritalStatus = string.Empty;
            string countryOfBirth = string.Empty;
            string previousEmployer = string.Empty;
            string previousEmployerType = string.Empty;
            DateTime? csd3Months = null;
            DateTime? csd12Months = null;
            string nhsCrdsUuid = string.Empty;
            string systemPersonType = string.Empty;
            string userPersonType = string.Empty;
            string officeEmailAddress = string.Empty;
            DateTime? nhsStartDate = null;
            DateTime? esrLastUpdateDate = null;
            string disabilityFlag = string.Empty;
            string legacyPayrollNumber = string.Empty;
            string nationality = string.Empty;

            #endregion record variable

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)PersonRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrPersonUpdateRecordType:
                    if (rec.Length != ((int)PersonRecordColumnsRef.Spare7 + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    long.TryParse(rec[(int)PersonRecordColumnsRef.PersonId], out esrPersonId);
                    DateTime tmpDate;
                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.EffectiveStartDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveStartDate = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.EffectiveEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveEndDate = tmpDate;
                    }

                    employeeNumber = rec[(int)PersonRecordColumnsRef.EmployeeNumber];
                    title = rec[(int)PersonRecordColumnsRef.Title];
                    firstname = rec[(int)PersonRecordColumnsRef.FirstName];
                    lastname = rec[(int)PersonRecordColumnsRef.LastName];
                    middleNames = rec[(int)PersonRecordColumnsRef.MiddleNames];
                    maidenName = rec[(int)PersonRecordColumnsRef.MaidenName];
                    preferredName = rec[(int)PersonRecordColumnsRef.PreferredName];
                    previousLastName = rec[(int)PersonRecordColumnsRef.PreviousLastName];
                    gender = rec[(int)PersonRecordColumnsRef.Gender];
                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.DateOfBirth], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        dateOfBirth = tmpDate;
                    }

                    niNumber = rec[(int)PersonRecordColumnsRef.NiNumber];
                    nhsUniqueId = rec[(int)PersonRecordColumnsRef.NhsUniqueId];
                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.HireDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        hireDate = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.ActualTerminationDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        actualTerminationDate = tmpDate;
                    }

                    terminationReason = rec[(int)PersonRecordColumnsRef.TerminationReason];
                    employeeStatusFlag = rec[(int)PersonRecordColumnsRef.EmployeeStatusFlag];
                    wtrOptOut = rec[(int)PersonRecordColumnsRef.WtrOptOut];
                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.WtrOptOutDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        wtrOptOutDate = tmpDate;
                    }

                    ethnicOrigin = rec[(int)PersonRecordColumnsRef.EthnicOrigin];
                    maritalStatus = rec[(int)PersonRecordColumnsRef.MaritalStatus];
                    countryOfBirth = rec[(int)PersonRecordColumnsRef.CountryOfBirth];
                    previousEmployer = rec[(int)PersonRecordColumnsRef.PreviousEmployer];
                    previousEmployerType = rec[(int)PersonRecordColumnsRef.PreviousEmployerType];
                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.Csd3Months], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        csd3Months = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.Csd12Months], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        csd12Months = tmpDate;
                    }

                    nhsCrdsUuid = rec[(int)PersonRecordColumnsRef.NhsCrsUuid];
                    systemPersonType = rec[(int)PersonRecordColumnsRef.SystemPersonType];
                    userPersonType = rec[(int)PersonRecordColumnsRef.UserPersonType];
                    officeEmailAddress = rec[(int)PersonRecordColumnsRef.OfficeEmailAddress];
                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.NhsStartDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        nhsStartDate = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)PersonRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        esrLastUpdateDate = tmpDate;
                    }

                    disabilityFlag = rec[(int)PersonRecordColumnsRef.DisabilityFlag];
                    legacyPayrollNumber = rec[(int)PersonRecordColumnsRef.LegacyPayrollNumber];
                    nationality = rec[(int)PersonRecordColumnsRef.Nationality];
                    break;

                case EsrRecordTypes.EsrPersonDeleteRecordType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrPersonId);
                    break;
                default:
                    return null;
            }

            return new EsrPersonRecord(
                esrPersonId,
                effectiveStartDate,
                effectiveEndDate,
                employeeNumber,
                title,
                lastname,
                firstname,
                middleNames,
                maidenName,
                preferredName,
                previousLastName,
                gender,
                dateOfBirth,
                niNumber,
                nhsUniqueId,
                hireDate,
                actualTerminationDate,
                terminationReason,
                employeeStatusFlag,
                wtrOptOut,
                wtrOptOutDate,
                ethnicOrigin,
                maritalStatus,
                countryOfBirth,
                previousEmployer,
                previousEmployerType,
                csd3Months,
                csd12Months,
                nhsCrdsUuid,
                systemPersonType,
                userPersonType,
                officeEmailAddress,
                nhsStartDate,
                esrLastUpdateDate,
                disabilityFlag,
                legacyPayrollNumber,
                nationality) { Action = recordAction };
        }

        /// <summary>
        /// Adds an ESR Assignment Record to the ESR Person Record
        /// </summary>
        /// <param name="rec">Assignment Record object</param>
        public void AddAssignmentRecord(EsrAssignmentRecord rec)
        {
            this.assignments.Add(rec);
        }

        /// <summary>
        /// Adds an ESR Phone Record to the ESR Person Record
        /// </summary>
        /// <param name="rec">Phone Record object</param>
        public void AddPhoneRecord(EsrPhoneRecord rec)
        {
            this.phones.Add(rec);
        }

        /// <summary>
        /// Adds an ESR Address Record to the ESR Person Record
        /// </summary>
        /// <param name="rec">Address Record object</param>
        public void AddAddressRecord(EsrAddressRecord rec)
        {
            this.addresses.Add(rec);
        }

        /// <summary>
        /// Adds an ESR Vehicle Record to the ESR Person Record
        /// </summary>
        /// <param name="rec">Vehicle Record object</param>
        public void AddVehicleRecord(EsrVehicleRecord rec)
        {
            this.vehicles.Add(rec);
        }

        /// <summary>
        /// Adds an ESR Assignment Costing Record to the ESR Person Record
        /// </summary>
        /// <param name="rec">Assignment Costing Record object</param>
        public void AddAssignmentCostingRecord(EsrAssignmentCostingRecord rec)
        {
            this.assignmentCostings.Add(rec);
        }

        /// <summary>
        /// Validate this instance of ESR person record
        /// Making sure the date time fields are valid
        /// that the sub records are all valid 
        /// and that the ESR person id matches in all records.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsValid()
        {
            var result = base.IsValid();
            if (this.ActionResult == null)
            {
                this.ActionResult = new ApiResult();
            }

            if (result)
            {
                List<EsrAssignment> esrAssignments = this.assignments;
                if (esrAssignments != null)
                {
                    foreach (EsrAssignment esrAssignment in esrAssignments)
                    {
                        if (!esrAssignment.IsValid())
                        {
                            this.ActionResult.Result = esrAssignment.ActionResult.Result;
                            this.ActionResult.Message += esrAssignment.ActionResult.Message;
                            result = false;
                            break;
                        }

                        if (this.ESRPersonId != esrAssignment.ESRPersonId)
                        {
                            this.ActionResult.Result = ApiActionResult.ValidationFailed;
                            this.ActionResult.Message = string.Format("ESR Assignment: {0} - ESR Person ID-{1} does not match parent ID {2}", esrAssignment.AssignmentID, esrAssignment.ESRPersonId, this.ESRPersonId);
                            result = false;
                        }
                    }
                }

                List<EsrAddress> esrAddresses = this.addresses;
                if (esrAddresses != null)
                {
                    foreach (EsrAddress esrAddress in esrAddresses)
                    {
                        if (!esrAddress.IsValid())
                        {
                            this.ActionResult.Result = esrAddress.ActionResult.Result;
                            this.ActionResult.Message += esrAddress.ActionResult.Message;
                            result = false;
                            break;
                        }

                        if (this.ESRPersonId != esrAddress.ESRPersonId)
                        {
                            this.ActionResult.Result = ApiActionResult.ValidationFailed;
                            this.ActionResult.Message = string.Format("ESR Address: {0} - ESR Person ID-{1} does not match parent ID {2}", esrAddress.ESRAddressId, esrAddress.ESRPersonId, this.ESRPersonId);
                            result = false;
                        }
                    }
                }

                List<EsrPhone> esrPhones = this.Phones;
                if (esrPhones != null)
                {
                    foreach (EsrPhone esrPhone in esrPhones)
                    {
                        if (!esrPhone.IsValid())
                        {
                            this.ActionResult.Result = esrPhone.ActionResult.Result;
                            this.ActionResult.Message += esrPhone.ActionResult.Message;
                            result = false;
                            break;
                        }

                        if (this.ESRPersonId != esrPhone.ESRPersonId)
                        {
                            this.ActionResult.Result = ApiActionResult.ValidationFailed;
                            this.ActionResult.Message = string.Format("ESR Phone: {0} - ESR Person ID-{1} does not match parent ID {2}", esrPhone.ESRPhoneId, esrPhone.ESRPersonId, this.ESRPersonId);
                            result = false;
                        }
                    }
                }

                List<EsrVehicle> esrVehicles = this.Vehicles;
                if (esrVehicles != null)
                {
                    foreach (EsrVehicle esrVehicle in esrVehicles)
                    {
                        if (!esrVehicle.IsValid())
                        {
                            this.ActionResult.Result = esrVehicle.ActionResult.Result;
                            this.ActionResult.Message += esrVehicle.ActionResult.Message;
                            result = false;
                            break;
                        }

                        if (this.ESRPersonId != esrVehicle.ESRPersonId)
                        {
                            this.ActionResult.Result = ApiActionResult.ValidationFailed;
                            this.ActionResult.Message = string.Format("ESR Vehicle: {0} - ESR Person ID-{1} does not match parent ID {2}", esrVehicle.ESRVehicleAllocationId, esrVehicle.ESRPersonId, this.ESRPersonId);
                            result = false;
                        }
                    }
                }

                List<EsrAssignmentCostings> esrCostings = this.AssignmentCostings;
                if (esrCostings != null)
                {
                    foreach (EsrAssignmentCostings esrCosting in esrCostings)
                    {
                        if (!esrCosting.IsValid())
                        {
                            this.ActionResult.Result = esrCosting.ActionResult.Result;
                            this.ActionResult.Message += esrCosting.ActionResult.Message;
                            result = false;
                            break;
                        }

                        if (this.ESRPersonId != esrCosting.ESRPersonId)
                        {
                            this.ActionResult.Result = ApiActionResult.ValidationFailed;
                            this.ActionResult.Message = string.Format("ESR Assignment Costing: {0} - ESR Person ID-{1} does not match parent ID {2}", esrCosting.ESRCostingAllocationId, esrCosting.ESRPersonId, this.ESRPersonId);
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        #endregion Methods
    }
}