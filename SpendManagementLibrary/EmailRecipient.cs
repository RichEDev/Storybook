namespace SpendManagementLibrary
{
    /// <summary>
    /// Email Recipient class
    /// </summary>
    public class EmailRecipient
    {
        /// <summary>
        /// creates an instance of Email Recipient
        /// </summary>
        public EmailRecipient()
        {

        }

        /// <summary>
        /// Creates an instance of Email Receipient
        /// </summary>
        /// <param name="employeeId">The Id of the employee</param>
        /// <param name="emailAddress">The email address of the employee</param>
        public EmailRecipient(int employeeId, string emailAddress)
        {
            this.EmployeeId = employeeId;
            this.EmailAddress = emailAddress;
        }

        /// <summary>
        /// The id of the employee
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// The email adress of the employee
        /// </summary>
        public string EmailAddress { get; set; }
    }
}