using System.Web.Http;

namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Types.MyDetails;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Manages operations on <see cref="MyDetail">MyDetails</see>.
    /// </summary>
    [Version(1)]
    [RoutePrefix("MyDetails")]
    public class MyDetailsV1Controller : BaseApiController<MyDetail>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="MyDetail">MyDetail</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>
        /// Gets the <see cref="MyDetail">Employee</see> matching the specified id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="MyDetail">Employee</see> to get.</param>
        /// <returns>An EmployeeResponse containing the matching <see cref="MyDetail">Employee</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public MyDetailResponse Get([FromUri] int id)
        {
            return this.Get<MyDetailResponse>(id);
        }
  
        /// <summary>
        /// Gets the <see cref="MyDetail">Employee</see> for the current employee
        /// </summary>
        /// <returns>An EmployeeResponse containing <see cref="MyDetail">Employee</see> for the current employee</returns>
        [HttpGet, Route("GetMyDetails")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MyDetailResponse GetMyDetails()
        {
            var response = this.InitialiseResponse<MyDetailResponse>();
            response.Item = ((MyDetailRepository)this.Repository).GetMyDetails();
            return response;
        }

        /// <summary>
        /// Updates the "My Details" part of an Employee.
        /// </summary>
        /// <param name="request">
        /// The <see cref="MyDetailRequest">MyDetailRequest</see> with the values to update.
        /// </param>
        /// <returns>
        /// The <see cref="MyDetailResponse">MyDetailResponse</see> with the updated values.
        /// </returns>
        [HttpPut, Route("ChangeMydetail")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public MyDetailResponse ChangeMydetail([FromBody] MyDetailRequest request)
        {
            var response = this.InitialiseResponse<MyDetailResponse>();
            response.Item = ((MyDetailRepository)this.Repository).ChangeMyDetails(request);
            return response;
        }

        /// <summary>
        /// Sends an email to the account administrator with the employee's requested changes to their "My Detail" part of an employee
        /// </summary>
        /// <param name="changeDetails">
        /// The change details.
        /// </param>
        /// <returns> 
        /// The <see cref="NotifyAdminOfChangeResponse">NotifyAdminOfChangeResponse</see> with the outcome of the action
        /// </returns>
        [HttpPut, Route("NotifyAdminOfChange")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NotifyAdminOfChangeResponse NotifyAdminOfChange([FromBody] string changeDetails)
        {
            var response = this.InitialiseResponse<NotifyAdminOfChangeResponse>();
            response.Item = ((MyDetailRepository)this.Repository).NotifyAdminOfChange(changeDetails);
            return response;
        }
    }
}