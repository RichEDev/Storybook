namespace SpendManagementLibrary
{
    using System.Collections;

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
        /// <returns>The index of the added item</returns>
        public int Add(int employeeId, string emailAddress)
        {
            if (employeeId <= 0 || string.IsNullOrEmpty(emailAddress))
            {
                return 0;
            }

            EmailRecipient recipient = new EmailRecipient
            {
                EmployeeId = employeeId,
                EmailAddress = emailAddress
            };

            return this.List.Add(recipient);
        }
    }
}