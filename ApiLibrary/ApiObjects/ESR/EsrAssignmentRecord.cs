using System;

namespace ApiLibrary.ApiObjects.ESR
{
    using System.Globalization;
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// SERIALIZABLE ESR Assignment Record object
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrAssignmentRecord : EsrAssignment
    {
        #region public properties

        #endregion public properties

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentRecord"/> class.
        /// </summary>
        /// <param name="sysInternalEsrAssignId">
        ///     The sys internal ESR assign id.
        /// </param>
        /// <param name="esrPersonId">
        ///     The ESR person record.
        /// </param>
        /// <param name="assignmentId">
        ///     The assignment id.
        /// </param>
        /// <param name="effectiveStartDate">
        ///     The effective start date.
        /// </param>
        /// <param name="effectiveEndDate">
        ///     The effective end date.
        /// </param>
        /// <param name="earliestStartDate">
        ///     The earliest start date.
        /// </param>
        /// <param name="assignmentType">
        ///     The assignment type.
        /// </param>
        /// <param name="assignmentNumber">
        ///     The assignment number.
        /// </param>
        /// <param name="systemAssignmentStatus">
        ///     The system assignment status.
        /// </param>
        /// <param name="assignmentStatus">
        ///     The assignment status.
        /// </param>
        /// <param name="employeeStatusFlag">
        ///     The employee status flag.
        /// </param>
        /// <param name="payrollName">
        ///     The payroll name.
        /// </param>
        /// <param name="payrollPeriodType">
        ///     The payroll period type.
        /// </param>
        /// <param name="esrLocationId">
        ///     The ESR location id.
        /// </param>
        /// <param name="supervisorFlag">
        ///     The supervisor flag.
        /// </param>
        /// <param name="supervisorPersonId">
        ///     The supervisor ESR Person Id
        /// </param>
        /// <param name="supervisorAssignmentId">
        ///     The supervisor ESR Assignment Id
        /// </param>
        /// <param name="supervisorAssignmentNumber">
        ///     The supervisor assignment number.
        /// </param>
        /// <param name="departmentManagerPersonId">
        ///     The department manager person id.
        /// </param>
        /// <param name="employeeCategory">
        ///     The employee category.
        /// </param>
        /// <param name="assignmentCategory">
        ///     The assignment category.
        /// </param>
        /// <param name="primaryAssignment">
        ///     The primary assignment.
        /// </param>
        /// <param name="primaryAssignmentString">
        ///     The primary assignment string.
        /// </param>
        /// <param name="normalHours">
        ///     The normal hours.
        /// </param>
        /// <param name="normalHoursFrequency">
        ///     The normal hours frequency.
        /// </param>
        /// <param name="gradeContractHours">
        ///     The grade contract hours.
        /// </param>
        /// <param name="fte">
        ///     The FTE.
        /// </param>
        /// <param name="flexibleWorkingPattern">
        ///     The flexible working pattern.
        /// </param>
        /// <param name="esrOrganisationId">
        ///     The ESR organisation id.
        /// </param>
        /// <param name="esrPositionId">
        ///     The ESR position id.
        /// </param>
        /// <param name="positionName">
        ///     The position name.
        /// </param>
        /// <param name="grade">
        ///     The grade.
        /// </param>
        /// <param name="gradeStep">
        ///     The grade step.
        /// </param>
        /// <param name="startDateInGrade">
        ///     The start date in grade.
        /// </param>
        /// <param name="annualSalaryValue">
        ///     The annual salary value.
        /// </param>
        /// <param name="jobName">
        ///     The job name.
        /// </param>
        /// <param name="peopleGroup">
        ///     The people group.
        /// </param>
        /// <param name="tAndAFlag">
        ///     The t and a flag.
        /// </param>
        /// <param name="assignmentNightWorkerAttribute">
        ///     The assignment night worker attribute
        /// </param>
        /// <param name="projectedHireDate">
        ///     The projected hire date.
        /// </param>
        /// <param name="vacancyId">
        ///     The vacancy id.
        /// </param>
        /// <param name="contractEndDate">
        ///     The contract end date.
        /// </param>
        /// <param name="incrementDate">
        ///     The increment date.
        /// </param>
        /// <param name="maximumPartTimeFlag">
        ///     The maximum part time flag.
        /// </param>
        /// <param name="afcFlag">
        ///     The AFC flag.
        /// </param>
        /// <param name="esrLastUpdate">
        ///     The ESR last update.
        /// </param>
        /// <param name="lastWorkingDay">
        ///     The last working day.
        /// </param>
        /// <param name="eKsfFSpinalPoint">
        ///     The e KSF f spinal point.
        /// </param>
        /// <param name="managerFlag">
        ///     The manager flag.
        /// </param>
        /// <param name="assignmentEndDate">
        ///     The assignment end date.
        /// </param>
        public EsrAssignmentRecord(int sysInternalEsrAssignId, long? esrPersonId, long assignmentId, DateTime effectiveStartDate, DateTime? effectiveEndDate, DateTime earliestStartDate, string assignmentType, string assignmentNumber, string systemAssignmentStatus, string assignmentStatus, string employeeStatusFlag, string payrollName, string payrollPeriodType, long? esrLocationId, string supervisorFlag, long? supervisorPersonId, long? supervisorAssignmentId, string supervisorAssignmentNumber, long? departmentManagerPersonId, string employeeCategory, string assignmentCategory, bool primaryAssignment, string primaryAssignmentString, decimal normalHours, string normalHoursFrequency, decimal gradeContractHours, decimal fte, string flexibleWorkingPattern, long? esrOrganisationId, long? esrPositionId, string positionName, string grade, string gradeStep, DateTime? startDateInGrade, decimal annualSalaryValue, string jobName, string peopleGroup, string tAndAFlag, string assignmentNightWorkerAttribute, DateTime? projectedHireDate, int? vacancyId, DateTime? contractEndDate, DateTime? incrementDate, string maximumPartTimeFlag, string afcFlag, DateTime? esrLastUpdate, DateTime? lastWorkingDay, string eKsfFSpinalPoint, string managerFlag, DateTime? assignmentEndDate)
        {
            this.esrAssignID = sysInternalEsrAssignId;
            this.ESRPersonId = esrPersonId;
            this.AssignmentID = assignmentId;
            this.EffectiveStartDate = effectiveStartDate;
            this.EffectiveEndDate = effectiveEndDate;
            this.EarliestAssignmentStartDate = earliestStartDate;
            this.AssignmentType = assignmentType;
            this.AssignmentNumber = assignmentNumber;
            this.SystemAssignmentStatus = systemAssignmentStatus;
            this.AssignmentStatus = assignmentStatus;
            this.EmployeeStatusFlag = employeeStatusFlag;
            this.PayrollName = payrollName;
            this.PayrollPeriodType = payrollPeriodType;
            this.EsrLocationId = esrLocationId;
            this.SupervisorFlag = supervisorFlag;
            this.SupervisorPersonId = supervisorPersonId;
            this.SupervisorAssignmentId = supervisorAssignmentId;
            this.SupervisorAssignmentNumber = supervisorAssignmentNumber;
            this.DepartmentManagerPersonId = departmentManagerPersonId;
            this.EmployeeCategory = employeeCategory;
            this.AssignmentCategory = assignmentCategory;
            this.PrimaryAssignmentString = primaryAssignmentString;
            this.PrimaryAssignment = primaryAssignment;
            this.NormalHours = normalHours;
            this.NormalHoursFrequency = normalHoursFrequency;
            this.GradeContractHours = gradeContractHours;
            this.Fte = fte;
            this.FlexibleWorkingPattern = flexibleWorkingPattern;
            this.EsrOrganisationId = esrOrganisationId;
            this.EsrPositionId = esrPositionId;
            this.PositionName = positionName;
            this.Grade = grade;
            this.GradeStep = gradeStep;
            this.StartDateInGrade = startDateInGrade;
            this.AnnualSalaryValue = annualSalaryValue;
            this.JobName = jobName;
            this.Group = peopleGroup;
            this.TAndAFlag = tAndAFlag;
            this.NightWorkerOptOut = assignmentNightWorkerAttribute;
            this.ProjectedHireDate = projectedHireDate;
            this.VacancyID = vacancyId;
            this.ContractEndDate = contractEndDate;
            this.IncrementDate = incrementDate;
            this.MaximumPartTimeFlag = maximumPartTimeFlag;
            this.AfcFlag = afcFlag;
            this.EsrLastUpdate = esrLastUpdate;
            this.LastWorkingDay = lastWorkingDay;
            this.eKSFSpinalPoint = eKsfFSpinalPoint;
            this.ManagerFlag = managerFlag;
            this.FinalAssignmentEndDate = assignmentEndDate;
        }

        #endregion Constructors

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum AssignmentRecordColumnsRef
        {
            RecordType = 0,
            PersonId,
            AssignmentId,
            EffectiveStartDate,
            EffectiveEndDate,
            EarliestAssignmentDate,
            AssignmentType,
            AssignmentNumber,
            SystemAssignmentStatus,
            UserAssignmentStatus,
            EmployeeStatusFlag,
            PayrollName,
            PayrollPeriodType,
            AssignmentLocationId,
            SupervisorFlag,
            SupervisorPersonId,
            SupervisorAssignmentId,
            SupervisorAssignmentNumber,
            DepartmentManagerPersonId,
            EmployeeCategory,
            AssignmentCategory,
            PrimaryAssignment,
            NormalHours,
            Frequency,
            GradeContractHours,
            Fte,
            FlexibleWorkingPattern,
            OrganisationId,
            PositionId,
            PositionName,
            Grade,
            GradeStep,
            StartDateInGrade,
            AnnualSalaryValue,
            JobName,
            PeopleGroup,
            TandAFlag,
            NightWorkerOptOut,
            ProjectedHireDate,
            VacancyId,
            ContractEndDate,
            IncrementDate,
            MaximumPartTimeFlag,
            AfcFlag,
            LastUpdateDate,
            LastWorkingDay,
            EksfSpinalPoint,
            ManagerFlag,
            AssignmentEndDate,
            Spare
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into ESRAssignmentRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns>an ESR assignment record</returns>
        public static EsrAssignmentRecord ParseEsrAssignmentRecord(string recordLine)
        {
            #region record variables

            int sysInternalEsrAssignId = 0;
            long esrPersonId = 0;
            long assignmentId = 0;
            DateTime effectiveStartDate = new DateTime(1900, 1, 1);
            DateTime? effectiveEndDate = null;
            DateTime earliestStartDate = new DateTime(1900, 1, 1);
            string assignmentType = string.Empty;
            string assignmentNumber = string.Empty;
            string systemAssignmentStatus = string.Empty;
            string assignmentStatus = string.Empty;
            string employeeStatusFlag = string.Empty;
            string payrollName = string.Empty;
            string payrollPeriodType = string.Empty;
            long? esrLocationId = null;
            string supervisorFlag = string.Empty;
            long? supervisorPersonId = null;
            long? supervisorAssignmentId = null;
            string supervisorAssignmentNumber = string.Empty;
            long? departmentManagerPersonId = null;
            string employeeCategory = string.Empty;
            string assignmentCategory = string.Empty;
            bool primaryAssignment = false;
            string primaryAssignmentString = string.Empty;
            decimal normalHours = 0;
            string normalHoursFrequency = string.Empty;
            decimal gradeContractHours = 0;
            decimal fte = 0;
            string flexibleWorkingPattern = string.Empty;
            long? esrOrganisationId = null;
            long? esrPositionId = null;
            string positionName = string.Empty;
            string grade = string.Empty;
            string gradeStep = string.Empty;
            DateTime? startDateInGrade = null;
            decimal annualSalaryValue = 0;
            string jobName = string.Empty;
            string peopleGroup = string.Empty;
            string tAndAFlag = string.Empty;
            string assignmentNightWorkerAttribute = string.Empty;
            DateTime? projectedHireDate = null;
            int? vacancyId = null;
            DateTime? contractEndDate = null;
            DateTime? incrementDate = null;
            string maximumPartTimeFlag = string.Empty;
            string afcFlag = string.Empty;
            DateTime? esrLastUpdate = null;
            DateTime? lastWorkingDay = null;
            string eKsfFSpinalPoint = string.Empty;
            string managerFlag = string.Empty;
            DateTime? assignmentEndDate = null;

            #endregion record variable

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)AssignmentRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrAssignmentUpdateRecordType:
                    if (rec.Length != ((int)AssignmentRecordColumnsRef.Spare + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    long.TryParse(rec[(int)AssignmentRecordColumnsRef.PersonId], out esrPersonId);
                    long.TryParse(rec[(int)AssignmentRecordColumnsRef.AssignmentId], out assignmentId);
                    DateTime tmpDate;
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.EffectiveStartDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveStartDate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.EffectiveEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveEndDate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.EarliestAssignmentDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        earliestStartDate = tmpDate;
                    }
                    assignmentType = rec[(int)AssignmentRecordColumnsRef.AssignmentType];
                    assignmentNumber = rec[(int)AssignmentRecordColumnsRef.AssignmentNumber];
                    systemAssignmentStatus = rec[(int)AssignmentRecordColumnsRef.SystemAssignmentStatus];
                    assignmentStatus = rec[(int)AssignmentRecordColumnsRef.UserAssignmentStatus];
                    employeeStatusFlag = rec[(int)AssignmentRecordColumnsRef.EmployeeStatusFlag];
                    payrollName = rec[(int)AssignmentRecordColumnsRef.PayrollName];
                    payrollPeriodType = rec[(int)AssignmentRecordColumnsRef.PayrollPeriodType];
                    long tmpId;
                    if (long.TryParse(rec[(int)AssignmentRecordColumnsRef.AssignmentLocationId], out tmpId))
                    {
                        esrLocationId = tmpId;
                    }
                    supervisorFlag = rec[(int)AssignmentRecordColumnsRef.SupervisorFlag];
                    if (long.TryParse(rec[(int)AssignmentRecordColumnsRef.SupervisorPersonId], out tmpId))
                    {
                        supervisorPersonId = tmpId;
                    }
                    if (long.TryParse(rec[(int)AssignmentRecordColumnsRef.SupervisorAssignmentId], out tmpId))
                    {
                        supervisorAssignmentId = tmpId;
                    }
                    supervisorAssignmentNumber = rec[(int)AssignmentRecordColumnsRef.SupervisorAssignmentNumber];
                    if (long.TryParse(rec[(int)AssignmentRecordColumnsRef.DepartmentManagerPersonId], out tmpId))
                    {
                        departmentManagerPersonId = tmpId;
                    }
                    employeeCategory = rec[(int)AssignmentRecordColumnsRef.EmployeeCategory];
                    assignmentCategory = rec[(int)AssignmentRecordColumnsRef.AssignmentCategory];
                    primaryAssignmentString = rec[(int)AssignmentRecordColumnsRef.PrimaryAssignment];
                    primaryAssignment = primaryAssignmentString.ToUpper() == "YES";
                    decimal tmpDec;
                    if (decimal.TryParse(rec[(int)AssignmentRecordColumnsRef.NormalHours], out tmpDec))
                    {
                        normalHours = tmpDec;
                    }
                    normalHoursFrequency = rec[(int)AssignmentRecordColumnsRef.Frequency];
                    if (decimal.TryParse(rec[(int)AssignmentRecordColumnsRef.GradeContractHours], out tmpDec))
                    {
                        gradeContractHours = tmpDec;
                    }
                    if (decimal.TryParse(rec[(int)AssignmentRecordColumnsRef.Fte], out tmpDec))
                    {
                        fte = tmpDec;
                    }
                    flexibleWorkingPattern = rec[(int)AssignmentRecordColumnsRef.FlexibleWorkingPattern];
                    if (long.TryParse(rec[(int)AssignmentRecordColumnsRef.OrganisationId], out tmpId))
                    {
                        esrOrganisationId = tmpId;
                    }
                    if (long.TryParse(rec[(int)AssignmentRecordColumnsRef.PositionId], out tmpId))
                    {
                        esrPositionId = tmpId;
                    }
                    positionName = rec[(int)AssignmentRecordColumnsRef.PositionName];
                    grade = rec[(int)AssignmentRecordColumnsRef.Grade];
                    gradeStep = rec[(int)AssignmentRecordColumnsRef.GradeStep];
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.StartDateInGrade], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        startDateInGrade = tmpDate;
                    }
                    if (decimal.TryParse(rec[(int)AssignmentRecordColumnsRef.AnnualSalaryValue], out tmpDec))
                    {
                        annualSalaryValue = tmpDec;
                    }
                    jobName = rec[(int)AssignmentRecordColumnsRef.JobName];
                    peopleGroup = rec[(int)AssignmentRecordColumnsRef.PeopleGroup];
                    tAndAFlag = rec[(int)AssignmentRecordColumnsRef.TandAFlag];
                    assignmentNightWorkerAttribute = rec[(int)AssignmentRecordColumnsRef.NightWorkerOptOut];
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.ProjectedHireDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        projectedHireDate = tmpDate;
                    }
                    int tmpInt;
                    if (int.TryParse(rec[(int)AssignmentRecordColumnsRef.VacancyId], out tmpInt))
                    {
                        vacancyId = tmpInt;
                    }
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.ContractEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        contractEndDate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.IncrementDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        incrementDate = tmpDate;
                    }
                    maximumPartTimeFlag = rec[(int)AssignmentRecordColumnsRef.MaximumPartTimeFlag];
                    afcFlag = rec[(int)AssignmentRecordColumnsRef.AfcFlag];
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        esrLastUpdate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.LastWorkingDay], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        lastWorkingDay = tmpDate;
                    }
                    eKsfFSpinalPoint = rec[(int)AssignmentRecordColumnsRef.EksfSpinalPoint];
                    managerFlag = rec[(int)AssignmentRecordColumnsRef.ManagerFlag];
                    if (DateTime.TryParseExact(rec[(int)AssignmentRecordColumnsRef.AssignmentEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        assignmentEndDate = tmpDate;
                    }
                    break;
                case EsrRecordTypes.EsrAssignmentDeleteRecordType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out assignmentId);
                    break;
                default:
                    return null;
            }

            return new EsrAssignmentRecord(
                sysInternalEsrAssignId,
                esrPersonId,
                assignmentId,
                effectiveStartDate,
                effectiveEndDate,
                earliestStartDate,
                assignmentType,
                assignmentNumber,
                systemAssignmentStatus,
                assignmentStatus,
                employeeStatusFlag,
                payrollName,
                payrollPeriodType,
                esrLocationId,
                supervisorFlag,
                supervisorPersonId,
                supervisorAssignmentId,
                supervisorAssignmentNumber,
                departmentManagerPersonId,
                employeeCategory,
                assignmentCategory,
                primaryAssignment,
                primaryAssignmentString,
                normalHours,
                normalHoursFrequency,
                gradeContractHours,
                fte,
                flexibleWorkingPattern,
                esrOrganisationId,
                esrPositionId,
                positionName,
                grade,
                gradeStep,
                startDateInGrade,
                annualSalaryValue,
                jobName,
                peopleGroup,
                tAndAFlag,
                assignmentNightWorkerAttribute,
                projectedHireDate,
                vacancyId,
                contractEndDate,
                incrementDate,
                maximumPartTimeFlag,
                afcFlag,
                esrLastUpdate,
                lastWorkingDay,
                eKsfFSpinalPoint,
                managerFlag,
                assignmentEndDate)
                {
                    Action = recordAction,
                    SafeDepartmentManagerPersonId = departmentManagerPersonId,
                    SafeSupervisorAssignmentNumber = supervisorAssignmentNumber,
                    SafeSupervisorPersonId = supervisorPersonId,
                    SafeSupervisorAssignmentId = supervisorAssignmentId
                };
        }

        /// <summary>
        /// Reset the safe fields in the assignment record.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public static EsrAssignment ResetAssignmentRecord(EsrAssignment record)
        {
            if (record.DepartmentManagerPersonId == null)
            {
                record.DepartmentManagerPersonId = record.SafeDepartmentManagerPersonId;
            }

            if (record.SupervisorAssignmentNumber == null)
            {
                record.SupervisorAssignmentNumber = record.SafeSupervisorAssignmentNumber;
            }

            if (record.SupervisorPersonId == null)
            {
                record.SupervisorPersonId = record.SafeSupervisorPersonId;
            }

            if (record.SupervisorAssignmentId == null)
            {
                record.SupervisorAssignmentId = record.SafeSupervisorAssignmentId;
            }

            record.ActionResult = new ApiResult();
            return record;
        }
    }
}
