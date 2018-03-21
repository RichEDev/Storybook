namespace Spend_Management.shared.code.DVLA
{
    using System;

    using Common.Logging;

    using DutyOfCareAPI.DutyOfCare;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// A class to manage creation of logs sin a customer database
    /// </summary>
    public class LicenceCheckVehicleLookupLogger : ILookupLogger
    {
         /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<LicenceCheckVehicleLookupLogger>().GetLogger();
        
        /// <summary>
        /// A private instance of the _current user.
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenceCheckVehicleLookupLogger"/> class.
        /// </summary>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        public LicenceCheckVehicleLookupLogger(ICurrentUser currentUser)
        {
            this._currentUser = currentUser;
        }

        /// <summary>
        /// Log a new entry for registration, code and message
        /// </summary>
        /// <param name="registration">The registration used for the lookup</param>
        /// <param name="code">The return code</param>
        /// <param name="message">The return message</param>
        public void Write(string registration, string code, string message)
        {
            if (code != "200" && code != "204")
            {
                Log.Warn($"Vehicle Lookup for {registration} failed: Vehicle Lookup Http response {code} : {message}");
            }

            using (var connection = new DatabaseConnection(this._currentUser.Account.ConnectionString))
            {
                connection.AddWithValue("@employeeid", this._currentUser.EmployeeID);
                if (this._currentUser.isDelegate)
                {
                    connection.AddWithValue("@delegateid", this._currentUser.Delegate.EmployeeID);    
                }
                else
                {
                    connection.AddWithValue("@delegateid", DBNull.Value);
                }
                
                connection.AddWithValue("@registration", registration);
                connection.AddWithValue("@code", code);
                connection.AddWithValue("@message", message);
                connection.ExecuteProc("dbo.SaveVehicleLookupLog");
            }
        }

        /// <summary>
        /// Log a new entry for registration, code and message
        /// </summary>
        /// <param name="registration">The registration used for the lookup</param>
        /// <param name="code">The return code</param>
        /// <param name="message">The return message</param>
        public void Write(string registration, int code, string message)
        {
            this.Write(registration, code.ToString(), message);
        }
    }
}