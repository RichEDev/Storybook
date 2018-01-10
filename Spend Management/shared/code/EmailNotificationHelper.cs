﻿namespace Spend_Management.shared.code
{
    using System.Collections.Generic;
    using System.Linq;
    using BusinessLogic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using Spend_Management.expenses.code;

    /// <summary>
    /// A helper for building email notifications
    /// </summary>
    public class EmailNotificationHelper
    {
        /// <summary>
        /// The employee who the email is about
        /// </summary>
        public Employee Employee;

        /// <summary>
        /// The account properties
        /// </summary>
        public cAccountProperties AccountProperties;

        /// <summary>
        /// The email sender
        /// </summary>
        public EmailSender EmailSender;

        /// <summary>
        /// The address the email willcome from
        /// </summary>
        public string FromAddress;

        /// <summary>
        /// Constructor for the EmailNotificationHelper class
        /// </summary>
        /// <param name="employee">The current employee</param>
        public EmailNotificationHelper(Employee employee)
        {
            this.Employee = employee;
            this.AccountProperties = new cAccountSubAccountsBase(this.Employee.AccountID).getSubAccountById(this.Employee.DefaultSubAccount).SubAccountProperties;
            this.EmailSender = new EmailSender(this.AccountProperties.EmailServerAddress);
            this.FromAddress = this.GetFromEmailAddress();
        }

        /// <summary>
        /// Sends a emails to a email notification group
        /// </summary>
        /// <param name="sendMessageDescription">The email description</param>
        /// <param name="emailNotificationType">The email notification type</param>
        /// <param name="modules">The module type</param>
        public void Send(SendMessageDescription sendMessageDescription, EmailNotificationType emailNotificationType, Modules modules)
        {
            var notificationEmployees = this.GetEmployeesToNotify(emailNotificationType);
            var user = new CurrentUser(this.Employee.AccountID, this.Employee.EmployeeID, 0, modules, this.Employee.DefaultSubAccount);
            var emailTemplates = new cEmailTemplates(user);

            var recipients = notificationEmployees.Select(notificationEmployee => notificationEmployee.EmployeeID).ToList();
            emailTemplates.SendMessage(SendMessageEnum.GetEnumDescription(sendMessageDescription), this.Employee.EmployeeID, recipients.ToArray(), this.Employee.EmployeeID);
        }

        /// <summary>
        /// Sends an email to all employees who are a member of the Excess Mileage notification group
        /// </summary>
        public void ExcessMileage()
        {
            Guard.ThrowIfNull(this.Employee, nameof(this.Employee));
            if (this.Employee.ExcessMileage <= 0) return;
            this.Send(SendMessageDescription.SentToTheExcessMileageNotificationGroupWhenAClaimantsHomeOrWorkAddressChangesAndTheyHaveAnExcessMileageValueSet
                , EmailNotificationType.ExcessMileage
                , Modules.expenses);
        }

        /// <summary>
        /// Gets the from address for the emails
        /// </summary>
        /// <returns>The from address for the emails</returns>
        public string GetFromEmailAddress()
        {
            var fromAddress = "admin@sel-expenses.com";
            if (this.AccountProperties.SourceAddress == 1 && this.AccountProperties.EmailAdministrator != string.Empty)
            {
                fromAddress = this.AccountProperties.EmailAdministrator;
            }
            else if (this.AccountProperties.SourceAddress == 0 && this.Employee.EmailAddress != string.Empty)
            {
                fromAddress = this.Employee.EmailAddress;
            }

            return fromAddress;
        }

        /// <summary>
        /// Gets the employees to send the type of notification to
        /// </summary>
        /// <param name="emailNotificationType">The notification type</param>
        /// <returns>A list of employees</returns>
        public List<Employee> GetEmployeesToNotify(EmailNotificationType emailNotificationType)
        {
            var notificationSubscriptions = new cEmailNotifications(this.Employee.AccountID).GetNotificationSubscriptions(emailNotificationType);
            var notificationEmployeeIds = new List<int>(from notificationSubscription in notificationSubscriptions where (sendType)notificationSubscription[0] == sendType.employee select (int)notificationSubscription[1]);

            return notificationEmployeeIds.Select(employeeid => Employee.Get(employeeid, this.Employee.AccountID)).ToList();
        }
    }
}