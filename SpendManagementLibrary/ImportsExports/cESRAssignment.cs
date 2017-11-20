using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary
{
    using System.Data.SqlClient;

    /// <summary>
    /// ESRAssignmentStatus enumeration
    /// </summary>
    public enum ESRAssignmentStatus
    {
        /// <summary>
        /// Not Specified
        /// </summary>
        NotSpecified = 0,
        /// <summary>
        /// Acting Up
        /// </summary>
        ActingUp,
        /// <summary>
        /// Active Assignment
        /// </summary>
        ActiveAssignment,
        /// <summary>
        /// Assignment Costing Deletion
        /// </summary>
        AssignmentCostingDeletion,
        /// <summary>
        /// Career Break
        /// </summary>
        CareerBreak,
        /// <summary>
        /// Internal Secondment
        /// </summary>
        InternalSecondment,
        /// <summary>
        /// Maternity Leave
        /// </summary>
        Maternity,
        /// <summary>
        /// Out on External Secondment (Paid)
        /// </summary>
        OutOnExternalSecondment_Paid,
        /// <summary>
        /// Out on External Secondment (Unpaid)
        /// </summary>
        OutOnExternalSecondment_Unpaid,
        /// <summary>
        /// Suspend Assignment
        /// </summary>
        SuspendAssignment,
        /// <summary>
        /// Suspend No Pay
        /// </summary>
        SuspendNoPay,
        /// <summary>
        /// Suspend with Pay
        /// </summary>
        SuspendWithPay,
        /// <summary>
        /// Terminator
        /// </summary>
        Terminator,
        /// <summary>
        /// Offer Accepted
        /// </summary>
        OfferAccepted,
        InactiveNotWorked
    }

    /// <summary>
    /// cESRAssignment class
    /// </summary>
    [Serializable]
    public class cESRAssignment
    {
        private long nAssignmentId; // db unique id
        private int nSysInternalAssignmentID;
        private string sAssignmentNumber;
        private DateTime dtEarliestAssignmentStartDate;
        private DateTime? dtFinalAssigmentEndDate;
        private ESRAssignmentStatus eAssignmentStatus;
        private string sPayrollPayType;
        private string sPayrollName;
        private string sPayrollPeriodType;
        private string sAssignmentAddressLine1;
        private string sAssignmentAddressLine2;
        private string sAssignmentAddressTown;
        private string sAssignmentAddressCounty;
        private string sAssignmentAddressPostcode;
        private string sAssignmentAdressCountry;
        private bool bSupervisorFlag;
        private string sSupervisorAssignmentNumber;
        private string sSupervisorEmployeeNumber;
        private string sSupervisorFullName;
        private string sAccrualPlan;
        private string sEmployeeCategory;
        private string sAssignmentCategory;
        private bool bPrimaryAssignment;
        private string sESRPrimaryAssignmentString;
        private decimal dNormalHours;
        private string sNormalHoursFrequency;
        private decimal dGradeContractHours;
        private decimal dNoOfSessions;
        private string sSessionsFrequency;
        private string sWorkPatternDetails;
        private string sWorkPatternStartDay;
        private string sFlexibleWorkingPattern;
        private string sAvailabilitySchedule;
        private string sOrganisation;
        private string sLegalEntity;
        private string sPositionName;
        private string sJobRole;
        private string sOccupationCode;
        private string sAssignmentLocation;
        private string sGrade;
        private string sJobName;
        private string sGroup;
        private string sTAndAFlag;
        private string sNightWorkerOptOut;
        private DateTime? dtProjectedHireDate;
        private int? nVacancyID;
        private long? _ESRLocationID;
        private bool bActive;
        private DateTime? dtCreatedOn;
        private int? nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private DateTime? effectiveStartDate;
        private DateTime? effectiveEndDate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentid"></param>
        /// <param name="sysinternalassignmentid"></param>
        /// <param name="assignmentnumber"></param>
        /// <param name="earliestassignementstartdate"></param>
        /// <param name="finalassignmentenddate"></param>
        /// <param name="assignmentstatus"></param>
        /// <param name="payrollpaytype"></param>
        /// <param name="payrollname"></param>
        /// <param name="payrollperiodtype"></param>
        /// <param name="assignmentaddressline1"></param>
        /// <param name="assignmentaddressline2"></param>
        /// <param name="assignmentaddresstown"></param>
        /// <param name="assignmentaddresscounty"></param>
        /// <param name="assignmentaddresspostcode"></param>
        /// <param name="assignmentaddresscountry"></param>
        /// <param name="supervisorflag"></param>
        /// <param name="supervisorassignmentnumber"></param>
        /// <param name="supervisoremployeenumber"></param>
        /// <param name="supervisorfullname"></param>
        /// <param name="accrualplan"></param>
        /// <param name="employeecategory"></param>
        /// <param name="assignmentcategory"></param>
        /// <param name="primaryassignment"></param>
        /// <param name="esrprimaryassignmentstring"></param>
        /// <param name="normalhours"></param>
        /// <param name="normalhoursfrequency"></param>
        /// <param name="gradecontracthours"></param>
        /// <param name="numberofsessions"></param>
        /// <param name="sessionsfrequency"></param>
        /// <param name="workpatterndetails"></param>
        /// <param name="workpatterstartday"></param>
        /// <param name="flexibleworkingpattern"></param>
        /// <param name="availabilityschedule"></param>
        /// <param name="organisation"></param>
        /// <param name="legalentity"></param>
        /// <param name="positionname"></param>
        /// <param name="jobrole"></param>
        /// <param name="occupationcode"></param>
        /// <param name="assignmentlocation"></param>
        /// <param name="grade"></param>
        /// <param name="jobname"></param>
        /// <param name="group"></param>
        /// <param name="tandaflag"></param>
        /// <param name="nightworkeroptout"></param>
        /// <param name="projectedhiredate"></param>
        /// <param name="vacancyid"></param>
        /// <param name="esrLocationId"></param>
        /// <param name="active"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        public cESRAssignment(long assignmentid, int sysinternalassignmentid, string assignmentnumber, DateTime earliestassignementstartdate, DateTime? finalassignmentenddate, ESRAssignmentStatus assignmentstatus, string payrollpaytype, string payrollname, string payrollperiodtype, string assignmentaddressline1, string assignmentaddressline2, string assignmentaddresstown, string assignmentaddresscounty, string assignmentaddresspostcode, string assignmentaddresscountry, bool supervisorflag, string supervisorassignmentnumber, string supervisoremployeenumber, string supervisorfullname, string accrualplan, string employeecategory, string assignmentcategory, bool primaryassignment, string esrprimaryassignmentstring, decimal normalhours, string normalhoursfrequency, decimal gradecontracthours, decimal numberofsessions, string sessionsfrequency, string workpatterndetails, string workpatterstartday, string flexibleworkingpattern, string availabilityschedule, string organisation, string legalentity, string positionname, string jobrole, string occupationcode, string assignmentlocation, string grade, string jobname, string group, string tandaflag, string nightworkeroptout, DateTime? projectedhiredate, int? vacancyid, long? esrLocationId, bool active, IOwnership signOffOwner, DateTime? createdon, int? createdby, DateTime? modifiedon, int? modifiedby, DateTime? effectiveStartDate = null, DateTime? effectiveEndDate = null)
        {
            nAssignmentId = assignmentid;
            nSysInternalAssignmentID = sysinternalassignmentid;
            sAssignmentNumber = assignmentnumber;
            dtEarliestAssignmentStartDate = earliestassignementstartdate;
            dtFinalAssigmentEndDate = finalassignmentenddate;
            eAssignmentStatus = assignmentstatus;
            sPayrollPayType = payrollpaytype;
            sPayrollName = payrollname;
            sPayrollPeriodType = payrollperiodtype;
            sAssignmentAddressLine1 = assignmentaddressline1;
            sAssignmentAddressLine2 = assignmentaddressline2;
            sAssignmentAddressTown = assignmentaddresstown;
            sAssignmentAddressCounty = assignmentaddresscounty;
            sAssignmentAddressPostcode = assignmentaddresspostcode;
            sAssignmentAdressCountry = assignmentaddresscountry;
            bSupervisorFlag = supervisorflag;
            sSupervisorAssignmentNumber = supervisorassignmentnumber;
            sSupervisorEmployeeNumber = supervisoremployeenumber;
            sSupervisorFullName = supervisorfullname;
            sAccrualPlan = accrualplan;
            sEmployeeCategory = employeecategory;
            sAssignmentCategory = assignmentcategory;
            bPrimaryAssignment = primaryassignment;
            sESRPrimaryAssignmentString = esrprimaryassignmentstring;
            dNormalHours = normalhours;
            sNormalHoursFrequency = normalhoursfrequency;
            dGradeContractHours = gradecontracthours;
            dNoOfSessions = numberofsessions;
            sSessionsFrequency = sessionsfrequency;
            sWorkPatternDetails = workpatterndetails;
            sWorkPatternStartDay = workpatterstartday;
            sFlexibleWorkingPattern = flexibleworkingpattern;
            sAvailabilitySchedule = availabilityschedule;
            sOrganisation = organisation;
            sLegalEntity = legalentity;
            sPositionName = positionname;
            sJobRole = jobrole;
            sOccupationCode = occupationcode;
            sAssignmentLocation = assignmentlocation;
            sGrade = grade;
            sJobName = jobname;
            sGroup = group;
            sTAndAFlag = tandaflag;
            sNightWorkerOptOut = nightworkeroptout;
            dtProjectedHireDate = projectedhiredate;
            nVacancyID = vacancyid;
            bActive = active;
            this._ESRLocationID = esrLocationId;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            this.effectiveEndDate = effectiveEndDate;
            this.effectiveStartDate = effectiveStartDate;
            this.SignOffOwner = signOffOwner;
        }

        #region properties
        /// <summary>
        /// assignmentid
        /// </summary>
        public long assignmentid
        {
            get { return nAssignmentId; }
        }
        /// <summary>
        /// esrAssignID
        /// </summary>
        public int sysinternalassignmentid
        {
            get { return nSysInternalAssignmentID; }
        }
        /// <summary>
        /// assignmentnumber
        /// </summary>
        public string assignmentnumber
        {
            get { return sAssignmentNumber; }
        }
        /// <summary>
        /// earliestassignmentstartdate
        /// </summary>
        public DateTime earliestassignmentstartdate
        {
            get { return dtEarliestAssignmentStartDate; }
        }
        /// <summary>
        /// finalassignmentenddate
        /// </summary>
        public DateTime? finalassignmentenddate
        {
            get { return dtFinalAssigmentEndDate; }
        }
        /// <summary>
        /// assignmentstatus
        /// </summary>
        public ESRAssignmentStatus assignmentstatus
        {
            get { return eAssignmentStatus; }
        }
        /// <summary>
        /// payrollpaytype
        /// </summary>
        public string payrollpaytype
        {
            get { return sPayrollPayType; }
        }
        /// <summary>
        /// payrollname
        /// </summary>
        public string payrollname
        {
            get { return sPayrollName; }
        }
        /// <summary>
        /// payrollperiodtype
        /// </summary>
        public string payrollperiodtype
        {
            get { return sPayrollPeriodType; }
        }
        /// <summary>
        /// assignmentaddress1
        /// </summary>
        public string assignmentaddress1
        {
            get { return sAssignmentAddressLine1; }
        }
        /// <summary>
        /// assignmentaddress2
        /// </summary>
        public string assignmentaddress2
        {
            get { return sAssignmentAddressLine2; }
        }
        /// <summary>
        /// assignmentaddresstown
        /// </summary>
        public string assignmentaddresstown
        {
            get { return sAssignmentAddressTown; }
        }
        /// <summary>
        /// assignmentaddresscounty
        /// </summary>
        public string assignmentaddresscounty
        {
            get { return sAssignmentAddressCounty; }
        }
        /// <summary>
        /// assignmentaddresspostcode
        /// </summary>
        public string assignmentaddresspostcode
        {
            get { return sAssignmentAddressPostcode; }
        }
        /// <summary>
        /// assignmentaddresscountry
        /// </summary>
        public string assignmentaddresscountry
        {
            get { return sAssignmentAdressCountry; }
        }
        /// <summary>
        /// supervisorflag
        /// </summary>
        public bool supervisorflag
        {
            get { return bSupervisorFlag; }
        }
        /// <summary>
        /// supervisorassignmentnumber
        /// </summary>
        public string supervisorassignmentnumber
        {
            get { return sSupervisorAssignmentNumber; }
        }
        /// <summary>
        /// supervisoremployementnumber
        /// </summary>
        public string supervisoremployementnumber
        {
            get { return sSupervisorEmployeeNumber; }
        }
        /// <summary>
        /// supervisorfullname
        /// </summary>
        public string supervisorfullname
        {
            get { return sSupervisorFullName; }
        }
        /// <summary>
        /// accrualplan
        /// </summary>
        public string accrualplan
        {
            get { return sAccrualPlan; }
        }
        /// <summary>
        /// employeecategory
        /// </summary>
        public string employeecategory
        {
            get { return sEmployeeCategory; }
        }
        /// <summary>
        /// assignmentcategory
        /// </summary>
        public string assignmentcategory
        {
            get { return sAssignmentCategory; }
        }
        /// <summary>
        /// primaryassignment
        /// </summary>
        public bool primaryassignment
        {
            get { return bPrimaryAssignment; }
        }
        /// <summary>
        /// primary assignment definition from the ESRAssignment record
        /// </summary>
        public string esrprimaryassignmentstring
        {
            get
            {
                return sESRPrimaryAssignmentString;
            }
        }
        /// <summary>
        /// normalhours
        /// </summary>
        public decimal normalhours
        {
            get { return dNormalHours; }
        }
        /// <summary>
        /// normalhoursfrequency
        /// </summary>
        public string normalhoursfrequency
        {
            get { return sNormalHoursFrequency; }
        }
        /// <summary>
        /// gradecontracthours
        /// </summary>
        public decimal gradecontracthours
        {
            get { return dGradeContractHours; }
        }
        /// <summary>
        /// noofsessions
        /// </summary>
        public decimal noofsessions
        {
            get { return dNoOfSessions; }
        }
        /// <summary>
        /// sessionsfrequency
        /// </summary>
        public string sessionsfrequency
        {
            get { return sSessionsFrequency; }
        }
        /// <summary>
        /// workpatterndetails
        /// </summary>
        public string workpatterndetails
        {
            get { return sWorkPatternDetails; }
        }
        /// <summary>
        /// workpatternstartday
        /// </summary>
        public string workpatternstartday
        {
            get { return sWorkPatternStartDay; }
        }
        /// <summary>
        /// flexibleworkingpattern
        /// </summary>
        public string flexibleworkingpattern
        {
            get { return sFlexibleWorkingPattern; }
        }
        /// <summary>
        /// availabilityschedule
        /// </summary>
        public string availabilityschedule
        {
            get { return sAvailabilitySchedule; }
        }
        /// <summary>
        /// organisation
        /// </summary>
        public string organisation
        {
            get { return sOrganisation; }
        }
        /// <summary>
        /// legalentity
        /// </summary>
        public string legalentity
        {
            get { return sLegalEntity; }
        }
        /// <summary>
        /// positionname
        /// </summary>
        public string positionname
        {
            get { return sPositionName; }
        }
        /// <summary>
        /// jobrole
        /// </summary>
        public string jobrole
        {
            get { return sJobRole; }
        }
        /// <summary>
        /// occupationcode
        /// </summary>
        public string occupationcode
        {
            get { return sOccupationCode; }
        }
        /// <summary>
        /// assignmentlocation
        /// </summary>
        public string assignmentlocation
        {
            get { return sAssignmentLocation; }
        }
        /// <summary>
        /// grade
        /// </summary>
        public string grade
        {
            get { return sGrade; }
        }
        /// <summary>
        /// jobname
        /// </summary>
        public string jobname
        {
            get { return sJobName; }
        }
        /// <summary>
        /// group
        /// </summary>
        public string group
        {
            get { return sGroup; }
        }
        /// <summary>
        /// tandaflag
        /// </summary>
        public string tandaflag
        {
            get { return sTAndAFlag; }
        }
        /// <summary>
        /// nightworkeroptout
        /// </summary>
        public string nightworkeroptout
        {
            get { return sNightWorkerOptOut; }
        }
        /// <summary>
        /// projectedhiredate
        /// </summary>
        public DateTime? projectedhiredate
        {
            get { return dtProjectedHireDate; }
        }
        /// <summary>
        /// vacancyid
        /// </summary>
        public int? vacancyid
        {
            get { return nVacancyID; }
        }
        
        /// <summary>
        /// ESR Location ID
        /// </summary>
        public long? esrLocationId
        {
            get { return this._ESRLocationID; }
        }

        /// <summary>
        /// Is Assignment Active
        /// </summary>
        public bool active
        {
            get { return bActive; }
        }
        /// <summary>
        /// Date Assignment record created
        /// </summary>
        public DateTime? CreatedOn
        {
            get { return dtCreatedOn; }
        }
        /// <summary>
        /// Gets the employee who created the assignment (null if added automatically via import)
        /// </summary>
        public int? CreatedBy
        {
            get { return nCreatedBy; }
        }
        /// <summary>
        /// Gets the date the assignment was last modified
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
        }
        /// <summary>
        /// Gets the employee who last modified the assignment (null if modified automatically via import)
        /// </summary>
        public int? ModifiedBy
        {
            get { return nModifiedBy; }
        }
        /// <summary>
        /// Gets the Effective start date which is popultes by ESR go2
        /// </summary>
        public DateTime? EffectiveStartDate
        {
            get { return this.effectiveStartDate; }
        }

        /// <summary>
        /// Gets the Effective end date which is popultes by ESR go2
        /// </summary>
        public DateTime? EffectiveEndDate
        {
            get { return this.effectiveEndDate; }
        }

        public IOwnership SignOffOwner { get; private set; }

        #endregion

    }
}
