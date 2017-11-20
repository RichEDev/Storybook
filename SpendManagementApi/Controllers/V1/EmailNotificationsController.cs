namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// Contains operations for managing <see cref="EmailNotification">EmailNotifications</see>.
    /// </summary>
    [RoutePrefix("EmailNotifications")]
    [Version(1)]
    public class EmailNotificationsV1Controller : BaseApiController<EmailNotification>
    {
        /// <summary>
        /// Gets all of the available end points from the <see cref="EmailNotification">EmailNotifications</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Returns list of standard <see cref="EmailNotification">EmailNotifications</see>.
        /// </summary>
        [HttpGet, Route("getStandardNotifications")]
        [AuthAudit(SpendManagementElement.Emails, AccessRoleType.View)]
        public List<EmailNotification> GetStandardNotifications()
        {
            var standardNotifications = new cEmailNotifications(this.CurrentUser.AccountID);
            return standardNotifications.EmailNotificationsByCustomerType(CustomerType.Standard)
                    .Select(x => new EmailNotification().From(x.Value, null))
                    .ToList();
        }

        /// <summary>
        /// Returns list of NHS <see cref="EmailNotification">EmailNotifications</see>.
        /// </summary>
        [HttpGet, Route("getNhsNotifications")]
        [AuthAudit(SpendManagementElement.Emails, AccessRoleType.View)]
        public List<EmailNotification> GetNhsNotifications()
        {
            if (this.CurrentUser.Account.IsNHSCustomer)
            {
                var nhsNotifications = new cEmailNotifications(this.CurrentUser.AccountID);
                return nhsNotifications.EmailNotificationsByCustomerType(CustomerType.NHS).Select(x => x.Value).Select(e => new EmailNotification().From(e, null)).ToList();
            }
            return new List<EmailNotification>();
        }

        /// <summary>
        /// Applies the supplied <see cref="EmailNotification">EmailNotification</see> to the supplied <see cref="Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="EmailNotification">EmailNotification</see> to apply.</param>
        /// <param name="eid">The Id of the <see cref="Employee">Employee</see>.</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/AssignToEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse AssignToEmployee([FromUri] int id, [FromUri] int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            this.ValidateAndAddOrRemoveEmailNotificationLink(id, eid, true);
            response.EmployeeId = eid;
            response.LinkedItemId = id;
            return response;
        }

        /// <summary>
        /// Unlinks the supplied <see cref="AccessRole">AccessRole</see> from the supplied <see cref="Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="AccessRole">AccessRole</see> to revoke.</param>
        /// <param name="eid">The Id of the <see cref="Employee">Employee</see>.</param>
        /// <returns>An EmployeeLinkageResponse, containing the two Ids.</returns>
        [HttpPatch, Route("{id:int}/RevokeFromEmployee/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public EmployeeLinkageResponse RevokeFromEmployee([FromUri]int id, [FromUri]int eid)
        {
            var response = this.InitialiseResponse<EmployeeLinkageResponse>();
            this.ValidateAndAddOrRemoveEmailNotificationLink(id, eid, false);
            response.EmployeeId = eid;
            response.LinkedItemId = id;
            return response;
        }

        /// <summary>
        /// Init.
        /// </summary>
        protected override void Init()
        {
        }

        /// <summary>
        /// Validates the employee and email notification exists, then depending on the supplied value,
        /// subscribes to (true), or unsubscribes from (false) the email notification.
        /// </summary>
        /// <param name="notificationId">The notification id.</param>
        /// <param name="employeeId">The employee id.</param>
        /// <param name="theyAreToBeLinked">Whether to link or unlink.</param>
        private void ValidateAndAddOrRemoveEmailNotificationLink(int notificationId, int employeeId, bool theyAreToBeLinked)
        {
            var accountId = this.CurrentUser.AccountID;
            var employees = new cEmployees(accountId);
            var emp = employees.GetEmployeeById(employeeId);
            if (emp == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var notifications = new cEmailNotifications(accountId);
            var not = notifications.GetEmailNotificationByID(notificationId);

            if (not == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmailNotificationId, ApiResources.ApiErrorWrongEmailNotificationIdMessage);
            }


            var idList = emp.GetEmailNotificationList().EmailNotificationIDs;
            if (theyAreToBeLinked)
            {
                if (!idList.Contains(not.EmailNotificationID))
                {
                    idList.Add(not.EmailNotificationID);
                }
            }
            else
            {
                if (idList.Contains(not.EmailNotificationID))
                {
                    idList.Remove(not.EmailNotificationID);
                }
            }

            notifications.SaveNotificationLink(idList, emp.EmployeeID, null);
        }

    }
}
