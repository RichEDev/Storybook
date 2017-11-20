using System.Collections;

namespace SpendManagementLibrary
{
    /// <summary>
    /// A collection of Email Receipients
    /// </summary>
    public class EmailRecipients : CollectionBase
    {
        /// <summary>
        /// Adds an instance of Email Recipient 
        /// </summary>
        /// <param name="employeeId">The id of the employee</param>
        /// <param name="emailAddress">The email address of the employee</param>
        /// <returns></returns>
        public int Add(int employeeId, string emailAddress)
        {
            EmailRecipient recipient = new EmailRecipient
            {
                EmployeeId = employeeId,
                EmailAddress = emailAddress
            };

            return this.List.Add(recipient);
        }
    }
}