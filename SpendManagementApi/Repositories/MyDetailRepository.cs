namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Types.MyDetails;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    /// <summary>
    /// Manages data access for <see cref="MyDetail">MyDetail</see>
    /// </summary>
    internal class MyDetailRepository : BaseRepository<MyDetail>, ISupportsActionContext
    {
        /// <summary>
        /// The employees.
        /// </summary>
        private readonly cEmployees employees;

        #region Constructor

        public MyDetailRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, emp => emp.Id, emp => emp.UserName)
        {
            this.employees = this.ActionContext.Employees;
        }

        #endregion

        public override IList<MyDetail> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get employee details by employeeId.
        /// </summary>
        /// <param name="id">
        /// The employee id.
        /// </param>
        /// <returns>
        /// Employee details <see cref="MyDetail"/>.
        /// </returns>
        public override MyDetail Get(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an instance of <see cref="MyDetail">MyDetail</see> for the current employee
        /// </summary>
        /// <returns>
        /// An instance of <see cref="MyDetail">MyDetail</see>.
        /// </returns>
        public MyDetail GetMyDetails()
        {         
            Employee employee = this.employees.GetEmployeeById(this.User.EmployeeID);
            return employee.CastEmploye<MyDetail>(User, ActionContext);
        }

        /// <summary>
        /// Updates the "My Details" part of an Employee.
        /// </summary>
        /// <param name="request">
        /// The <see cref="MyDetailRequest">MyDetailRequest</see> with the data to update.
        /// </param>
        /// <returns>
        /// The <see cref="MyDetail">MyDetail</see> with the updated values.
        /// </returns>
        public MyDetail ChangeMyDetails(MyDetailRequest request)
        {
            if (this.User.EmployeeID != request.EmployeeId)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEmployeeIdDoesNotBelongToCurrentUser);
            }

            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountProperties subAccountProperties = subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;

            if (!subAccountProperties.EditMyDetails)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAccountDoesNotPermitEditOfMyDetails);
            }

            var employee = this.employees.GetEmployeeById(request.EmployeeId);

            if (employee == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            try
            {
                employee.Forename = request.Forename;
                employee.Surname = request.Surname;
                employee.Title = request.Title;
                employee.TelephoneExtensionNumber = request.TelephoneExtensionNumber;
                employee.MobileTelephoneNumber = request.MobileNumber;
                employee.PagerNumber = request.PagerNumber;
                employee.EmailAddress = request.EmailAddress;
                employee.HomeEmailAddress = request.PersonalEmailAddress;
                employee.TelephoneNumber = request.TelephoneNumber;
                employee.Save(this.User);
            }
            catch (Exception)
            {
                throw new InvalidDataException(ApiResources.ApiErrorInEmployeeUpdateMessage);
            }

            return this.GetMyDetails();
        }

        /// <summary>
        /// Sends an email to the account admin with the employee's requested changes to their "My Detail" part of an employee
        /// </summary>
        /// <param name="detailsOfChange">
        /// The details of change.
        /// </param>
        /// <returns>
        /// The <see cref="NotifyAdminOfChange">NotifyAdminOfChange</see> with the outcome of the action
        /// </returns>
        public NotifyAdminOfChange NotifyAdminOfChange(string detailsOfChange)
        {
            var subAccounts = new cAccountSubAccounts(User.AccountID);
            cAccountProperties reqProperties = subAccounts.getFirstSubAccount().SubAccountProperties;

            NotifyAdminOfChangesOutcome actionOutcome = NotifyAdminOfChangesOutcome.EmailSentSuccessfully ;

            if (reqProperties.MainAdministrator == 0)
            {
                actionOutcome = NotifyAdminOfChangesOutcome.AccountHasNoAdministrator;
            }

            if (reqProperties.EmailAdministrator == string.Empty)
            {
                actionOutcome = NotifyAdminOfChangesOutcome.AccountDoesNotPermitChangeOfDetailsNotifications;
            }

            if (!reqProperties.AllowEmployeeToNotifyOfChangeOfDetails)
            {
                actionOutcome = NotifyAdminOfChangesOutcome.AccountDoesNotPermitChangeOfDetailsNotifications;
            }

            if (actionOutcome != NotifyAdminOfChangesOutcome.EmailSentSuccessfully)
            {
                return new NotifyAdminOfChange { ActionOutcome = actionOutcome };
            }

            var emailSender = new EmailSender(reqProperties.EmailServerAddress);

            SpendManagementLibrary.Enumerators.NotifyAdminOfChangesOutcome notifyOutcome = this.ActionContext.Employees.NotifyAdminOfChanges(
                User,
                detailsOfChange,
                new cModules(),
                reqProperties,
                emailSender);

            return new NotifyAdminOfChange { ActionOutcome = (NotifyAdminOfChangesOutcome)notifyOutcome };
        }
    }    
}