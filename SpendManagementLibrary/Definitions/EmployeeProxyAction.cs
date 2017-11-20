namespace SpendManagementLibrary.Definitions
{
    using System;
    using Utilities.Cryptography;

/// <summary>
/// A class to manage the query string for the Employee Proxy Page.
/// </summary>
    public class EmployeeProxyAction
    {
        /// <summary>
        /// The current action ID
        /// 1 = Add
        /// 3 = Delete
        /// </summary>
        public int Action { get; set; }


        /// <summary>
        /// The employee ID
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// The session ID this was created in
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="EmployeeProxyAction"/>
        /// </summary>
        /// <param name="action">The action ID 1 or 3</param>
        /// <param name="employeeId">The employee ID</param>
        /// <param name="sessionId">The current session id</param>
        public EmployeeProxyAction(int action, int employeeId, string sessionId)
        {
            this.Action = action;
            this.EmployeeId = employeeId;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Create a new instance of <see cref="EmployeeProxyAction"/>
        /// </summary>
        /// <param name="encodedProxyInformation">A string with the encoded Proxy Information</param>
        public EmployeeProxyAction(string encodedProxyInformation)
        {
            var crypt = new ExpensesCryptography();
            var decrypted = crypt.DecryptString(encodedProxyInformation.Replace(" ", "+")).Split(',');
            if (decrypted.Length != 3)
            {
                throw new InvalidOperationException("encodedProxyInformation");
            }

            this.ExtractValuesFromArray(decrypted);
        }

        /// <summary>
        /// Create a new instance of <see cref="EmployeeProxyAction"/>
        /// </summary>
        /// <param name="partial">A string <seealso cref="Array"/>of strings.  0 = Action, 1 = Session, 2 = Employee ID</param>
        public EmployeeProxyAction(string[] partial)
        {
            if (partial.Length != 3)
            {
                throw new InvalidOperationException("encodedProxyInformation");
            }

            this.ExtractValuesFromArray(partial);
        }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return ExpensesCryptography.Encrypt($"{this.Action},{this.SessionId},{this.EmployeeId}");
        }

        private void ExtractValuesFromArray(string[] partial)
        {
            int actionId;
            int.TryParse(partial[0], out actionId);
            this.Action = actionId;
            if (this.Action != 1 && this.Action != 3)
            {
                throw new InvalidOperationException("Action must be 1 or 3");
            }

            int employeeId;
            int.TryParse(partial[2], out employeeId);
            this.EmployeeId = employeeId;

            this.SessionId = partial[1];
        }

        
    }
}
