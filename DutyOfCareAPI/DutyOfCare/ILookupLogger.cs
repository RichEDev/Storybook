namespace DutyOfCareAPI.DutyOfCare
{
    /// <summary>
    /// An interface to define the logging required for vehicle lookups.
    /// </summary>
    public interface ILookupLogger
    {
        /// <summary>
        /// Log a new entry for <param name="registration"></param><param name="code"></param> and <param name="message"></param>
        /// </summary>
        /// <param name="registration">The registration used for the lookup</param>
        /// <param name="code">The return code</param>
        /// <param name="message">The return message</param>
        void Write(string registration, string code, string message);

        /// <summary>
        /// Log a new entry for <param name="registration"></param><param name="code"></param> and <param name="message"></param>
        /// </summary>
        /// <param name="registration">The registration used for the lookup</param>
        /// <param name="code">The return code</param>
        /// <param name="message">The return message</param>
        void Write(string registration, int code, string message);
    }
}