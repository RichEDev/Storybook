using System;
using Common.Logging;
using DutyOfCareAPI.DutyOfCare;
using SpendManagementLibrary.Helpers;

namespace Spend_Management.shared.code.DVLA
{
    /// <summary>
    /// A class to manage creation of logs sin a customer database
    /// </summary>
    public class VehicleLookupLogger : ILookupLogger
    {
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<VehicleLookupLogger>().GetLogger();

        /// <summary>
        /// Create a new instance of <see cref="VehicleLookupLogger"/>
        /// </summary>
        /// <param name="currentUser">An instance of the <see cref="ICurrentUser"/></param>
        public VehicleLookupLogger(ICurrentUser currentUser)
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
            if (code != "200")
            {
                Log.Warn($"Vehicle Lookup for {registration} failed: {code} : {message}");
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
    }
}