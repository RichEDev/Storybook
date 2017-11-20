using System.IO;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Utilities;

namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using SpendManagementLibrary;
    using Spend_Management;

    /// <summary>
    /// Represents all the details of an NHS Trust (unit / company / area).
    /// </summary>
    public class NhsDetails : IEquatable<NhsDetails>, IRequiresValidation
    {
        /// <summary>
        /// The unique trust Id. 
        /// You can find Trusts by looking at the NhsTrusts resource.
        /// </summary>
        public int? TrustId { get; set; }

        /// <summary>
        /// A list of ESR assignment records for this NHS trust.
        /// </summary>
        //public List<EsrAssignment> EsrAssignments { get; set; }

        /// <summary>
        /// The NHS Unique Id for this NHS trust.
        /// </summary>
        public string NhsUniqueId { get; set; }
        
        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="actionContext"></param>
        public void Validate(IActionContext actionContext)
        {
            if (TrustId.HasValue)
            {
                var trusts = actionContext.NhsTrusts;
                var trust = trusts.GetESRTrustByID(TrustId.Value);

                if (trust == null)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorNhsTrustNonExistent);
                }

                if (trust.Archived)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorNhsTrustArchived);
                }
            }
        }

        internal static NhsDetails Merge(NhsDetails dataToUpdate, NhsDetails existingData)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new NhsDetails
                                   {
                                       //EsrAssignments = existingData.EsrAssignments,
                                       NhsUniqueId = existingData.NhsUniqueId,
                                       TrustId = existingData.TrustId
                                   };
            }

            return dataToUpdate;
        }

        public bool Equals(NhsDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return this.NhsUniqueId.Equals(other.NhsUniqueId)
                // && this.EsrAssignments.SequenceEqual(other.EsrAssignments)
                   && this.TrustId.Equals(other.TrustId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NhsDetails);
        }

    }

    /// <summary>
    /// Represents an assigment of an Electronic Staff Record.
    /// </summary>
    public class EsrAssignment : BaseExternalType, IEquatable<EsrAssignment>
    {
        /// <summary>
        /// The Assignment Id.
        /// </summary>
        public long AssignmentId { get; set; }

        /// <summary>
        /// The ESR assignment Id.
        /// </summary>
        internal int EsrAssignmentId { get; set; }
        
        /// <summary>
        /// The assignment number.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Whether the ESRAssignment is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether this is the primary assignment.
        /// </summary>
        public bool IsPrimaryAssignment { get; set; }

        /// <summary>
        /// The effective start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The effective end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        public int? SupervisorEmployeeId { get; set; }

        #region Internal Properties
        internal bool SupervisorFlag { get; set; }
        internal string SupervisorAssignmentNumber { get; set; }
        internal string SupervisorEmployeeNumber { get; set; }
        internal string SupervisorFullName { get; set; }

        #endregion


        public bool Equals(EsrAssignment other)
        {
            if (other == null)
            {
                return false;
            }
            return this.AssignmentNumber.Equals(other.AssignmentNumber)
                   && this.DateCompare(this.EndDate, other.EndDate)
                   && this.DateCompare(this.StartDate, other.StartDate)
                   && this.IsActive.Equals(other.IsActive)
                   && this.IsPrimaryAssignment.Equals(other.IsPrimaryAssignment);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EsrAssignment);
        }

        public int EmployeeId { get; set; }
    }




    internal static class EsrAssignmentConversion
    {
        internal static TResult Cast<TResult>(this cESRAssignment originalDbAssignment, int accountId)
            where TResult : EsrAssignment, new()
        {
            var esrAssignment = new TResult
                    {
                        AssignmentId = originalDbAssignment.assignmentid,
                        AssignmentNumber = originalDbAssignment.assignmentnumber,
                        EndDate = originalDbAssignment.finalassignmentenddate,
                        StartDate = originalDbAssignment.earliestassignmentstartdate,
                        IsActive = originalDbAssignment.active,
                        IsPrimaryAssignment = originalDbAssignment.primaryassignment,
                        SupervisorEmployeeId = GetSupervisorId(originalDbAssignment.supervisorassignmentnumber, accountId),
                        SupervisorEmployeeNumber = originalDbAssignment.supervisoremployementnumber,
                        SupervisorAssignmentNumber = originalDbAssignment.supervisorassignmentnumber,
                        SupervisorFlag = originalDbAssignment.supervisorflag,
                        SupervisorFullName = originalDbAssignment.supervisorfullname
                    };
            return esrAssignment;
        }

        private static int? GetSupervisorId(string supervisorAssignmentNumber, int accountId)
        {
            if (!string.IsNullOrEmpty(supervisorAssignmentNumber))
            {
                cEmployees employees = new cEmployees(accountId);
                SpendManagementLibrary.Employees.Employee supervisorEmployee =
                    employees.GetEmployeeById(
                        employees.getEmployeeidByAssignmentNumber(accountId, supervisorAssignmentNumber));

                if (supervisorEmployee == null)
                {
                    return null;
                }

                return supervisorEmployee.EmployeeID;
            }

            return null;
        }

        internal static cESRAssignment Cast<TResult>(this EsrAssignment assignment, cESRAssignment oldAssignment)
        {
            if (oldAssignment != null)
            {
                return new cESRAssignment(oldAssignment.assignmentid,
                    assignment.EsrAssignmentId,
                    assignment.AssignmentNumber,
                    assignment.StartDate,
                    assignment.EndDate.Value,
                    oldAssignment.assignmentstatus,
                    oldAssignment.payrollpaytype,
                    oldAssignment.payrollname,
                    oldAssignment.payrollpaytype,
                    oldAssignment.assignmentaddress1,
                    oldAssignment.assignmentaddress2,
                    oldAssignment.assignmentaddresstown,
                    oldAssignment.assignmentaddresscounty,
                    oldAssignment.assignmentaddresspostcode,
                    oldAssignment.assignmentaddresscountry,
                    oldAssignment.supervisorflag,
                    assignment.SupervisorAssignmentNumber,
                    assignment.SupervisorEmployeeNumber,
                    assignment.SupervisorFullName,
                    oldAssignment.accrualplan,
                    oldAssignment.employeecategory,
                    oldAssignment.assignmentcategory,
                    assignment.IsPrimaryAssignment,
                    (assignment.IsPrimaryAssignment ? "Yes" : "No"),
                    oldAssignment.normalhours,
                    oldAssignment.normalhoursfrequency,
                    oldAssignment.gradecontracthours,
                    oldAssignment.noofsessions,
                    oldAssignment.sessionsfrequency,
                    oldAssignment.workpatterndetails,
                    oldAssignment.workpatternstartday,
                    oldAssignment.flexibleworkingpattern,
                    oldAssignment.availabilityschedule,
                    oldAssignment.organisation,
                    oldAssignment.legalentity,
                    oldAssignment.positionname,
                    oldAssignment.jobname,
                    oldAssignment.occupationcode,
                    oldAssignment.assignmentlocation,
                    oldAssignment.grade, oldAssignment.jobname,
                    oldAssignment.group, oldAssignment.tandaflag,
                    oldAssignment.nightworkeroptout,
                    oldAssignment.projectedhiredate,
                    oldAssignment.vacancyid,
                    oldAssignment.esrLocationId,
                    assignment.IsActive,
                    oldAssignment.SignOffOwner,
                    oldAssignment.CreatedOn,
                    oldAssignment.CreatedBy,
                    DateTime.UtcNow,
                    assignment.EmployeeId);
            }
            return new cESRAssignment(0,
                0,
                assignment.AssignmentNumber,
                assignment.StartDate,
                assignment.EndDate,
                ESRAssignmentStatus.NotSpecified,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                false,
                assignment.SupervisorAssignmentNumber,
                assignment.SupervisorEmployeeNumber,
                assignment.SupervisorFullName,
                string.Empty,
                string.Empty,
                string.Empty,
                assignment.IsPrimaryAssignment,
                (assignment.IsPrimaryAssignment ? "Yes" : "No"),
                0,
                string.Empty,
                0,
                0,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                assignment.IsActive,
                null,
                DateTime.UtcNow,
                assignment.EmployeeId,
                null,
                null);
        }
    }
}