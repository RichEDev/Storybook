namespace Spend_Management.shared.code
{
    using System.Collections.Generic;
    using System.Linq;
    using BusinessLogic;
    using BusinessLogic.Modules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using Spend_Management.expenses.code;
	using System.Threading.Tasks;
    using System.Configuration;

	/// <summary>
	/// A helper for building email notifications
	/// </summary>
	public class NotificationHelper
    {
        /// <summary>
        /// The employee who the email is about
        /// </summary>
        public Employee Employee {get; set;}

        /// <summary>
        /// The account properties
        /// </summary>
        public cAccountProperties AccountProperties { get; set; }

        /// <summary>
        /// The email sender
        /// </summary>
        public EmailSender EmailSender { get; set; }

        /// <summary>
        /// The address the email will come from
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Constructor for the EmailNotificationHelper class
        /// </summary>
        /// <param name="employee">The current employee</param>
        public NotificationHelper(Employee employee)
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
			var emailTemplates = new NotificationTemplates(user);
			Task.Run(() => emailTemplates.SendMessage(SendMessageEnum.GetEnumDescription(sendMessageDescription), this.Employee.EmployeeID, notificationEmployees.ToArray(), this.Employee.EmployeeID));
        }

        /// <summary>
        /// Sends an email to all employees who are a member of the Excess Mileage notification group
        /// </summary>
        public void ExcessMileage()
        {
            Guard.ThrowIfNull(this.Employee, nameof(this.Employee));
            if (this.Employee.ExcessMileage <= 0)
            {
                return;
            }

            this.Send(SendMessageDescription.SentToTheExcessMileageNotificationGroupWhenAClaimantsAddressChanges
                , EmailNotificationType.ExcessMileage
                , Modules.Expenses);
        }

        /// <summary>
        /// Gets the from address for the emails
        /// </summary>
        /// <returns>The from address for the emails</returns>
        private string GetFromEmailAddress()
        {
            var fromAddress = ConfigurationManager.AppSettings["AdminEmailAddress"];
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
        private List<int> GetEmployeesToNotify(EmailNotificationType emailNotificationType)
        {
            var notificationSubscriptions = new Notifications(this.Employee.AccountID).GetNotificationSubscriptions(emailNotificationType);
            var notificationEmployeeIds = new List<int>(from notificationSubscription in notificationSubscriptions where (sendType)notificationSubscription[0] == sendType.employee select (int)notificationSubscription[1]);

            return notificationEmployeeIds;
        }
    }
}