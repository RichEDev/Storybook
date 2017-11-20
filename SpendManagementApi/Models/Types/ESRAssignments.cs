
namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Runtime.Serialization;

    using SpendManagementApi.Interfaces;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// ESR assignments class.
    /// </summary>
    public class ESRAssignments : BaseExternalType, IApiFrontForDbObject<cESRAssignment, ESRAssignments>
    {
        /// <summary>
        /// The unique Id for this ESR object.
        /// </summary>
        public long Assignmentid { get; set; }


        /// <summary>
        /// The internal assignmentId 
        /// </summary>
        public int SysInternalAssignmentId;

        /// <summary>
        /// The internal ESR assignment identifier.
        /// </summary>
        public long EsrAssignId { get; set; }

        /// <summary>
        /// Gets or sets the assignment number assigned to employee.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the creator of the assignment.
        /// </summary>
        [IgnoreDataMember]
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether assignment active or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the earliest assignment start date.
        /// </summary>
        public DateTime? Earliestassignmentstartdate { get; set; }

        /// <summary>
        /// Gets or sets the final assignment end date.
        /// </summary>
        public DateTime? Finalassignmentenddate { get; set; }

        /// <summary>
        /// Gets or sets the assignment status assigne to employee.
        /// </summary>
        public ESRAssignmentStatus Assignmentstatus { get; set; }

        /// <summary>
        /// Gets or sets the assignment payroll paytype.
        /// </summary>
        public string Payrollpaytype { get; set; }

        /// <summary>
        /// Gets or sets the assignment payroll name.
        /// </summary>
        public string Payrollname { get; set; }

        /// <summary>
        /// Gets or sets the assignment payroll periodtype.
        /// </summary>
        public string Payrollperiodtype { get; set; }

        /// <summary>
        /// Gets or sets the assignment address 1.
        /// </summary>
        public string Assignmentaddress1 { get; set; }

        /// <summary>
        /// Gets or sets the assignment address 2.
        /// </summary>
        public string Assignmentaddress2 { get; set; }

        /// <summary>
        /// Gets or sets the assignment address town.
        /// </summary>
        public string Assignmentaddresstown { get; set; }

        /// <summary>
        /// Gets or sets the assignment address country.
        /// </summary>
        public string Assignmentaddresscountry { get; set; }

        /// <summary>
        /// Gets or sets the assignment address postcode.
        /// </summary>
        public string Assignmentaddresspostcode { get; set; }

        /// <summary>
        /// Gets or sets the assignment address county.
        /// </summary>
        public string Assignmentaddresscounty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether supervisor flag.
        /// </summary>
        public bool Supervisorflag { get; set; }

        /// <summary>
        /// Gets or sets the supervisor assignmentnumber.
        /// </summary>
        public string Supervisorassignmentnumber { get; set; }

        /// <summary>
        /// Gets or sets the supervisor employement number.
        /// </summary>
        public string Supervisoremployementnumber { get; set; }

        /// <summary>
        /// Gets or sets the supervisor fullname.
        /// </summary>
        public string Supervisorfullname { get; set; }

        /// <summary>
        /// Gets or sets the assignment accrual plan.
        /// </summary>
        public string Accrualplan { get; set; }

        /// <summary>
        /// Gets or sets the employee category.
        /// </summary>
        public string Employeecategory { get; set; }

        /// <summary>
        /// Gets or sets the assignment category.
        /// </summary>
        public string Assignmentcategory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether primary assignment.
        /// </summary>
        public bool Primaryassignment { get; set; }

        /// <summary>
        /// Gets or sets the esr primary assignment value.
        /// </summary>
        public string Esrprimaryassignmentstring { get; set; }

        /// <summary>
        /// Gets or sets the assignment normal hours.
        /// </summary>
        public decimal Normalhours { get; set; }

        /// <summary>
        /// Gets or sets the assignment normal hours frequency.
        /// </summary>
        public string Normalhoursfrequency { get; set; }

        /// <summary>
        /// Gets or sets the assignment grade contract hours.
        /// </summary>
        public decimal Gradecontracthours { get; set; }

        /// <summary>
        /// Gets or sets the assignment noof sessions.
        /// </summary>
        public decimal Noofsessions { get; set; }

        /// <summary>
        /// Gets or sets the assignment sessions frequency.
        /// </summary>
        public string Sessionsfrequency { get; set; }

        /// <summary>
        /// Gets or sets the assignment work pattern details.
        /// </summary>
        public string Workpatterndetails { get; set; }

        /// <summary>
        /// Gets or sets the assignment work pattern start day.
        /// </summary>
        public string Workpatternstartday { get; set; }

        /// <summary>
        /// Gets or sets the flexible working pattern.
        /// </summary>
        public string Flexibleworkingpattern { get; set; }

        /// <summary>
        /// Gets or sets the availability schedule.
        /// </summary>
        public string Availabilityschedule { get; set; }

        /// <summary>
        /// Gets or sets the organisation.
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// Gets or sets the legal entity.
        /// </summary>
        public string Legalentity { get; set; }

        /// <summary>
        /// Gets or sets the position name.
        /// </summary>
        public string Positionname { get; set; }

        /// <summary>
        /// Gets or sets the job role.
        /// </summary>
        public string Jobrole { get; set; }

        /// <summary>
        /// Gets or sets the occupation code.
        /// </summary>
        public string Occupationcode { get; set; }

        /// <summary>
        /// Gets or sets the assignment location.
        /// </summary>
        public string Assignmentlocation { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the job name.
        /// </summary>
        public string Jobname { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the tanda flag.
        /// </summary>
        public string Tandaflag { get; set; }

        /// <summary>
        /// Gets or sets the night worker optout.
        /// </summary>
        public string Nightworkeroptout { get; set; }

        /// <summary>
        /// Gets or sets the projected hire date.
        /// </summary>
        public DateTime? Projectedhiredate { get; set; }

        /// <summary>
        /// Gets or sets the vacancyid.
        /// </summary>
        public int? Vacancyid { get; set; }

        /// <summary>
        /// Gets or sets the esr location id.
        /// </summary>
        public long? EsrLocationId { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        [IgnoreDataMember]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the modified on.
        /// </summary>
        [IgnoreDataMember]
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [IgnoreDataMember]
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the effective start date.
        /// </summary>
        public DateTime? EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective end date.
        /// </summary>
        public DateTime? EffectiveEndDate { get; set; }

        /// <summary>
        /// The signoff owner
        /// </summary>
        public IOwnership SignOffOwner { get; set; }

        /// <summary>
        /// Gets or sets the number of sessions.
        /// </summary>
        public decimal NumberOfSessions { get; set; }

        /// <summary>
        /// Gets or sets The employeeId the assignment is assoicated with.
        /// </summary>
        public int EmployeeId { get; set; }
      
        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ESRAssignments From(cESRAssignment dbType, IActionContext actionContext)
        {
            Assignmentid = dbType.assignmentid;
            EsrAssignId = dbType.sysinternalassignmentid;
            AssignmentNumber = dbType.assignmentnumber;
            CreatedBy = dbType.CreatedBy;
            Active = dbType.active;
            Earliestassignmentstartdate = dbType.earliestassignmentstartdate;
            Finalassignmentenddate = dbType.finalassignmentenddate;
            Payrollpaytype = dbType.payrollpaytype;
            Payrollname = dbType.payrollname;
            Payrollperiodtype = dbType.payrollperiodtype;
            Assignmentaddress1 = dbType.assignmentaddress1;
            Assignmentaddress2 = dbType.assignmentaddress2;
            Assignmentaddresscountry = dbType.assignmentaddresscountry;
            Assignmentaddresscounty = dbType.assignmentaddresscounty;
            Assignmentaddresspostcode = dbType.assignmentaddresspostcode;
            Assignmentaddresscounty = dbType.assignmentaddresscounty;
            Supervisorflag = dbType.supervisorflag;
            Supervisorassignmentnumber = dbType.supervisorassignmentnumber;
            Supervisoremployementnumber = dbType.supervisoremployementnumber;
            Supervisorfullname = dbType.supervisorfullname;
            Accrualplan = dbType.accrualplan;
            Employeecategory = dbType.employeecategory;
            Assignmentcategory = dbType.assignmentcategory;
            Primaryassignment = dbType.primaryassignment;
            Esrprimaryassignmentstring = dbType.esrprimaryassignmentstring;
            Normalhours = dbType.normalhours;
            Normalhoursfrequency = dbType.normalhoursfrequency;
            Gradecontracthours = dbType.gradecontracthours;
            Noofsessions = dbType.noofsessions;
            Sessionsfrequency = dbType.sessionsfrequency;
            Workpatterndetails = dbType.workpatterndetails;
            Workpatternstartday = dbType.workpatternstartday;
            Flexibleworkingpattern = dbType.flexibleworkingpattern;
            Availabilityschedule = dbType.availabilityschedule;
            Organisation = dbType.organisation;
            Legalentity = dbType.legalentity;
            Positionname = dbType.positionname;
            Jobrole = dbType.jobrole;
            Occupationcode = dbType.occupationcode;
            Assignmentlocation = dbType.assignmentlocation;
            Grade = dbType.grade;
            Jobname = dbType.jobname;
            Group = dbType.group;
            Tandaflag = dbType.tandaflag;
            Nightworkeroptout = dbType.nightworkeroptout;
            Projectedhiredate = dbType.projectedhiredate;
            Vacancyid = dbType.vacancyid;
            EsrLocationId = dbType.esrLocationId;
            Active = dbType.active;
            CreatedOn = dbType.CreatedOn;
            CreatedBy = dbType.CreatedBy;
            ModifiedBy = dbType.ModifiedBy;
            EffectiveStartDate = dbType.EffectiveStartDate;
            EffectiveEndDate = dbType.EffectiveEndDate;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cESRAssignment To(IActionContext actionContext)
        {
            return new cESRAssignment(
                Assignmentid,
                SysInternalAssignmentId,
                this.AssignmentNumber,
                this.Earliestassignmentstartdate.Value,
                this.Finalassignmentenddate,
                this.Assignmentstatus,
                this.Payrollpaytype,
                this.Payrollname,
                this.Payrollperiodtype,
                this.Assignmentaddress1,
                this.Assignmentaddress2,
                this.Assignmentaddresstown,
                this.Assignmentaddresscounty,
                this.Assignmentaddresspostcode,
                this.Assignmentaddresscountry,
                this.Supervisorflag,
                this.Supervisorassignmentnumber,
                this.Supervisoremployementnumber,
                this.Supervisorfullname,
                this.Accrualplan,
                this.Employeecategory,
                this.Assignmentcategory,
                this.Primaryassignment,
                this.Esrprimaryassignmentstring,
                this.Normalhours,
                this.Normalhoursfrequency,
                this.Gradecontracthours,
                this.NumberOfSessions,
                this.Sessionsfrequency,
                this.Workpatterndetails,
                this.Workpatternstartday,
                this.Flexibleworkingpattern,
                this.Availabilityschedule,
                this.Organisation,
                this.Legalentity,
                this.Positionname,
                this.Jobrole,
                this.Occupationcode,
                this.Assignmentlocation,
                this.Grade,
                this.Jobname,
                this.Group,
                this.Tandaflag,
                this.Nightworkeroptout,
                this.Projectedhiredate,
                this.Vacancyid,
                this.EsrLocationId,
                this.Active,
                this.SignOffOwner,
                this.CreatedOn,
                this.CreatedBy,
                this.ModifiedOn,
                this.ModifiedBy,
                this.EffectiveStartDate,
                this.EffectiveEndDate);
        }
    }
}