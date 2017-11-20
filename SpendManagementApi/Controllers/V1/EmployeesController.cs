namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="Employee">Employees</see>.
    /// </summary>
    [RoutePrefix("Employees")]
    [Version(1)]
    public class EmployeesV1Controller : ArchivingApiController<Employee>
    {
        #region Direct Employee Manipulation

        /// <summary>
        /// Gets all of the available end points from the <see cref="Employee">Employees</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets all <see cref="Employee">Employees</see> in the system.
        /// </summary>
        /// <returns>A GetEmployeeResponse containing all <see cref="Employee">Employees</see> and dependencies</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public GetEmployeesResponse GetAll()
        {
            return this.GetAll<GetEmployeesResponse>();
        }

        /// <summary>
        /// Gets the <see cref="Employee">Employee</see> matching the specified id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Employee">Employee</see> to get.</param>
        /// <returns>An EmployeeResponse containing the matching <see cref="Employee">Employee</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public EmployeeResponse Get([FromUri] int id)
        {
            return this.Get<EmployeeResponse>(id);
        }

        /// <summary>
        /// Adds an <see cref="Employee">Employee</see>. You can leave out much of the information initially.
        /// You can update the record with corporate cards, cars, pool car associations, mobile devices, access role associations, 
        /// work and home addresses and item roles by using the respective resources.
        /// </summary>
        /// <param name="request"><see cref="Employee">Employee</see> to be added</param>
        /// <returns>An EmployeeResponse containing the newly added <see cref="Employee">Employee</see> and dependencies</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Add)]
        public EmployeeResponse Post([FromBody] Employee request)
        {
            return this.CheckForWarnings(this.Post<EmployeeResponse>(request));
        }

        /// <summary>
        /// Updates the <see cref="Employee">Employee</see> record. Either provide only the elements to be updated or get the element data 
        /// and then provide the modified record. Elements that support a ForDelete flag can be deleted. 
        /// If the ForDelete flag is not supported, please provide the complete list for that element if the list is being modified.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Employee">Employee</see> to edit.</param>
        /// <param name="request">The <see cref="Employee">Employee</see> to edit.</param>
        /// <returns>An EmployeeResponse containing the edited <see cref="Employee">Employee</see></returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeResponse Put([FromUri]int id, [FromBody]Employee request)
        {
            request.EmployeeId = id;
            return this.CheckForWarnings(this.Put<EmployeeResponse>(request));
        }

        /// <summary>
        /// Activates an Employee, optionally sending welcome and password emails. Provide a list of Employee Ids to activate.
        /// </summary>
        /// <param name="sendWelcomeAndPasswordEmail">Whether to send the employees welcome and password emails.</param>
        /// <param name="request">A list of <see cref="Employee">Employee</see> Ids to activate.</param>
        /// <returns>An EmployeeActivateResponse containing Lists of those who were and weren't activated.</returns>
        [HttpPatch, Route("ActivateEmployees/{sendWelcomeAndPasswordEmail:bool}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeActivateResponse ActivateEmployees([FromUri]bool sendWelcomeAndPasswordEmail, [FromBody]ActivateEmployeesRequest request)
        {
            var response = this.InitialiseResponse<EmployeeActivateResponse>();
            response = ((EmployeeRepository)this.Repository).ActivateEmployees(request.EmployeeIds, sendWelcomeAndPasswordEmail, response);
                return response;
            }

        /// <summary>
        /// Deletes an <see cref="Employee">Employee</see>.
        /// </summary>
        /// <param name="id"><see cref="Employee">Employee</see> Id</param>
        /// <returns>An EmployeeResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Delete)]
        public EmployeeResponse Delete(int id)
        {
            return this.Delete<EmployeeResponse>(id);
        }

        /// <summary>
        /// Archives or un-archives a <see cref="Employee">Employee</see>, depending on what is passed in.
        /// </summary>
        /// <param name="id">The id of the Employee to be archived/un-archived.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="Employee">Employee</see>.</param>
        /// <returns>An EmployeeResponse containing the freshly archived Item.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeResponse Archive(int id, bool archive)
        {
            return this.Archive<EmployeeResponse>(id, archive);
        }

        /// <summary>
        /// Finds all <see cref="Employee">Employees</see> matching the specified criteria.
        /// </summary>
        /// <param name="criteria">The search criteria for this request.</param>
        /// <returns>A FindEmployeeResponse containing matching <see cref="Employee">Employees</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public FindEmployeesResponse Find([FromUri] FindEmployeeRequest criteria)
        {
            var findEmployeesResponse = this.InitialiseResponse<FindEmployeesResponse>();
            var conditions = new List<Expression<Func<Employee, bool>>>();

            SetSearchConditions(criteria, conditions);

            findEmployeesResponse.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            var fEmployees = findEmployeesResponse.List.Select(employee => employee.EmployeeId != null ? this.Repository.Get(employee.EmployeeId.Value) : null);
            findEmployeesResponse.List = fEmployees.ToList();

            return findEmployeesResponse;
        }

  
        /// <summary>
        /// Finds all <see cref="Employee">Passengers</see> matching the specified criteria.
        /// </summary>
        /// <param name="criteria">The search criteria for this request.</param>
        /// <returns>A FindEmployeeResponse containing a stripped down version of <see cref="Employee">Employees</see> with just basic passenger information.</returns>
        [HttpPost, Route("PassengerSearch")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public FindEmployeesResponse PassengerSearch([FromBody] FindPassengerRequest request)
        {
            var findEmployeesResponse = this.InitialiseResponse<FindEmployeesResponse>();
          
            findEmployeesResponse.List =
                ((EmployeeRepository)this.Repository).GetPassengerSearchResults(request.Criteria);

            return findEmployeesResponse;
        }

        /// <summary>
        /// Changes an <see cref="Employee">Employees</see> password.
        /// </summary>
        /// <param name="id">The id of the <see cref="Employee">Employee</see> for whom the password will be changed.</param>
        /// <param name="newPassword">The <see cref="Employee">Employee</see> new password.</param>
        /// <returns><see cref="ChangePasswordResponse">A ChangePasswordResponse</see>.</returns>
        [HttpPatch, Route("{id:int}/ChangePassword")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public ChangePasswordResponse ChangePassword(int id, [FromBody] string newPassword)
        {
            var response = this.InitialiseResponse<ChangePasswordResponse>();
            response = ((EmployeeRepository)this.Repository).ChangePassword(id, newPassword, response);
            return response;
        }

        #endregion Direct Employee Manipulation

        #region Transaction

        /// <summary>
        /// Gets the number of transations that the API user should be billed for.
        /// </summary>
        public override int TransactionCount
        {
            get
            {
                return ((EmployeeRepository)this.Repository).GetTransactionCount();
            }
        }

        #endregion
        

        #region Private Methods

        private EmployeeResponse CheckForWarnings(EmployeeResponse response)
        {
            if (response.ResponseInformation.Status == ApiStatusCode.Success)
            {
                EmployeeRepository repository = (EmployeeRepository)this.Repository;
                if (repository.WarningMessages.Count > 0)
                {
                    response.ResponseInformation.Status = ApiStatusCode.PartialSuccessWithWarnings;
                    List<ApiErrorDetail> errorDetails =
                        repository.WarningMessages.Select(
                            msg => new ApiErrorDetail { ErrorCode = "Warning", Message = msg }).ToList();
                    response.ResponseInformation.Errors = errorDetails;
                }
            }

            return response;
        }

        /// <summary>
        /// Sets the search conditions
        /// </summary>
        /// <param name="criteria">The criteria</param>
        /// <param name="conditions">The conditions</param>
        private static void SetSearchConditions(FindEmployeeRequest criteria, List<Expression<Func<Employee, bool>>> conditions)
        {
            if (!string.IsNullOrWhiteSpace(criteria.UserName))
            {
                conditions.Add(employee => employee.UserName.ToLower().Contains(criteria.UserName.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Title))
            {
                conditions.Add(employee => employee.Title.ToLower().Contains(criteria.Title.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Forename))
            {
                conditions.Add(employee => employee.Forename.ToLower().Contains(criteria.Forename.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Surname))
            {
                conditions.Add(employee => employee.Surname.ToLower().Contains(criteria.Surname.Trim().ToLower()));
            }
        }

        #endregion
    }
}