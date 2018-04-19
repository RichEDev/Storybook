namespace BusinessLogic.GeneralOptions.SessionTimeout
{
    /// <summary>
    /// Defines a <see cref="SessionTimeoutOptions"/> and it's members
    /// </summary>
    public class SessionTimeoutOptions : ISessionTimeoutOptions
    {
        /// <summary>
        /// Gets or sets the idle timeout
        /// </summary>
        public int IdleTimeout { get; set; }

        /// <summary>
        /// Gets or sets the countdown timer
        /// </summary>
        public int CountdownTimer { get; set; }
    }
}
